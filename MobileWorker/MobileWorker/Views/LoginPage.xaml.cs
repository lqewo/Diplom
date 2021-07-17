using System;
using Diplom.Common;
using Diplom.Common.Bodies;
using Diplom.Common.Models;
using Flurl.Http;
using MobileWorker.Views.DetailMenu;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileWorker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            loginEntry.Text = "";
            passwordEntry.Text = "";
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            // если нет подключение к интернету
            if(!CrossConnectivity.Current.IsConnected)
            {
                return;
            }
            
            var login = loginEntry.Text;
            var password = passwordEntry.Text;
            var body = new AuthBody
            {
                Login = loginEntry.Text,
                Password = passwordEntry.Text
            };

            if(string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Внимание", "Заполнены не все поля", "ОК");
                return;
            }

            var response = await RequestBuilder.Create()
                                               .AppendPathSegments("api", "account", "login") // добавляет к ендпоинт
                                               .PostJsonAsync(body); //  https://localhost:5001/api/account/login?login=1&password=1234567

            var result = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                await DisplayAlert("ошибка", result, "cancel");
                return;
            }

            var data = JsonConvert.DeserializeObject<AuthResponse>(result);
            if (data.Role == RoleNames.User)
            {
                await DisplayAlert("Внимание", "У вас нет доступа к данным функциям", "ОК");
                return;
            }
            MySettings.Token = data.AccessToken;
            MySettings.UserName = data.UserName;
            MySettings.Email = data.Email;
            MySettings.UserId = data.UserId;
            MySettings.Role = data.Role;

            if(MySettings.Role == RoleNames.User)
            {
                await DisplayAlert("Внимание", "У вас нет доступа к данным функциям", "ОК");
                MySettings.Clear();
            }
            else if(MySettings.Role == RoleNames.Worker)
            {
                await Navigation.PushAsync(new MasterDetailPage1());
            }
            else if(MySettings.Role == RoleNames.Director)
            {
                await Navigation.PushAsync(new MasterDetailPage1());
            }
        }

        private async  void Button_Clicked_1(object sender, EventArgs e)
        {
             //new NavigationPage(new MasterDetailPage1());
            await Navigation.PushAsync(new RegistrationPage());
        }

        //private async void Button_Clicked_2(object sender, EventArgs e)
        //{
        //    await DisplayAlert("a", MySettings.Role, "cancel");
        //}
    }
}