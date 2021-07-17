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
	public partial class NewsUpdatePage : ContentPage
	{
        Content _content { get; set; }
        public Plugin.Media.Abstractions.MediaFile file { get; set; }
        public string nameImage { get; set; }

        public NewsUpdatePage (Content selectedContent)
		{
			InitializeComponent ();
            _content = selectedContent;
		}
        protected override async void OnAppearing()
        {
            // если нет подключение к интернету
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Ошибка", "Отсутствует подключение к интернету", "Ok");
                return;
            }
            name.Text = _content.Text;
            textEntry.Text = _content.Text;
        }

        // выбор нового изображения
        private async void Button_Clicked_1(object sender, EventArgs e)
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

        // обновление записи
        private async void Button_Clicked(object sender, EventArgs e)
        {
            // Если нет интернета
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Ошибка", "Отсутствует подключение к интернету", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(name.Text) || string.IsNullOrWhiteSpace(textEntry.Text))
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
                var bodyImg = new Content
                {
                    ContentId = _content.ContentId,
                    Name = name.Text,
                    Img = nameImage,
                    Text = textEntry.Text,
                };
                // отправление тела записи
                var responseImg = await RequestBuilder.Create()
                                                   .AppendPathSegments("api", "content", "contentUpdateImg") // добавляет к ендпоинт
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
            var body = new Content
            {
                ContentId = _content.ContentId,
                Name = name.Text,
                Text = textEntry.Text,
            };
            // отправление тела записи
            var response = await RequestBuilder.Create()
                                               .AppendPathSegments("api", "content", "contentUpdate") // добавляет к ендпоинт
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