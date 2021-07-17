using System;
using Diplom.Common.Entities;
using Flurl.Http;
using MobileWorker.ViewModels;
using Plugin.Connectivity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileWorker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdditionProductsPage : ContentPage
    {
        public AdditionMenu[] AdditionMenus { get; set; }
        public Product Product { get; set; }
        private readonly AdditionProductdViewModel _additionProductdViewModel;

        public AdditionProductsPage(Product product)
        {
            InitializeComponent();

            _additionProductdViewModel = new AdditionProductdViewModel(product);
            BindingContext = _additionProductdViewModel;
            Product = product;
        }

        // выбор записи для изменения
        private void AdditionList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var selectedProduct = e.Item as AdditionMenu;
            if (selectedProduct == null)
            {
                return;
            }
            // вывод основной информации
            price.Text = selectedProduct.Price;
            gram.Text = selectedProduct.Grams;
            kalor.Text = selectedProduct.Calories;
        }

        // обновить запись
        private async void Button_Clicked(object sender, EventArgs e)
        {
            // Если нет подключения к интернету
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Внимание", "Отсутствует подключение к интернету", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(price.Text) || string.IsNullOrWhiteSpace(gram.Text) || string.IsNullOrWhiteSpace(kalor.Text))
            {
                await DisplayAlert("Внимание", "Заполнены не все поля", "OK");
                return;
            }
            var body = new AdditionMenu
            {
                Price = price.Text,
                Grams = gram.Text,
                Calories = kalor.Text,
                ProductId = Product.ProductId,
            };
            // отправление тела записи
            var response = await RequestBuilder.Create()
                                               .AppendPathSegments("api", "product", "productAdditionUpdate") // добавляет к ендпоинт
                                               .PostJsonAsync(body);
            if (!response.IsSuccessStatusCode)
            {
                await DisplayAlert("Ошибка", "Ошибка обновления записи", "OK");
                return;
            }
            await DisplayAlert("OK", "Запись успешно обновлена", "OK");
        }

        // удаление записи
        public async void OnDelete(object sender, EventArgs e)
        {
            // Если нет интернета
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Ошибка", "Отсутствует подключение к интернету", "OK");
                return;
            }
            var mi = (MenuItem)sender;
            var del = mi.CommandParameter as AdditionMenu;
            await DisplayAlert("Удалить", mi.CommandParameter + " Удаление записи", "OK");
            if (del != null)
            {
                await _additionProductdViewModel.DeleteAddition(del);
                additionList.ItemsSource = _additionProductdViewModel.AdditionList;
            }
            else
            {
                await DisplayAlert("Ошибка", "объект не выбран", "OK");
            }
        }

    }
}