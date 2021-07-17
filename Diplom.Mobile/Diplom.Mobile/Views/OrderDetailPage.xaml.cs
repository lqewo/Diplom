using System;
using Diplom.Common.Entities;
using Diplom.Common.Models;
using Flurl.Http;
using Plugin.Connectivity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diplom.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrderDetailPage : ContentPage
    {
        public OrderList OrderDetail { get; set; } //выбраный заказ
        public Order OrderGet { get; set; }

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
                                               .AppendPathSegments("api", "order", "orderOneGet") // добавляет к ендпоинт
                                               .SetQueryParam("orderOne", OrderDetail.OrderId)
                                               .GetJsonAsync<Order>(); //  http://192.168.1.12:5002/api/order/orderOneGet
            OrderGet = orderGet;
            timePicker.Time = new TimeSpan(OrderGet.LeadTime.Hour, OrderGet.LeadTime.Minute, 0);
            datePicker.Date = OrderGet.LeadTime;
            reviewEntry.Text = OrderGet.Comment;
            picker.SelectedIndex = (int)OrderGet.TypePayment;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            // если нет подключение к интернету
            if(!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("ошибка", "Отсутствует подключение к интернету", "cancel");
                return;
            }

            //проверяем готов ли заказа
            if(OrderGet.Status == StatusType.Completed)
            {
                await DisplayAlert("Внимание", "Ваш заказ уже готов", "OK");
                return;
            }

            if(datePicker.Date.DayOfWeek != DayOfWeek.Sunday)
            {
                await DisplayAlert("дата", "хорошая дата", "OK");
            }
            else
            {
                await DisplayAlert("дата", "Неверная дата", "OK");
                datePicker.Date = DateTime.Now;
                return;
            }

            // DisplayAlert("время", $"{timePicker.Time} выбрано", "OK");
            var selectedTime = timePicker.Time; // выбранное время
            var date1 = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0); //текущее время

            //DisplayAlert("время", $"{date1} текушее время", "OK");
            var maxTime = new TimeSpan(18, 00, 00);
            var minTime = new TimeSpan(08, 00, 00);

            if(datePicker.Date == DateTime.Now.Date)
            {
                if(selectedTime > date1 && selectedTime < maxTime && selectedTime > minTime)
                {
                    await DisplayAlert("время", "выбранное время находится в нужном диапазоне", "OK");
                }
                else
                {
                    await DisplayAlert("Внимание", "Не верное время", "OK");
                    timePicker.Time = new TimeSpan(DateTime.Now.AddHours(1).Hour, DateTime.Now.Minute, 0);
                    return;
                }
            }
            else
            {
                if(selectedTime < maxTime && selectedTime > minTime)
                {
                    await DisplayAlert("время", "выбранное время находится в нужном диапазоне", "OK");
                }
                else
                {
                    await DisplayAlert("Внимание", "Не верное время", "OK");
                    timePicker.Time = new TimeSpan(DateTime.Now.AddHours(1).Hour, DateTime.Now.Minute, 0);
                    return;
                }
            }

            var data = new Order
            {
                OrderId = OrderGet.OrderId,
                LeadTime = datePicker.Date + timePicker.Time,
                Comment = reviewEntry.Text,
                TypePayment = (PaymentType)picker.SelectedIndex,
                Status = StatusType.Processing,
            };

            var response = await RequestBuilder.Create()
                                               .AppendPathSegments("api", "order", "orderUpdate") // добавляет к ендпоинт
                                               .PostJsonAsync(data);
            if(response.IsSuccessStatusCode)
            {
                await DisplayAlert("Заказ", "Данные успешно обновлены", "OK");
            }
            else
            {
                await DisplayAlert("время", "Что то пошло не так при обновлении заказа", "OK");
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