using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Diplom.Common.Models;
using Flurl.Http;
using PropertyChanged;

namespace MobileWorker.ViewModels
{
    // подключенная  PropertyChanged.Fody и файлик FodyWeavers.xml
    [AddINotifyPropertyChangedInterface]
    public class OrderListDetailViewModel
    {
        public ObservableCollection<BasketList> BasketList { get; set; }
        public BasketList SelectedBasket { get; set; }
        public string AllPrice => BasketList.Sum(x => x.OverallPrice).ToString();
        public string Deleted { get; set; }
        public string Update { get; set; }

        public OrderList OrderDetail { get; set; }

        public OrderListDetailViewModel(OrderList dell)
        {
            OrderDetail = dell;

            //получение записей выбранного заказа
            var basket = RequestBuilder.Create()
                                       .AppendPathSegments("api", "basket", "basketOneGet") // добавляет к ендпоинт
                                       .SetQueryParam("orderOne", OrderDetail.OrderId)
                                       .GetJsonAsync<BasketList[]>().GetAwaiter()
                                       .GetResult(); //  https://192.168.1.12:5002/api/basket/basketOneGet

            BasketList = new ObservableCollection<BasketList>(basket);
            //UpdatePrice().GetAwaiter().GetResult();
        }

       

        public void UpdatePrice()
        {
            try
            {
                var response = RequestBuilder.Create()
                                                   .AppendPathSegments("api", "order", "orderUpdatePrice") // добавляет к ендпоинт
                                                   .SetQueryParam("orderId", OrderDetail.OrderId)
                                                   .PostJsonAsync(null).GetAwaiter().GetResult();
                var ok = response.Content.ReadAsStringAsync();
                Update = ok.ToString();
            }
            catch (Exception ex)
            {
                var x = ex;
            }
        }
    }
}