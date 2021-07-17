using Diplom.Common.Entities;
using Flurl.Http;
using Plugin.Connectivity;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileWorker.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NewsDetailPage : ContentPage
	{
        public Plugin.Media.Abstractions.MediaFile file { get; set; }
        public string nameImage { get; set; }

        public NewsDetailPage ()
		{
			InitializeComponent ();
            text.Text = "Изображение не выбрано";
        }

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
                if(text.Text == "Изображение не выбрано")
                {
                    await DisplayAlert("Ошибка", "Изображение не выбрано", "OK");
                    return;
                }
                await DisplayAlert("Ошибка", "Заполнены не все поля", "OK");
            }
            // отправление изображения
            var image =  await RequestBuilder.Create()
                                    .AppendPathSegments("api", "content", "contentUpload") // добавляет к ендпоинт
                                    .PostMultipartAsync(x =>
                                    {
                                        x.AddFile("file1", file.Path);
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

            // составление тела записи
            var body = new Content
            {
                Name = name.Text,
                Img = nameImage,
                Text = textEntry.Text,
                Date = DateTime.Now,
            };
            // отправление тела записи
            var response = await RequestBuilder.Create()
                                               .AppendPathSegments("api", "content", "contentAdd") // добавляет к ендпоинт
                                               .PostJsonAsync(body);
            if (!response.IsSuccessStatusCode)
            {
                await DisplayAlert("Ошибка", "Ошибка добавления записи", "OK");
                return;
            }
            await DisplayAlert("Ок", "Запись успешно добавлена.", "OK");

        }

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
            if(file.Path != null)
            {
                text.Text = "Изображение выбрано и готово к загрузке";
                //await RequestBuilder.Create()
                //                    .AppendPathSegments("api", "content", "contentUpload") // добавляет к ендпоинт
                //                    .PostMultipartAsync(x =>
                //                    {
                //                        x.AddFile("file1", file.Path);
                //                    });
            }
           
        }
    }
}