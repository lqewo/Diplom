using System;
using System.Linq;
using Diplom.Common.Entities;
using Diplom.Common.Models;
using Diplom.Mobile.ViewModels;
using Plugin.Connectivity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diplom.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrderListDetailPage : ContentPage
    {
        public OrderList OrderDetail { get; set; }
        private readonly OrderListDetailViewModel _basketViewModel;

        public OrderListDetailPage(OrderList dell)
        {
            InitializeComponent();
            OrderDetail = dell;

            _basketViewModel = new OrderListDetailViewModel(OrderDetail);
            BindingContext = _basketViewModel;
        }

        //удаление продукта
        public async void OnDelete(object sender, EventArgs e)
        {
            // Если нет подключения к интернету
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Внимание", "Отсутствует подключение к интернету", "OK");
                return;
            }
            //проверяем готов ли заказа
            if (OrderDetail.Status == StatusType.Completed)
            {
                await DisplayAlert("Внимание", "Ваш заказ уже готов", "OK");
                return;
            }

            var mi = (MenuItem) sender;
            var del = mi.CommandParameter as BasketList;
            await DisplayAlert("Delete Context Action", mi.CommandParameter + " delete context action", "OK");

            if(del == null)
            {
                await DisplayAlert("Ошибочка", "объект не выбран", "OK");
                return;
            }

            await _basketViewModel.DeleteFromBasket(del);
            basketList.ItemsSource = _basketViewModel.BasketList;

            if(string.IsNullOrEmpty(_basketViewModel.Deleted))
            {
                return;
            }

            await DisplayAlert("Ошибочка", _basketViewModel.Deleted, "OK");
            await Navigation.PushAsync(new NewsPage());
        }

        //прибавление количества порций
        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            // Если нет подключения к интернету
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Внимание", "Отсутствует подключение к интернету", "OK");
                return;
            }
            //проверяем готов ли заказа
            if (OrderDetail.Status == StatusType.Completed)
            {
                await DisplayAlert("Внимание", "Ваш заказ уже готов", "OK");
                return;
            }

            var mi = ((Button) sender).BindingContext;
            var del = mi as BasketList;
            if(del != null)
            {
                await _basketViewModel.AddQuantity(del);
                await DisplayAlert("del", $"{del.Quantity}", "OK");
                var asd = _basketViewModel.BasketList.FirstOrDefault(x => x.BasketListId == del.BasketListId).Quantity;
                await DisplayAlert("Ошибочка", asd.ToString(), "OK");

                basketList.ItemsSource = _basketViewModel.BasketList;
            }
            else
            {
                await DisplayAlert("Ошибочка", "объект не выбран", "OK");
            }
        }

        // уменьшение количества порций
        private async void Button_Clicked_2(object sender, EventArgs e)
        {
            // Если нет подключения к интернету
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Внимание", "Отсутствует подключение к интернету", "OK");
                return;
            }
            //проверяем готов ли заказа
            if (OrderDetail.Status == StatusType.Completed)
            {
                await DisplayAlert("Внимание", "Ваш заказ уже готов", "OK");
                return;
            }

            var mi = ((Button) sender).BindingContext;
            var del = mi as BasketList;
            if(del != null)
            {
                await _basketViewModel.LowerQuantity(del);
                await DisplayAlert("del", $"{del.Quantity}", "OK");
                var asd = _basketViewModel.BasketList.FirstOrDefault(x => x.BasketListId == del.BasketListId).Quantity;
                await DisplayAlert("Ошибочка", $"{asd}", "OK");

                basketList.ItemsSource = _basketViewModel.BasketList;
            }
            else
            {
                await DisplayAlert("Ошибочка", "объект не выбран", "OK");
            }
        }
    }
}