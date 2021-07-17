using Diplom.Common.Entities;
using Flurl.Http;
using Plugin.Connectivity;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileWorker.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProductUpdatePage : ContentPage
	{
        Product _product { get; set; }
        public Plugin.Media.Abstractions.MediaFile file { get; set; }
        public string nameImage { get; set; }

        public ProductUpdatePage (Product selectedProduct)
		{
			InitializeComponent ();
            _product = selectedProduct;

        }
        protected override async void OnAppearing()
        {
            // если нет подключение к интернету
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Ошибка", "Отсутствует подключение к интернету", "Ok");
                return;
            }
            name.Text = _product.Name;
            shortDiscription.Text = _product.ShortDescription;
            longDiscription.Text = _product.LongDescription;
        }

        // выбор нового изображения
        private async void Button_Clicked(object sender, EventArgs e)
        {
            // Если нет интернета
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Ошибка", "Отсутствует подключение к интернету", "OK");
                return;
            }
            await CrossMedia.Current.Initialize();
            // проверка возможности выбора изображений
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("Ошибка", ":( Разрешение на выбор изображени отсутствует.", "OK");
                return;
            }
            // выбор изображения
            file = await CrossMedia.Current.PickPhotoAsync();
            await DisplayAlert("ок", file.Path, "OK");
            if (file.Path != null)
            {
                text.Text = "Изображение выбрано и готово к загрузке";
            }
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AdditionProductsPage(_product));
        }

        private async void Button_Clicked_2(object sender, EventArgs e)
        {
            // Если нет интернета
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Ошибка", "Отсутствует подключение к интернету", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(name.Text) || string.IsNullOrWhiteSpace(shortDiscription.Text) || string.IsNullOrWhiteSpace(longDiscription.Text))
            {
                await DisplayAlert("Ошибка", "Заполнены не все поля", "OK");
            }
            if (text.Text == "Изображение выбрано и готово к загрузке")
            {
                // отправление изображения
                var imageupdate = await RequestBuilder.Create()
                                        .AppendPathSegments("api", "content", "contentUpload") // добавляет к ендпоинт
                                        .PostMultipartAsync(x =>
                                        {
                                            x.AddFile("file1", file.Path);
                                        });
                if (!imageupdate.IsSuccessStatusCode)
                {
                    var resultt = await imageupdate.Content.ReadAsStringAsync();
                    await DisplayAlert("Ошибка", "Размер файла слишком большой. Максимальный размер 2Мб", "OK");
                    return;
                }
                var nameimg = await imageupdate.Content.ReadAsStringAsync();
                nameImage = nameimg;

                // составление тела записи
                var bodyImg = new Product
                {
                    ProductId = _product.ProductId,
                    ShortDescription = shortDiscription.Text,
                    LongDescription = longDiscription.Text,
                    Name = name.Text,
                    Img = nameImage,
                };
                // отправление тела записи
                var responseImg = await RequestBuilder.Create()
                                                   .AppendPathSegments("api", "product", "productUpdateImg") // добавляет к ендпоинт
                                                   .PostJsonAsync(bodyImg);
                if (!responseImg.IsSuccessStatusCode)
                {
                    await DisplayAlert("Ошибка", "Ошибка обновления записи", "OK");
                    return;
                }
                await DisplayAlert("Ок", "Запись успешно обновлена.", "OK");
                return;
            }

            // составление тела записи
            var body = new Product
            {
                ProductId = _product.ProductId,
                ShortDescription = shortDiscription.Text,
                LongDescription = longDiscription.Text,
                Name = name.Text,
            };
            // отправление тела записи
            var response = await RequestBuilder.Create()
                                               .AppendPathSegments("api", "product", "productUpdate") // добавляет к ендпоинт
                                               .PostJsonAsync(body);
            if (!response.IsSuccessStatusCode)
            {
                await DisplayAlert("Ошибка", "Ошибка обновления записи", "OK");
                return;
            }
            await DisplayAlert("Ок", "Запись успешно обновлена.", "OK");
        }
    }
}