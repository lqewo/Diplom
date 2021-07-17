using System;
using System.Linq;
using Diplom.Common.Models;
using MobileWorker.ViewModels;
using Plugin.Connectivity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileWorker.Views
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
            if (_orderListViewModel.Error)
            {
                DisplayAlert("Внимание", "Отсутствует подключение к интернету", "OK");
            }
        }

        // прокрутка и подгрузка списка
        private async void NewsList_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            var itemTypeObject = e.Item as OrderList;
            if (_orderListViewModel.OrderList.Last() == itemTypeObject && _orderListViewModel.OrderList.Count() != 1 && _orderListViewModel.OrderList.Count() != 2)
            {
                if (_orderListViewModel.IsBusy)
                {
                    if (!CrossConnectivity.Current.IsConnected)
                    {
                        // Показываем из локальной БД 
                        using (var db = new ApplicationContext())
                        {
                            // _orderListViewModel.LoadMoreEmployerResultInLockal();
                            await DisplayAlert("Внимание", "Отсутствует подключение к интернету", "OK");
                        }
                        return;
                    }
                    // Загружать из интернета
                    await _orderListViewModel.LoadMoreEmployerResult(itemTypeObject);
                }
            }
        }


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