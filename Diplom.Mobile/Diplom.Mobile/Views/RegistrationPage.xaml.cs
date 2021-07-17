using System;
using Diplom.Common;
using Diplom.Common.Bodies;
using Diplom.Common.Models;
using Diplom.Mobile.Views.DetailMenu;
using Flurl.Http;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.ComponentModel.DataAnnotations;

namespace Diplom.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegistrationPage : ContentPage
    {
        public RegistrationPage()
        {
            InitializeComponent();
            picker.SelectedIndex = 0;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            // если нет подключение к интернету
            if(!CrossConnectivity.Current.IsConnected)
            {
                return;
            }

            if(string.IsNullOrWhiteSpace(loginEntry.Text) ||
               string.IsNullOrWhiteSpace(passwordEntry.Text) ||
               string.IsNullOrWhiteSpace(emailEntry.Text) ||
               string.IsNullOrWhiteSpace(firstNameEntry.Text) ||
               string.IsNullOrWhiteSpace(lastNameEntry.Text) ||
               string.IsNullOrWhiteSpace(yearsEntry.Text))
            {
                await DisplayAlert("Ошибка", "Заполнены не все поля", "cancel");
                return;
            }
            if (!IsValidEmail(emailEntry.Text))
            {
                await DisplayAlert("Ошибка", "Не верный Email", "cancel");
                return;
            }
            var body = new RegisterBody
            {
                Login = loginEntry.Text,
                Password = passwordEntry.Text,
                Email = emailEntry.Text,
                FirstName = firstNameEntry.Text,
                LastName = lastNameEntry.Text,
                Year = Convert.ToInt32(yearsEntry.Text),
                PhoneNumber = telefonEntry.Text,
                Sex = (SexType)picker.SelectedIndex
            };

            if(!body.Email.Contains("@"))
            {
                await DisplayAlert("Ошибка", "Некоректный email", "cancel");
                return;
            }

            if(body.Year < 16 || body.Year > 150)
            {
                await DisplayAlert("Ошибка", "Некоректный возраст", "cancel");
                return;
            }

            if(body.Password.Length <= 6)
            {
                await DisplayAlert("Ошибка", "Длина пароля должна быть больше 6", "cancel");
                return;
            }

            var response = await RequestBuilder.Create()
                                               .AppendPathSegments("api", "account", "register") // добавляет к ендпоинт
                                               .PostJsonAsync(body); //  https://localhost:5001/api/account/login?login=1&password=1234567

            if(!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                await DisplayAlert("a", error, "cancel");
                return;
            }

            //сохранение данных пользователя
            var data = JsonConvert.DeserializeObject<AuthResponse>(await response.Content.ReadAsStringAsync());
            MySettings.Token = data.AccessToken;
            MySettings.UserName = data.UserName;
            MySettings.Email = data.Email;
            MySettings.UserId = data.UserId;
            MySettings.Role = data.Role;

            if(MySettings.Role == RoleNames.User)
            {
                await Navigation.PushAsync(new MasterDetailPage1());
            }

            //else if (MySettings.Role == "worker")
            //{
            //    await DisplayAlert("a", MySettings.Role, "cancel");
            //}
            //else if (MySettings.Role == "director")
            //{
            //    await DisplayAlert("a", MySettings.Role, "cancel");
            //}
        }
        public bool IsValidEmail(string source)
        {
            return new EmailAddressAttribute().IsValid(source);
        }
    }
}