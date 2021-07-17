using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Diplom.Common.Entities;
using Flurl.Http;
using Plugin.Connectivity;
using PropertyChanged;
using Xamarin.Forms;

namespace Diplom.Mobile.ViewModels
{
    // подключенная  PropertyChanged.Fody и файлик FodyWeavers.xml
    [AddINotifyPropertyChangedInterface]
    public class NewsViewModel
    {
        public ObservableCollection<Content> ContentList { get; set; }
        public bool IsBusy { get; set; } // последняя запись в БД или нет
        public int Skip { get; set; } // сколько записей пропустить при запросе
        public int SkipStage { get; set; } // кол-во загруженых записей
        public bool IsRefreshing { get; set; } // иконка загрузки
        public bool Error { get; set; } //ошибка

        public ICommand RefreshCommand { get; set; }

        public NewsViewModel()
        {
            IsBusy = true;
            IsRefreshing = false;
            SkipStage = 0;
            Skip = SkipStage * 5;

            if (!CrossConnectivity.Current.IsConnected)
            {
                //показываем из локальной БД 
                using (var db = new ApplicationContext())
                {
                    ContentList = new ObservableCollection<Content>();
                    LoadMoreEmployerResultInLockal();
                }
                return;
            }

            //получение записей 
            var news = RequestBuilder.Create()
                                     .AppendPathSegments("api", "content", "contentTake") // добавляет к ендпоинт
                                     .SetQueryParam("skip", Skip)
                                     .GetJsonAsync<Content[]>()
                                     .GetAwaiter().GetResult(); //  http://192.168.0.4:5002/api/content/contentTake
            if(news is null) { return; }
            //var sortList = news.OrderByDescending(x => x.ContentId).ToList();
            ContentList = new ObservableCollection<Content>(news);
            SkipStage++;

            // Обновление списка
            RefreshCommand = new Command<Content>(commandParameter =>
            {
                IsBusy = true;
                IsRefreshing = true;
                SkipStage = 0;
                Skip = SkipStage * 5;
                if (!CrossConnectivity.Current.IsConnected)
                {
                    //показываем из локальной БД 
                    using (var db = new ApplicationContext())
                    {
                        //ContentList = new ObservableCollection<Content>();
                        LoadMoreEmployerResultInLockal();
                    }
                    return;
                }
                var newss = RequestBuilder.Create()
                                         .AppendPathSegments("api", "content", "contentTake") // добавляет к ендпоинт
                                         .SetQueryParam("skip", Skip)
                                         .GetJsonAsync<Content[]>()
                                         .GetAwaiter().GetResult(); //  http://192.168.0.4:5002/api/content/contentTake
                if (newss is null) { return; }
                ContentList = new ObservableCollection<Content>(newss);
                IsRefreshing = false;
            });
        }

        public async Task LoadMoreEmployerResult(Content itemTypeObject)
        {
            IsRefreshing = true;
            Skip = 5 * SkipStage;

            //получение записей 
            var news = await RequestBuilder.Create()
                                           .AppendPathSegments("api", "content", "contentTake") // добавляет к ендпоинт
                                           .SetQueryParam("skip", Skip)
                                           .GetJsonAsync<Content[]>(); //  http://192.168.0.4:5002/api/content/contentTake
            if(!news.Any())
            {
                IsBusy = false;
                IsRefreshing = false;
                return;
            }
            
            SkipStage++;
            var xxx = ContentList;
            foreach(var x in news)
            {
                xxx.Add(x);
            }

            //заносим в локальную БД
            using (var db = new ApplicationContext())
            {
                db.Content.RemoveRange(db.Content);
                await db.SaveChangesAsync();

                await db.Content.AddRangeAsync(xxx);
                await db.SaveChangesAsync();
            }
            
            ContentList = new ObservableCollection<Content>(xxx);
            IsRefreshing = false;

            //var sortList = xxx.OrderByDescending(x => x.ContentId).ToList();
            //var sortList = news.OrderByDescending(x => x.ContentId).ToList();
            //ContentList = new ObservableCollection<Content>(sortList);
        }
        public void LoadMoreEmployerResultInLockal()
        {
            Error = false;
            IsRefreshing = true;
            Skip = 5 * SkipStage;

            // берем из локальной БД
            using (var db = new ApplicationContext())
            {
                const int take = 5;
                var contents = db.Content.ToArray();
                if (!contents.Any())
                {
                    Error = true;
                    IsRefreshing = false;
                    return;
                }
                var sortList = contents.OrderByDescending(x => x.Date).ToList();
                var tske = sortList.Skip(Skip).Take(take);
                if (!tske.Any())
                {
                    IsBusy = false;
                }
                SkipStage++;
                var xxx = ContentList;
                foreach (var x in tske)
                {
                    xxx.Add(x);
                }
                ContentList = new ObservableCollection<Content>(xxx);
                Error = false;
                IsRefreshing = false;
            }
        }
    }
}