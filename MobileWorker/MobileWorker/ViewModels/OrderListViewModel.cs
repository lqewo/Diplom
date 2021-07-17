using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Diplom.Common.Models;
using Flurl.Http;
using Plugin.Connectivity;
using PropertyChanged;
using Xamarin.Forms;

namespace MobileWorker.ViewModels
{
    // подключенная  PropertyChanged.Fody и файлик FodyWeavers.xml
    [AddINotifyPropertyChangedInterface]
    public class OrderListViewModel
    {
        public ObservableCollection<OrderList> OrderList { get; set; }
        public BasketList SelectedBasket { get; set; }
        public ConverterPaymentType TypePaymentt { get; set; }

        public bool IsBusy { get; set; } // последняя запись в БД или нет
        public int Skip { get; set; } // сколько записей пропустить при запросе
        public int SkipStage { get; set; } // кол-во загруженых записей
        public bool IsRefreshing { get; set; } // иконка загрузки
        public bool Error { get; set; } //ошибка
        public ICommand RefreshCommand { get; set; }

        public OrderListViewModel()
        {
            IsBusy = true;
            IsRefreshing = true;
            SkipStage = 0;
            Skip = SkipStage * 5;

            if (!CrossConnectivity.Current.IsConnected)
            {
                OrderList = new ObservableCollection<OrderList>();
                Error = true;
                LoadMoreEmployerResult();
                return;
            }

            var basket = RequestBuilder.Create()
                                       .AppendPathSegments("api", "order", "orderWorkerGet") // добавляет к ендпоинт
                                       .SetQueryParam("skip", Skip)
                                       .GetJsonAsync<OrderList[]>().GetAwaiter().GetResult(); //  https://192.168.1.12:5002/api/order/orderWorkerGet

            //Payment(basket);
            //    Status(basket);
            if (basket is null)
            {
                IsRefreshing = false;
                return;
            }

            SaveDb(basket).GetAwaiter().GetResult();

            OrderList = new ObservableCollection<OrderList>(basket);
            SkipStage++;
            IsRefreshing = false;

            // Обновление списка
            RefreshCommand = new Command<OrderList>(commandParameter =>
            {
                IsBusy = true;
                IsRefreshing = true;
                SkipStage = 0;
                Skip = SkipStage * 5;
                if (!CrossConnectivity.Current.IsConnected)
                {
                    //показываем из локальной БД 
                    using (var db = new ApplicationContext())
                    {
                        //ContentList = new ObservableCollection<Content>();
                        //LoadMoreEmployerResultInLockal();
                    }
                    return;
                }
                var basketUpdate = RequestBuilder.Create()
                                       .AppendPathSegments("api", "order", "orderWorkerGet") // добавляет к ендпоинт
                                       .SetQueryParam("skip", Skip)
                                       .GetJsonAsync<OrderList[]>().GetAwaiter().GetResult(); //  https://192.168.1.12:5002/api/order/orderWorkerGet

                //Payment(basket);
                //    Status(basket);
                if (basketUpdate is null)
                {
                    IsRefreshing = false;
                    return;
                }

                SaveDb(basket).GetAwaiter().GetResult();

                OrderList = new ObservableCollection<OrderList>(basketUpdate);
                SkipStage++;
                IsRefreshing = false;
            });
        }

        // динамическая подгрузка данных
        public async Task LoadMoreEmployerResult(OrderList itemTypeObject)
        {
            IsRefreshing = true;
            Skip = 5 * SkipStage;

            //получение записей 
            var basket = RequestBuilder.Create()
                                       .AppendPathSegments("api", "order", "orderWorkerGet") // добавляет к ендпоинт
                                       .SetQueryParam("skip", Skip)
                                       .GetJsonAsync<OrderList[]>().GetAwaiter().GetResult(); //  https://192.168.1.12:5002/api/order/orderWorkerGet
            if (!basket.Any())
            {
                IsBusy = false;
                IsRefreshing = false;
                return;
            }

            SkipStage++;
            var xxx = OrderList;
            foreach (var x in basket)
            {
                xxx.Add(x);
            }

            //заносим в локальную БД
            SaveDb(basket).GetAwaiter().GetResult();

            OrderList = new ObservableCollection<OrderList>(xxx);
            IsRefreshing = false;
        }


        // сохранение полученных данных в БД
        public async Task SaveDb(OrderList[] data)
        {
            try
            {
                using (var db = new ApplicationContext())
                {
                    db.OrderList.RemoveRange(db.OrderList);
                    await db.SaveChangesAsync();

                    await db.OrderList.AddRangeAsync(data);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                var x = ex;
            }
        }

        public async Task LoadMoreEmployerResult()
        {
            using (var db = new ApplicationContext())
            {
                var data = db.OrderList;
                OrderList = new ObservableCollection<OrderList>(data);
            }
        }
    }
}