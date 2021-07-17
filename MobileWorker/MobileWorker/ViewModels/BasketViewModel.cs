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
    public class BasketViewModel
    {
        public ObservableCollection<BasketList> BasketList { get; set; }
        public BasketList SelectedBasket { get; set; }
        public bool Access { get; set; }
        public int AllPrice => BasketList.Sum(x => x.OverallPrice);

        public ICommand QuantityPlusCommand { get; set; }
        public ICommand QuantityMinusCommand { get; set; }

        public BasketViewModel()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                BasketList = new ObservableCollection<BasketList>();
                return;
            }
            Access = false;
            var basket = RequestBuilder.Create()
                                       .AppendPathSegments("api", "basket", "basketGet") // добавляет к ендпоинт
                                       .GetJsonAsync<BasketList[]>()
                                       .GetAwaiter().GetResult(); //  https://192.168.1.12:5002/api/basket/basketGet
            //if(!basket.Any())
            //{
            //    Access = false;
            //    return;
            //}

            Access = true;
            BasketList = new ObservableCollection<BasketList>(basket);
            if (AllPrice == 0)
            {
                Access = false;
                return;
            }

            QuantityPlusCommand = new Command<BasketList>(commandParameter =>
            {
                commandParameter.Quantity++;
                if(commandParameter.Quantity > 10)
                {
                    commandParameter.Quantity = 10;
                }
            }, commandParameter => commandParameter != null);

            QuantityMinusCommand = new Command<BasketList>(commandParameter =>
            {
                commandParameter.Quantity--;
                if(commandParameter.Quantity < 1)
                {
                    commandParameter.Quantity = 1;
                }
            }, commandParameter => commandParameter != null);
        }

        //public ICommand DeketeCommand { get; set; }

        //Удаление целой записи
        public async Task DeleteBasket(BasketList del)
        {
            _ = await RequestBuilder.Create()
                                    .AppendPathSegments("api", "basket", "basketDell") // добавляет к ендпоинт
                                    .PostJsonAsync(del);
            var basket = await RequestBuilder.Create()
                                             .AppendPathSegments("api", "basket", "basketGet") // добавляет к ендпоинт
                                             .GetJsonAsync<BasketList[]>(); //  https://192.168.1.12:5002/api/basket/basketGet

            BasketList = new ObservableCollection<BasketList>(basket);

            //using (var db = new ApplicationContext())
            //{
            //    //удалить из локальной БД 
            //    db.BasketList.Remove(del);
            //    var basket = db.Basket.FirstOrDefault(x => x.BasketId == del.BasketId);
            //    //удалить из передаваемой таблицы
            //    db.Basket.Remove(basket);
            //    //удалиь из лист вьюш
            //    BasketList.Remove(del);
            //    NumberPrice();

            //    db.SaveChangesAsync();
            //}
        }

        //добавление одного кол-ва
        public async Task AddQuantity(BasketList del)
        {
            _ = await RequestBuilder.Create()
                                    .AppendPathSegments("api", "basket", "basketOneAdd") // добавляет к ендпоинт
                                    .PostJsonAsync(del);
            var basket = await RequestBuilder.Create()
                                             .AppendPathSegments("api", "basket", "basketGet") // добавляет к ендпоинт
                                             .GetJsonAsync<BasketList[]>(); //  https://192.168.1.12:5002/api/basket/basketOneAdd

            BasketList = new ObservableCollection<BasketList>(basket);

            //using (var db = new ApplicationContext())
            //{
            //    var quantity = BasketList.FirstOrDefault(x => x.BasketListId == del.BasketListId).Quantity;
            //    if (quantity >= 10 )
            //    {
            //        return;
            //    }
            //    else
            //    {
            //        db.BasketList.FirstOrDefault(x => x.BasketListId == del.BasketListId).Quantity++; //изменить в БД
            //        db.Basket.FirstOrDefault(x => x.BasketId == del.BasketId).Quantity++; //изменить в передаваемой таблице
            //        BasketList.FirstOrDefault(x => x.BasketListId == del.BasketListId).Quantity++; //изменить в лист вьюш
            //        db.SaveChangesAsync();

            //        //посчет цены с учетом изменения количества
            //        var Price = Convert.ToInt32(BasketList.FirstOrDefault(x => x.BasketListId == del.BasketListId).Price);//цена товара
            //        var Quantity = Convert.ToInt32(BasketList.FirstOrDefault(x => x.BasketListId == del.BasketListId).Quantity);//количество товара
            //        BasketList.FirstOrDefault(x => x.BasketListId == del.BasketListId).OverallPrice = Price * Quantity; //изменение цены с учетом кол-ва товара
            //        db.BasketList.FirstOrDefault(x => x.BasketListId == del.BasketListId).OverallPrice = Price * Quantity; //изменение цены с учетом кол-ва товара в БД

            //        //подсчет общей цены и передача на страницу
            //        NumberPrice();


            //        BasketList = new ObservableCollection<BasketList>(db.BasketList);
            //    }
            //}
        }

        //удаление одного кол-ва
        public async Task LowerQuantity(BasketList del)
        {
            _ = await RequestBuilder.Create()
                                               .AppendPathSegments("api", "basket", "basketOneDell") // добавляет к ендпоинт
                                               .PostJsonAsync(del);

            var basket = await RequestBuilder.Create()
                                             .AppendPathSegments("api", "basket", "basketGet") // добавляет к ендпоинт
                                             .GetJsonAsync<BasketList[]>(); //  https://192.168.1.12:5002/api/basket/basketOneDell

            BasketList = new ObservableCollection<BasketList>(basket);

            //using (var db = new ApplicationContext())
            //{
            //    var quantity = BasketList.FirstOrDefault(x => x.BasketListId == del.BasketListId).Quantity;
            //    if (quantity <= 1)
            //    {
            //        return;
            //    }
            //    else
            //    {
            //        db.BasketList.FirstOrDefault(x => x.BasketListId == del.BasketListId).Quantity--; //изменить в БД
            //        db.Basket.FirstOrDefault(x => x.BasketId == del.BasketId).Quantity--; //изменить в передаваемой таблице
            //        BasketList.FirstOrDefault(x => x.BasketListId == del.BasketListId).Quantity--; //изменить в лист вьюш
            //        db.SaveChangesAsync();

            //        //посчет цены с учетом изменения количества
            //        var Price = Convert.ToInt32(BasketList.FirstOrDefault(x => x.BasketListId == del.BasketListId).Price);
            //        var Quantity = Convert.ToInt32(BasketList.FirstOrDefault(x => x.BasketListId == del.BasketListId).Quantity);
            //        BasketList.FirstOrDefault(x => x.BasketListId == del.BasketListId).OverallPrice = Price * Quantity;
            //        db.BasketList.FirstOrDefault(x => x.BasketListId == del.BasketListId).OverallPrice = Price * Quantity;

            //        //подсчет общей цены и передача на страницу
            //        NumberPrice();


            //        BasketList = new ObservableCollection<BasketList>(db.BasketList);
            //    }

            //}
        }
    }
}