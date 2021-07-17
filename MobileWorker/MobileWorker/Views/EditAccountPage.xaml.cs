using System;
using Diplom.Common.Models;
using Flurl.Http;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileWorker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditAccountPage : ContentPage
    {
        public EditAccountPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            var userResponse = await RequestBuilder.Create()
                                                   .AppendPathSegments("api", "account", "userGet") // добавляет к ендпоинт
                                                   .GetJsonAsync<UserResponse>(); //  https://192.168.1.12:5002/api/account/userGet

            loginEntry.Placeholder = userResponse.Login;
            firstNameEntry.Placeholder = userResponse.FirstName;
            lastNameEntry.Placeholder = userResponse.LastName;
            yearsEntry.Text = userResponse.Year.ToString();
            emailEntry.Text = userResponse.Email;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var body = new UserResponse
            {
                Login = loginEntry.Text,
                Email = emailEntry.Text,
                FirstName = firstNameEntry.Text,
                LastName = lastNameEntry.Text,
                Year = Convert.ToInt32(yearsEntry.Text),
            };

            var response = await RequestBuilder.Create()
                                               .AppendPathSegments("api", "account", "userEdit") // добавляет к ендпоинт
                                               .PostJsonAsync(body); //  https://192.168.1.12:5002/api/account/userEdit

            if(!response.IsSuccessStatusCode)
            {
                await DisplayAlert("ошибка", "Что то пошло не так при обновлении", "cancel");
                return;
            }

            //TODO: шо это такое?
            var data = JsonConvert.DeserializeObject<UserResponse>(await response.Content.ReadAsStringAsync());
        }
    }
}