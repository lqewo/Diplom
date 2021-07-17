using Diplom.Common.Models;
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
    public partial class StatisticsPage : ContentPage
    {
        public StatisticsPage()
        {
            InitializeComponent();
        }
        protected override async void OnAppearing()
        {
            // если нет подключение к интернету
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("ошибка", "Отсутствует подключение к интернету", "cancel");
                return;
            }

            var userResponse = await RequestBuilder.Create()
                                                    .AppendPathSegments("api", "content", "statistiksGet") // добавляет к ендпоинт
                                                    .GetJsonAsync<Statistiks>(); //  https://192.168.1.12:5002/api/content/statistiksGet
            populerEatMount.Text = $"Самое популярное блюдо за месяц: {userResponse.populerEatMount}";
            populerEatYear.Text = $"Самое популярное блюдо за год: {userResponse.populerEatYear}";
            revenueMount.Text = $"Доход за месяц: {userResponse.revenueMount.ToString()}Руб.";
            revenueYear.Text = $"Дохо за год: {userResponse.revenueYear}Руб.";
            averageCheck.Text = $"Средний чек заказа: {userResponse.averageCheck.ToString()}Руб.";
            averageYear.Text = $"Средний возраст покупателей: {userResponse.averageYear.ToString()} Лет";

            countSexFemale.Text = $"Кол-во покупателей за месяц женского пола: {userResponse.countSexFemale.ToString()} Шт.";
            countSexMale.Text = $"Кол-во покупателей за месяц мужского пола: {userResponse.countSexMale.ToString()} Шт.";

            userCount.Text = $"Общее количество пользователей: {userResponse.userCount.ToString()}";

        }
	}
}