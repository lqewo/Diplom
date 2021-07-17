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
    public partial class BasketPage : ContentPage
    {
        private readonly BasketViewModel _basketViewModel;

        public BasketPage()
        {
            InitializeComponent();
            
            _basketViewModel = new BasketViewModel();
            BindingContext = _basketViewModel;
        }

        //public void OnMore(object sender, EventArgs e)
        //{
        //    var mi = ((MenuItem)sender);
        //    DisplayAlert("More Context Action", mi.CommandParameter.ToString() + " more context action", "OK");
        //}

        public async void OnDelete(object sender, EventArgs e)
        {
            var mi = (MenuItem) sender;
            var del = mi.CommandParameter as BasketList;
            await DisplayAlert("Delete Context Action", mi.CommandParameter + " delete context action", "OK");
            if(del != null)
            {
                await _basketViewModel.DeleteBasket(del);
                basketList.ItemsSource = _basketViewModel.BasketList;
            }
            else
            {
                await DisplayAlert("Ошибочка", "объект не выбран", "OK");
            }
        }

        private void BasketList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            //Binding BasketViewModel SelectedBasket;
            DisplayAlert("Delete Context Action", " тапед", "OK");
        }

        private void BasketList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            DisplayAlert("Delete Context Action", " сеелектед", "OK");
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            DisplayAlert("Delete Context Action", " началось удаление", "OK");
            using(var db = new ApplicationContext())
            {
                db.Basket.RemoveRange(db.Basket);
                db.BasketList.RemoveRange(db.BasketList);
                db.SaveChangesAsync();
                DisplayAlert("Delete Context Action", " удалилось из таблиц", "OK");
            }
        }

        //прибавление количества порций
        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            //var stringInThisCell = (string)((Button)sender).BindingContext;
            //myList.Remove(stringInThisCell);

            var mi = ((Button) sender).BindingContext;
            var del = mi as BasketList;
            if(del != null)
            {
                await _basketViewModel.AddQuantity(del);
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

        // уменьшение количества порций
        private async void Button_Clicked_2(object sender, EventArgs e)
        {
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
                await DisplayAlert("Ошибка", "объект не выбран", "OK");
            }
        }

        //создание заказа
        private async void Button_Clicked_3(object sender, EventArgs e)
        {
            // если нет подключение к интернету
            if(!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Ошибка", "Отсутствует подключение к интернету", "OK");
                return;
            }
            else if (!_basketViewModel.Access)
            {
                await DisplayAlert("Ошибка", "Корзина пуста", "OK");
                return;
            }
            else if (basketList.ItemsSource == null)
            {
                await DisplayAlert("Ошибка", "Корзина пуста", "OK");
                return;
            }

            await Navigation.PushAsync(new OrderPage(cena.Text));
        }

        //public void AllPrice(string AllPrice)
        //{
        //    cena.Text = AllPrice;
        //}

        //private void BasketList_ItemTapped(object sender, ItemTappedEventArgs e)
        //{

        //}

        //protected override async void OnAppearing()
        //{
        //    //using (var db = new ApplicationContext())
        //    //{
        //    //    var test = db.BasketList.ToList();
        //    //    basketList.ItemsSource = db.BasketList.ToList();
        //    //}

        //}

        //private async void Stepper_ValueChanged(object sender, ValueChangedEventArgs e)
        //{
        //    var znach = e.NewValue;
        //    await DisplayAlert("Выбранная модель", $"{znach} - ", "OK");
        //    //lable.Text = String.Format("Выбрано: {0:F1}", e.NewValue);
        //}

        //private async void BasketList_ItemTapped(object sender, ItemTappedEventArgs e)
        //{
        //    var selectedProduct = e.Item as BasketViewModel;
        //    await DisplayAlert("Выбранная модель", $"{selectedProduct} - ", "OK");

        //}

        //private async void Stepper_ValueChanged(object sender, ValueChangedEventArgs e)
        //{
        //    var znach = e.NewValue;
        //    await DisplayAlert("Выбранная модель", $"{znach} - ", "OK");
        //    var z = e.OldValue;
        //    await DisplayAlert("Выбранная модель", $"{z} - ", "OK");
        //    //lable.Text = String.Format("Выбрано: {0:F1}", e.NewValue);

        //}
        //вывод меню 
        //удаление из меню
        //смена кол-ва порций
        //оформление заказа
    }
}