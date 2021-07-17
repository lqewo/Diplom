using System;
using System.Linq;
using Diplom.Common.Entities;
using Diplom.Common.Models;
using Diplom.Mobile.ViewModels;
using Flurl.Http;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diplom.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReviewsPage : ContentPage
    {
        private readonly ReviewsViewModel _reviewsViewModel;
        public ReviewsPage()
        {
            InitializeComponent();
            _reviewsViewModel = new ReviewsViewModel();
            BindingContext = _reviewsViewModel;
        }

        //protected override async void OnAppearing()
        //{
        //    // если нет подключение к интернету
        //    if(!CrossConnectivity.Current.IsConnected)
        //    {
        //        InsertDataFromLocalDb();
        //        base.OnAppearing();
        //        return;
        //    }

        //    var reviews = await RequestBuilder.Create()
        //                                      .AppendPathSegments("api", "review", "reviewGet") // добавляет к ендпоинт
        //                                      .GetAsync(); //  https://192.168.1.12:5002/api/review/reviewGet

        //    var data = JsonConvert.DeserializeObject<ReviewShow[]>(await reviews.Content.ReadAsStringAsync());

        //    //если ошбка или пришла пустота берем данные из локальной БД
        //    if(!reviews.IsSuccessStatusCode || data is null)
        //    {
        //        InsertDataFromLocalDb();

        //        base.OnAppearing();
        //        return;
        //    }

        //    //занесение в локальную БД новых данных
        //    using(var db = new ApplicationContext())
        //    {
        //        db.ReviewShow.RemoveRange(db.ReviewShow);
        //        await db.ReviewShow.AddRangeAsync(data);
        //        await db.SaveChangesAsync();
        //    }

        //    //reviewList.ItemsSource = data;
        //    using(var db = new ApplicationContext())
        //    {
        //        reviewList.ItemsSource = db.ReviewShow.ToList();
        //    }

        //    void InsertDataFromLocalDb()
        //    {
        //        using(var db = new ApplicationContext())
        //        {
        //            reviewList.ItemsSource = db.ReviewShow.ToList();
        //        }
        //    }
        //}

        private async void ReviewList_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            var itemTypeObject = e.Item as ReviewShow;
            if (_reviewsViewModel.ReviewsList.Last() == itemTypeObject && _reviewsViewModel.ReviewsList.Count() != 1 && _reviewsViewModel.ReviewsList.Count() != 2)
            {
                if (_reviewsViewModel.IsBusy)
                {
                    if (!CrossConnectivity.Current.IsConnected)
                    {
                        // Показываем из локальной БД 
                        using (var db = new ApplicationContext())
                        {
                            _reviewsViewModel.LoadMoreEmployerResultInLockal();
                            if (_reviewsViewModel.Error) { await DisplayAlert("Ошибка", "Нет записей", "OK"); }
                        }
                        return;
                    }
                    // Загружать из интернета
                    await _reviewsViewModel.LoadMoreEmployerResult();
                }
            }
        }

        // отправление отзыва
        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Ошибка", "Отсутствует подключение к интернету", "ок");
            }
            var body = new Review
            {
                Text = reviewEntry.Text,
                Rating = picker.SelectedIndex + 1, //Convert To int 32 если делать со слайдером
                Date = DateTime.Now,
            };
            var text = reviewEntry.Text;

            if(string.IsNullOrEmpty(text))
            {
                await DisplayAlert("Ошибка", "Заполнены не все поля", "ок");
                return;
            }

            var response = await RequestBuilder.Create()
                                               .AppendPathSegments("api", "review", "reviewAdd") // добавляет к ендпоинт
                                               .PostJsonAsync(body);

            if(response.IsSuccessStatusCode)
            {
                await DisplayAlert("ОК", "Отзыв добавлен", "ок");
                OnAppearing();
            }
            else
            {
                await DisplayAlert("Ошибка", "Ошибка при добавлении отзыва", "ок");
            }
        }

        public async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            var selectedReview = e.Item as ReviewShow;
            if(selectedReview != null)
            {
                await DisplayAlert("Выбранная модель", $"{selectedReview.Rating} - {selectedReview.Date} - {selectedReview.FirstName}", "OK");
            }
        }
    }
}