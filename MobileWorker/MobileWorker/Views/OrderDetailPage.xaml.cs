using System;
using Diplom.Common.Entities;
using Diplom.Common.Models;
using Flurl.Http;
using Plugin.Connectivity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileWorker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrderDetailPage : ContentPage
    {
        public OrderList OrderDetail { get; set; } //выбраный заказ
        public OrderDetailWorker OrderGet { get; set; }

        public OrderDetailPage(OrderList del)
        {
            InitializeComponent();
            OrderDetail = del;
        }

        protected override async void OnAppearing()
        {
            // если нет подключение к интернету
            if(!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("ошибка", "Отсутствует подключение к интернету", "cancel");
                return;
            }

            var orderGet = await RequestBuilder.Create()
                                               .AppendPathSegments("api", "order", "orderWorkerOneGet") // добавляет к ендпоинт
                                               .SetQueryParam("orderOne", OrderDetail.OrderId)
                                               .GetJsonAsync<OrderDetailWorker>(); //  http://192.168.1.12:5002/api/order/orderWorkerOneGet
            OrderGet = orderGet;
            timePicker.Time = new TimeSpan(OrderGet.LeadTime.Hour, OrderGet.LeadTime.Minute, 0);
            datePicker.Date = OrderGet.LeadTime;
            reviewEntry.Text = OrderGet.Comment;
            picker.SelectedIndex = (int)OrderGet.TypePayment;

            prise.Text = OrderDetail.TotalPrice.ToString();
            email.Text = orderGet.Email;

            telefone.Text = orderGet.PhoneNumber;

            pickerStatus.SelectedIndex = (int)OrderGet.Status;
            
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            // если нет подключение к интернету
            if(!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("ошибка", "Отсутствует подключение к интернету", "cancel");
                return;
            }

           
            var orderId = OrderDetail.OrderId;
            var status = (StatusType)pickerStatus.SelectedIndex;

            var response = await RequestBuilder.Create()
                                               .AppendPathSegments("api", "order", "orderUpdateStatus") // добавляет к ендпоинт
                                               .SetQueryParam("orderId", orderId)
                                               .SetQueryParam("status", status)
                                               .PostJsonAsync(null);
            if(response.IsSuccessStatusCode)
            {
                await DisplayAlert("Статус заказа", "Статус успешно обновлен", "OK");
            }
            else
            {
                await DisplayAlert("Статус заказа", "Что то пошло не так при обновлении статуса", "OK");
            }
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            // если нет подключение к интернету
            if(!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("ошибка", "Отсутствует подключение к интернету", "cancel");
                return;
            }

            await Navigation.PushAsync(new OrderListDetailPage(OrderDetail));
        }
    }
}