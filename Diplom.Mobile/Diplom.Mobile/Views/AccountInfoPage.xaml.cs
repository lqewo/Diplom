using System;
using System.ComponentModel.DataAnnotations;
using Diplom.Common.Models;
using Flurl.Http;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diplom.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountInfoPage : ContentPage
    {
        public AccountInfoPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            // если нет подключение к интернету
            if(!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("ошибка", "Отсутствует подключение к интернету", "cancel");
                return;
            }

            var userResponse = await RequestBuilder.Create()
                                                    .AppendPathSegments("api", "account", "userGet") // добавляет к ендпоинт
                                                    .GetJsonAsync<UserResponse>(); //  https://192.168.1.12:5002/api/account/userGet
            
            loginEntry.Text = userResponse.Login;
            firstNameEntry.Text = userResponse.FirstName;
            lastNameEntry.Text = userResponse.LastName;
            yearsEntry.Text = userResponse.Year.ToString();
            emailEntry.Text = userResponse.Email;
            telefonEntry.Text = userResponse.PhoneNumber;
            picker.SelectedIndex = (int)userResponse.Sex;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            // если нет подключение к интернету
            if(!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("ошибка", "Отсутствует подключение к интернету", "cancel");
                return;
            }

            if (string.IsNullOrWhiteSpace(loginEntry.Text) ||
              string.IsNullOrWhiteSpace(emailEntry.Text) ||
              string.IsNullOrWhiteSpace(firstNameEntry.Text) ||
              string.IsNullOrWhiteSpace(lastNameEntry.Text) ||
              string.IsNullOrWhiteSpace(yearsEntry.Text) ||
              string.IsNullOrWhiteSpace(telefonEntry.Text))
            {
                await DisplayAlert("Ошибка", "Заполнены не все поля", "cancel");
                return;
            }

            if (!IsValidEmail(emailEntry.Text))
            {
                await DisplayAlert("Ошибка", "Не верный Email", "cancel");
                return;
            }
            var body = new UserResponse
            {
                Login = loginEntry.Text,
                Email = emailEntry.Text,
                FirstName = firstNameEntry.Text,
                LastName = lastNameEntry.Text,
                Year = Convert.ToInt32(yearsEntry.Text),
                PhoneNumber = telefonEntry.Text,
                Sex = (SexType)picker.SelectedIndex
            };

            var response = await RequestBuilder.Create()
                                               .AppendPathSegments("api", "account", "userEdit") // добавляет к ендпоинт
                                               .PostJsonAsync(body); //  https://192.168.1.12:5002/api/account/userEdit

            if(!response.IsSuccessStatusCode)
            {
                await DisplayAlert("ошибка", "Что-то пошло не так при обновлении", "cancel");
                return;
            }

            //заносим данные в модель
            var data = JsonConvert.DeserializeObject<UserResponse>(await response.Content.ReadAsStringAsync());

            //обновляем данные на странице
            loginEntry.Text = data.Login;
            firstNameEntry.Text = data.FirstName;
            lastNameEntry.Text = data.LastName;
            yearsEntry.Text = data.Year.ToString();
            emailEntry.Text = data.Email;
            telefonEntry.Text = data.PhoneNumber;
            picker.SelectedIndex = (int)data.Sex;
            
            await DisplayAlert("ОК", "Данные успешно обновлены", "cancel");
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            // если нет подключение к интернету
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("ошибка", "Отсутствует подключение к интернету", "cancel");
                return;
            }
            await Navigation.PushAsync(new ChangePasswordPage());
        }
        public bool IsValidEmail(string source)
        {
            return new EmailAddressAttribute().IsValid(source);
        }
    }
}