using Diplom.Common.Entities;
using Diplom.Common.Models;
using Flurl.Http;
using Plugin.Connectivity;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Diplom.Mobile.ViewModels
{
    // подключенная  PropertyChanged.Fody и файлик FodyWeavers.xml
    [AddINotifyPropertyChangedInterface]
    class ProductsViewModel
    {
        public ObservableCollection<Product> ProductList { get; set; }
        public bool IsBusy { get; set; } // последняя запись в БД или нет
        public int Skip { get; set; } // сколько записей пропустить при запросе
        public int SkipStage { get; set; } // кол-во загруженых записей
        public bool IsRefreshing { get; set; } // иконка загрузки
        public bool Error { get; set; } //ошибка

        //public ICommand RefreshCommand { get; set; }

        public ProductsViewModel(MenuType type)
        {
            IsBusy = true;
            IsRefreshing = false;
            SkipStage = 0;
            Skip = SkipStage * 5;
            if((int)type == -1)
            {
                type = MenuType.FirstDish;
            }

            // Если нет интернета
            if (!CrossConnectivity.Current.IsConnected)
            {
                ProductList = new ObservableCollection<Product>();
                //показываем из локальной БД 
                LoadMoreEmployerResultInLockal(type);
                return;
            }

            //получение записей 
            var product = RequestBuilder.Create()
                                     .AppendPathSegments("api", "product", "productTake") // добавляет к ендпоинт
                                     .SetQueryParam("skip", Skip)
                                     .SetQueryParam("type", (int)type)
                                     .GetJsonAsync<Product[]>()
                                     .GetAwaiter().GetResult(); //  http://192.168.0.4:5002/api/product/productTake
            if (!product.Any())
            {
                return;
            }

            // Занесение в локальную БД
            SaveDb(product).GetAwaiter().GetResult();

            using (var db = new ApplicationContext())
            {
                var data = db.Product.Where(x => x.Type == (MenuType)type).OrderBy(x => x.Name);
                ProductList = new ObservableCollection<Product>(data);
            }
            
            SkipStage++;

            //// Команда обновления списка
            //RefreshCommand = new Command<Product>(commandParameter =>
            //{
            //    IsBusy = true;
            //    IsRefreshing = true;
            //    SkipStage = 0;
            //    Skip = SkipStage * 5;
            //    if ((int)type == -1)
            //    {
            //        type = MenuType.Food;
            //    }

            //    // Если нет интернета
            //    if (!CrossConnectivity.Current.IsConnected)
            //    {
            //        //показываем из локальной БД 
            //        using (var db = new ApplicationContext())
            //        {
            //            LoadMoreEmployerResultInLockal(type);
            //        }
            //        return;
            //    }
            //    var products = RequestBuilder.Create()
            //                             .AppendPathSegments("api", "product", "productTake") // добавляет к ендпоинт
            //                             .SetQueryParam("skip", Skip)
            //                             .SetQueryParam("type", (int)type)
            //                             .GetJsonAsync<Product[]>()
            //                             .GetAwaiter().GetResult(); //  http://192.168.0.4:5002/api/product/productTake

            //    ProductList = new ObservableCollection<Product>(products);
            //    IsRefreshing = false;
            //});
        }
        // Выбор данных при смене типа продукта
        public async void Products(MenuType type)
        {
            IsBusy = true;
            if ((int)type == -1)
            {
                type = MenuType.FirstDish;
            }
            SkipStage = 0;
            // Если нет интернета
            if (!CrossConnectivity.Current.IsConnected)
            {
                ProductList = new ObservableCollection<Product>();
                ProductList = null;
                ProductList = new ObservableCollection<Product>();
                //показываем из локальной БД 
                LoadMoreEmployerResultInLockal(type);
                return;
            }
            ProductList = null;
            ProductList = new ObservableCollection<Product>();
            await LoadMoreEmployerResult(type);

        }
        // Динамическая подгрузка данных при прокручивании списка
        public async Task LoadMoreEmployerResult(MenuType type)
        {
            if ((int)type == -1)
            {
                type = MenuType.FirstDish;
            }
            IsRefreshing = true;
            Skip = 5 * SkipStage;

            //получение записей 
            var product = await RequestBuilder.Create()
                                           .AppendPathSegments("api", "product", "productTake") // добавляет к ендпоинт
                                           .SetQueryParam("skip", Skip)
                                           .SetQueryParam("type", (int)type)
                                           .GetJsonAsync<Product[]>(); //  http://192.168.0.4:5002/api/review/reviewTake
            if (!product.Any())
            {
                IsBusy = false;
                IsRefreshing = false;
                return;
            }

            SkipStage++;
            var xxx = ProductList;
            foreach (var x in product)
            {
                xxx.Add(x);
            }
            //заносим в локальную БД
            using (var db = new ApplicationContext())
            {
                // Отчистить записи полученного типа 
                var remove = db.Product.Where(x => x.Type == type).ToArray();
                db.Product.RemoveRange(remove);
                //db.Product.RemoveRange(db.Product);
                await db.SaveChangesAsync();

                await db.Product.AddRangeAsync(xxx);
                await db.SaveChangesAsync();

                var data = db.Product.Where(x => x.Type == (MenuType)type).OrderBy(x => x.Name);
                ProductList = new ObservableCollection<Product>(data);
            }

            IsBusy = true;
            IsRefreshing = false;
        }

        // Динамическая загрузка данных из локальной БД при отсутствии интернета
        public void LoadMoreEmployerResultInLockal(MenuType type)
        {
            if ((int)type == -1)
            {
                type = MenuType.FirstDish;
            }
            Error = false;
            IsRefreshing = true;
            Skip = 5 * SkipStage;

            // берем из локальной БД
            using (var db = new ApplicationContext())
            {
                const int take = 5;
                var products = db.Product.Where(x => x.Type == type).ToArray();
                if (!products.Any())
                {
                    Error = true;
                    IsRefreshing = false;
                    return;
                }

                var sortList = products.OrderBy(x => x.Name).ToList();
                var tske = sortList.Skip(Skip).Take(take);
                if (!tske.Any())
                {
                    IsBusy = false;
                }
                SkipStage++;
                var xxx = ProductList;
                foreach (var x in tske)
                {
                    xxx.Add(x);
                }
                var data = db.Product.Where(x => x.Type == (MenuType)type);
                
                ProductList = new ObservableCollection<Product>(data);
                Error = false;
                IsRefreshing = false;
            }
        }
        public async Task SaveDb(Product[] data)
        {
            //заносим в локальную БД
            using (var db = new ApplicationContext())
            {
                db.Product.RemoveRange(db.Product);
                await db.SaveChangesAsync();
                
                await db.Product.AddRangeAsync(data);
                await db.SaveChangesAsync();
            }
        }
    }
}
