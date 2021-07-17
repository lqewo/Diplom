using System;
using System.ComponentModel.DataAnnotations;
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
            if (body.Year < 16 || body.Year > 150)
            {
                await DisplayAlert("Ошибка", "Некоректный возраст", "cancel");
                return;
            }
            if (body.Password.Length <= 6)
            {
                await DisplayAlert("Ошибка", "Длина пароля должна быть больше 6", "cancel");
                return;
            }

            var response = await RequestBuilder.Create()
                                               .AppendPathSegments("api", "account", "registerWork") // добавляет к ендпоинт
                                               .SetQueryParam("rol", pickerRol.SelectedIndex)
                                               .PostJsonAsync(body); //  https://localhost:5001/api/account/login?login=1&password=1234567

            if(!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                await DisplayAlert("a", error, "cancel");
                return;
            }
            await DisplayAlert("ОК", "Работник успешно зарегестрирован", "ОК");
        }
        public bool IsValidEmail(string source)
        {
            return new EmailAddressAttribute().IsValid(source);
        }
    }
}