using Diplom.Common.Entities;
using Flurl.Http;
using Plugin.Connectivity;
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
	public partial class ProductAdditionAddPage : ContentPage
	{
        public Plugin.Media.Abstractions.MediaFile _fileAdd { get; set; } // изображение продукта
        public Product _body { get; set; } 
        public bool add { get; set; } // разрешение добавления нового продукта
        public string nameImage { get; set; }
        public int productId { get; set; } // ИД добавленого продукта

        public ProductAdditionAddPage (Product body, Plugin.Media.Abstractions.MediaFile file)
		{
			InitializeComponent ();
            _body = body;
            add = true;
            _fileAdd = file;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            // Если нет интернета
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Ошибка", "Отсутствует подключение к интернету", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(price.Text) || string.IsNullOrWhiteSpace(kalor.Text) || string.IsNullOrWhiteSpace(gram.Text))
            {
                await DisplayAlert("Ошибка", "Заполнены не все поля", "OK");
                return;
            }
            if (!add)
            {
                await DisplayAlert("Ошибка", "Данный продукт уже добавлен", "OK");
                return;
            }

            // отправление изображения
            var image = await RequestBuilder.Create()
                                    .AppendPathSegments("api", "content", "contentUpload") // добавляет к ендпоинт
                                    .PostMultipartAsync(x =>
                                    {
                                        x.AddFile("file1", _fileAdd.Path);
                                    });
            if (!image.IsSuccessStatusCode)
            {
                var resultt = await image.Content.ReadAsStringAsync();
                await DisplayAlert("Ошибка", resultt, "OK");
                await DisplayAlert("Ошибка", "Размер файла слишком большой. Максимальный размер 2Мб", "OK");
                return;
            }
            var result = await image.Content.ReadAsStringAsync();
            nameImage = result;

            var body = new Product
            {
                Name = _body.Name,
                ShortDescription = _body.ShortDescription,
                LongDescription = _body.LongDescription,
                Img = nameImage,
                Type = _body.Type,
            };
            // отправление тела записи
            var response = await RequestBuilder.Create()
                                               .AppendPathSegments("api", "product", "productAdd") // добавляет к ендпоинт
                                               .PostJsonAsync(body);
            if (!response.IsSuccessStatusCode)
            {
                await DisplayAlert("Ошибка", "Ошибка добавления записи", "OK");
                return;
            }
            productId = Convert.ToInt32(await response.Content.ReadAsStringAsync());

            var addition = new AdditionMenu
            {
                Price = price.Text,
                Calories = kalor.Text,
                Grams = gram.Text,
                ProductId = productId,
            };

            // добавление расширеной записи
            var aditionMenu = await RequestBuilder.Create()
                                               .AppendPathSegments("api", "product", "productAdditionAdd") // добавляет к ендпоинт
                                               .PostJsonAsync(addition);
            if (!aditionMenu.IsSuccessStatusCode)
            {
                await DisplayAlert("Ошибка", "Ошибка добавления расширеной записи", "OK");
            }

            await DisplayAlert("Ок", "Запись успешно добавлена", "OK");
            add = false;
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            // Если нет интернета
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Ошибка", "Отсутствует подключение к интернету", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(price.Text) || string.IsNullOrWhiteSpace(kalor.Text) || string.IsNullOrWhiteSpace(gram.Text))
            {
                await DisplayAlert("Ошибка", "Заполнены не все поля", "OK");
                return;
            }
            if (add)
            {
                await DisplayAlert("Ошибка", "Сначала нужно добавить продукт", "OK");
                return;
            }
            if (productId.ToString() == null)
            {
                await DisplayAlert("Ошибка", "Сначала нужно добавить продукт", "OK");
                return;
            }
            var addition = new AdditionMenu
            {
                Price = price.Text,
                Calories = kalor.Text,
                Grams = gram.Text,
                ProductId = productId,
            };
            // добавление расширеной записи
            var response = await RequestBuilder.Create()
                                               .AppendPathSegments("api", "product", "productAdditionAdd") // добавляет к ендпоинт
                                               .PostJsonAsync(addition);
            if (!response.IsSuccessStatusCode)
            {
                await DisplayAlert("Ошибка", "Ошибка добавления расширеной записи", "OK");
            }

            await DisplayAlert("Ок", "Расширенная запись успешно добавлена", "OK");
        }
    }
}