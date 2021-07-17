using System;
using Diplom.Common.Entities;
using Diplom.Common.Models;
using Diplom.Mobile.ViewModels;
using Plugin.Connectivity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diplom.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrderListPage : ContentPage
    {
        private readonly OrderListViewModel _orderListViewModel;

        public OrderListPage()
        {
            InitializeComponent();
            _orderListViewModel = new OrderListViewModel();
            BindingContext = _orderListViewModel;
        }

        public async void OnDelete(object sender, EventArgs e)
        {
            // Если нет подключения к интернету
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Внимание", "Отсутствует подключение к интернету", "OK");
                return;
            }
            var mi = (MenuItem) sender;
            var del = mi.CommandParameter as OrderList;
            if (del.Status == StatusType.Completed)
            {
                await DisplayAlert("Внимание", "Ваш заказ уже готов", "OK");
                return;
            }
            await DisplayAlert("Delete Context Action", mi.CommandParameter + " delete context action", "OK");
            if(del != null)
            {
                await _orderListViewModel.DeleteOrder(del);
            }
            else
            {
                await DisplayAlert("Ошибочка", "объект не выбран", "OK");
            }
        }

        //private async void BasketList_ItemTapped(object sender, ItemTappedEventArgs e)
        //{
        //    // Если нет подключения к интернету
        //    if (!CrossConnectivity.Current.IsConnected)
        //    {
        //        await DisplayAlert("Внимание", "Отсутствует подключение к интернету", "OK");
        //        return;
        //    }
        //    var mi = (MenuItem) sender;
        //    await DisplayAlert("Delete Context Action", $"{mi}", "OK");
        //    var del = mi.CommandParameter as OrderList;
        //    await DisplayAlert("Delete Context Action", mi.CommandParameter + " delete context action", "OK");
        //    if(del != null)
        //    {
        //        await _orderListViewModel.DeleteOrder(del);
        //    }
        //    else
        //    {
        //        await DisplayAlert("Ошибочка", "объект не выбран", "OK");
        //    }
        //}

        private async void Button_Clicked(object sender, EventArgs e)
        {
            // Если нет подключения к интернету
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Внимание", "Отсутствует подключение к интернету", "OK");
                return;
            }
            var mi = ((Button) sender).BindingContext;
            await DisplayAlert("Delete Context Action", $"{mi}", "OK");
            var del = mi as OrderList;

            await DisplayAlert("Delete Context Action", $"{del} -- {del.OrderListId} -- {del.OrderId}", "OK");

            // await DisplayAlert("Delete Context Action", mi.CommandParameter + " delete context action", "OK");
            if(del != null)
            {
                await Navigation.PushAsync(new OrderDetailPage(del));
            }
            else
            {
                await DisplayAlert("Ошибочка", "объект не выбран", "OK");
            }
        }
    }
}