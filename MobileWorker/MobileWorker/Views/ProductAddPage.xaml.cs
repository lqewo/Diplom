using Diplom.Common.Entities;
using Diplom.Common.Models;
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
	public partial class ProductAddPage : ContentPage
	{
        public Plugin.Media.Abstractions.MediaFile file { get; set; }
        public string nameImage { get; set; }

        public ProductAddPage ()
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
            // Если нет интернета
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Ошибка", "Отсутствует подключение к интернету", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(name.Text) || string.IsNullOrWhiteSpace(shortDiscription.Text) || string.IsNullOrWhiteSpace(longDiscription.Text) || picker.SelectedIndex == -1)
            {
                await DisplayAlert("Ошибка", "Заполнены не все поля.", "OK");
                return;
            }
            if(file == null)
            {
                await DisplayAlert("Ошибка", "Изображение не выбрано.", "OK");
                return;
            }
            var body = new Product
            {
                Name = name.Text,
                ShortDescription = shortDiscription.Text,
                LongDescription = longDiscription.Text,
                Type = (MenuType)picker.SelectedIndex,
            };
            await Navigation.PushAsync(new ProductAdditionAddPage(body, file));
        }
    }
}