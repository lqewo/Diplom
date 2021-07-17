using System;
using System.Linq;
using Diplom.Common.Entities;
using MobileWorker.ViewModels;
using Plugin.Connectivity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileWorker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewsPage : ContentPage
    {
        public Content[] News { get; set; }

        private readonly NewsViewModel _newsViewModel;

        public NewsPage()
        {
            InitializeComponent();
            _newsViewModel = new NewsViewModel();
            BindingContext = _newsViewModel;
        }

        public void Refresh(object sender, EventArgs e)
        {
            newsList.IsRefreshing = true; //отображает иконку загрузки
            BindingContext = _newsViewModel; // здесь загружаете ваш контент снова у вас здесь может быть другой код.
            newsList.IsRefreshing = false; // выкл. иконку загрузки
        }

        private async void NewsList_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            var itemTypeObject = e.Item as Content;
            if(_newsViewModel.ContentList.Last() == itemTypeObject && _newsViewModel.ContentList.Count() != 1 && _newsViewModel.ContentList.Count() != 2)
            {
                if (_newsViewModel.IsBusy)
                {
                    if (!CrossConnectivity.Current.IsConnected)
                    {
                        // Показываем из локальной БД 
                        using (var db = new ApplicationContext())
                        {
                             _newsViewModel.LoadMoreEmployerResultInLockal();
                            if (_newsViewModel.Error) { await DisplayAlert("Ошибка", "Нет записей", "OK"); }
                        }
                        return;
                    }
                    // Загружать из интернета
                    await _newsViewModel.LoadMoreEmployerResult(itemTypeObject);
                }
            }
        }

        // удаление записи
        public async void OnDelete(object sender, EventArgs e)
        {
            // Если нет интернета
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Ошибка", "Отсутствует подключение к интернету", "OK");
                return;
            }
            var mi = (MenuItem)sender;
            var del = mi.CommandParameter as Content;
            await DisplayAlert("Новость удалена",  "Выбранная новость будет удалена", "OK");
            if (del != null)
            {
                await _newsViewModel.DeleteBasket(del);
                newsList.ItemsSource = _newsViewModel.ContentList;
            }
            else
            {
                await DisplayAlert("Ошибка", "Объект не выбран", "OK");
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            // Если нет интернета
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Ошибка", "Отсутствует подключение к интернету", "OK");
                return;
            }
            await Navigation.PushAsync(new NewsDetailPage());
        }

        private async void NewsList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var selectedContent = e.Item as Content;
            await Navigation.PushAsync(new NewsUpdatePage(selectedContent));
        }
    }
}