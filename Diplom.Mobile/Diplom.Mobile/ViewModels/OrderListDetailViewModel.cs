using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Diplom.Common.Entities;
using Diplom.Common.Models;
using Flurl.Http;
using PropertyChanged;

namespace Diplom.Mobile.ViewModels
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
                                       .GetResult(); //  https://192.168.0.4:5002/api/basket/basketOneGet

            BasketList = new ObservableCollection<BasketList>(basket);
            //UpdatePrice().GetAwaiter().GetResult();
        }

        //уменьшение кол-ва
        public async Task LowerQuantity(BasketList del)
        {
            _ = await RequestBuilder.Create()
                                    .AppendPathSegments("api", "basket", "basketOneDell") // добавляет к ендпоинт
                                    .PostJsonAsync(del);

            var basket = await RequestBuilder.Create()
                                             .AppendPathSegments("api", "basket", "basketOneGet") // добавляет к ендпоинт
                                             .SetQueryParam("orderOne", OrderDetail.OrderId)
                                             .GetJsonAsync<BasketList[]>(); //  https://192.168.0.4:5002/api/basket/basketOneGet

            BasketList = new ObservableCollection<BasketList>(basket);

            UpdatePrice();

        }

        //прибавление кол-ва
        public async Task AddQuantity(BasketList del)
        {
            _ = await RequestBuilder.Create()
                                    .AppendPathSegments("api", "basket", "basketOneAdd") // добавляет к ендпоинт
                                    .PostJsonAsync(del);
            var basket = await RequestBuilder.Create()
                                             .AppendPathSegments("api", "basket", "basketOneGet") // добавляет к ендпоинт
                                             .SetQueryParam("orderOne", OrderDetail.OrderId)
                                             .GetJsonAsync<BasketList[]>(); //  https://192.168.0.4:5002/api/basket/basketOneGet

            BasketList = new ObservableCollection<BasketList>(basket);

            UpdatePrice();
        }

        //Удаление целой записи
        public async Task DeleteFromBasket(BasketList del)
        {
            var response = await RequestBuilder.Create()
                                               .AppendPathSegments("api", "basket", "basketOrderDell") // добавляет к ендпоинт
                                               .SetQueryParam("orderId", OrderDetail.OrderId)
                                               .PostJsonAsync(del);
            var ok = await response.Content.ReadAsStringAsync();
            Deleted = ok;

            var basket = await RequestBuilder.Create()
                                             .AppendPathSegments("api", "basket", "basketOneGet") // добавляет к ендпоинт
                                             .SetQueryParam("orderOne", OrderDetail.OrderId)
                                             .GetJsonAsync<BasketList[]>(); //  https://192.168.0.4:5002/api/basket/basketOneGet

            BasketList = new ObservableCollection<BasketList>(basket);
            UpdatePrice();

        }

        public void UpdatePrice()
        {
            //var data = new Order
            //{
            //    OrderId = OrderDetail.OrderId,
            //    TotalPrice = float.Parse(AllPrice),
            //};
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