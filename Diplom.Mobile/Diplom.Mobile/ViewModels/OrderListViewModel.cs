using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Diplom.Common.Entities;
using Diplom.Common.Models;
using Flurl.Http;
using Plugin.Connectivity;
using PropertyChanged;

namespace Diplom.Mobile.ViewModels
{
    // подключенная  PropertyChanged.Fody и файлик FodyWeavers.xml
    [AddINotifyPropertyChangedInterface]
    public class OrderListViewModel
    {
        public ObservableCollection<OrderList> OrderList { get; set; }
        public BasketList SelectedBasket { get; set; }

        public ConverterPaymentType TypePaymentt { get; set; }

        public OrderListViewModel()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                OrderList = new ObservableCollection<OrderList>();
                LoadMoreEmployerResult();
                return;
            }
            var basket = RequestBuilder.Create()
                                       .AppendPathSegments("api", "order", "orderGet") // добавляет к ендпоинт
                                       .GetJsonAsync<OrderList[]>().GetAwaiter().GetResult(); //  https://192.168.0.4:5002/api/order/orderGet

            //Payment(basket);
            //    Status(basket);

            SaveDb(basket).GetAwaiter().GetResult();

            OrderList = new ObservableCollection<OrderList>(basket);
        }
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

        //Удаление целой записи
        public async Task DeleteOrder(OrderList del)
        {
            _ = await RequestBuilder.Create()
                                    .AppendPathSegments("api", "order", "orderDell") // добавляет к ендпоинт
                                    .SetQueryParam("orderId", del.OrderId)
                                    .PostJsonAsync(null);
            var basket = await RequestBuilder.Create()
                                             .AppendPathSegments("api", "order", "orderGet") // добавляет к ендпоинт
                                             .GetJsonAsync<OrderList[]>(); //  https://192.168.0.4:5002/api/order/orderGet

            OrderList = new ObservableCollection<OrderList>(basket);
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