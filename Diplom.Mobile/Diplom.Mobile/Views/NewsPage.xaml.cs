using System;
using System.Linq;
using System.Threading.Tasks;
using Diplom.Common.Entities;
using Diplom.Mobile.ViewModels;
using Flurl.Http;
using Plugin.Connectivity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diplom.Mobile.Views
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

        //protected override async void OnAppearing()
        //{
            //News = await RequestBuilder.Create()
            //                                .AppendPathSegments("api", "content", "contentGet") // добавляет к ендпоинт
            //                                .GetJsonAsync<Content[]>(); //  http://192.168.1.12:5002/api/content/contentGet

            //var sortList = News.OrderByDescending(x => x.ContentId).ToList();
            //newsList.ItemsSource = sortList;
        //}

        //private void Button_Clicked(object sender, EventArgs e)
        //{
        //    //отсортировать по новизне добавления
        //    var sortList = News.OrderByDescending(x => x.ContentId);
        //    newsList.ItemsSource = sortList;
        //}
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
                            if (_newsViewModel.Error) { await DisplayAlert("Ошибочка", "Нет записей", "OK"); }
                        }
                        return;
                    }
                    // Загружать из интернета
                    await _newsViewModel.LoadMoreEmployerResult(itemTypeObject);
                }
            }
        }
    }
}