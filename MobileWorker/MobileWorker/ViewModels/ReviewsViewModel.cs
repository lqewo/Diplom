using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Diplom.Common.Models;
using Flurl.Http;
using Plugin.Connectivity;
using PropertyChanged;
using Xamarin.Forms;

namespace MobileWorker.ViewModels
{
    // подключенная  PropertyChanged.Fody и файлик FodyWeavers.xml
    [AddINotifyPropertyChangedInterface]
    public class ReviewsViewModel
    {
        public ObservableCollection<ReviewShow> ReviewsList { get; set; }
        public bool IsBusy { get; set; } // последняя запись в БД или нет
        public int Skip { get; set; } // сколько записей пропустить при запросе
        public int SkipStage { get; set; } // кол-во загруженых записей
        public bool IsRefreshing { get; set; } // иконка загрузки
        public bool Error { get; set; } //ошибка

        public ICommand RefreshCommand { get; set; }

        public ReviewsViewModel()
        {
            IsBusy = true;
            IsRefreshing = false;
            SkipStage = 0;
            Skip = SkipStage * 5;

            // Если нет интернета
            if (!CrossConnectivity.Current.IsConnected)
            {
                ReviewsList = new ObservableCollection<ReviewShow>();
                //показываем из локальной БД 
                LoadMoreEmployerResultInLockal();
                return;
            }

            //получение записей 
            var review = RequestBuilder.Create()
                                     .AppendPathSegments("api", "review", "reviewTake") // добавляет к ендпоинт
                                     .SetQueryParam("skip", Skip)
                                     .GetJsonAsync<ReviewShow[]>()
                                     .GetAwaiter().GetResult(); //  http://192.168.1.12:5002/api/review/reviewTake
            if (!review.Any()) { return; }

            // Занесение в локальную БД
            SaveDb(review).GetAwaiter().GetResult();

            ReviewsList = new ObservableCollection<ReviewShow>(review);
            SkipStage++;

            // Команда обновления списка
            RefreshCommand = new Command<ReviewShow>(commandParameter =>
            {
                IsBusy = true;
                IsRefreshing = true;
                SkipStage = 0;
                Skip = SkipStage * 5;

                // Если нет интернета
                if (!CrossConnectivity.Current.IsConnected)
                {
                    //показываем из локальной БД 
                    using (var db = new ApplicationContext())
                    {
                        LoadMoreEmployerResultInLockal();
                    }
                    return;
                }
                var reviews = RequestBuilder.Create()
                                         .AppendPathSegments("api", "review", "reviewTake") // добавляет к ендпоинт
                                         .SetQueryParam("skip", Skip)
                                         .GetJsonAsync<ReviewShow[]>()
                                         .GetAwaiter().GetResult(); //  http://192.168.1.12:5002/api/review/reviewTake

                ReviewsList = new ObservableCollection<ReviewShow>(reviews);
                IsRefreshing = false;
            });
        }

        // Динамическая подгрузка данных при прокручивании списка
        public async Task LoadMoreEmployerResult()
        {
            IsRefreshing = true;
            Skip = 5 * SkipStage;

            //получение записей 
            var review = await RequestBuilder.Create()
                                           .AppendPathSegments("api", "review", "reviewTake") // добавляет к ендпоинт
                                           .SetQueryParam("skip", Skip)
                                           .GetJsonAsync<ReviewShow[]>(); //  http://192.168.1.12:5002/api/review/reviewTake
            if (!review.Any())
            {
                IsBusy = false;
                IsRefreshing = false;
                return;
            }

            SkipStage++;
            var xxx = ReviewsList;
            foreach (var x in review)
            {
                xxx.Add(x);
            }
            //заносим в локальную БД
            using (var db = new ApplicationContext())
            {
                db.ReviewShow.RemoveRange(db.ReviewShow);
                await db.SaveChangesAsync();

                await db.ReviewShow.AddRangeAsync(xxx);
                await db.SaveChangesAsync();
            }

            ReviewsList = new ObservableCollection<ReviewShow>(xxx);
            IsRefreshing = false;
        }

        // Динамическая загрузка данных из локальной БД при отсутствии интернета
        public void LoadMoreEmployerResultInLockal()
        {
            Error = false;
            IsRefreshing = true;
            Skip = 5 * SkipStage;

            // берем из локальной БД
            using (var db = new ApplicationContext())
            {
                const int take = 5;
                var reviews = db.ReviewShow.ToArray();
                if (!reviews.Any())
                {
                    Error = true;
                    IsRefreshing = false;
                    return;
                }
                var sortList = reviews.OrderByDescending(x => x.Date).ToList();
                var tske = sortList.Skip(Skip).Take(take);
                if (!tske.Any())
                {
                    IsBusy = false;
                }
                SkipStage++;
                var xxx = ReviewsList;
                foreach (var x in tske)
                {
                    xxx.Add(x);
                }
                ReviewsList = new ObservableCollection<ReviewShow>(xxx);
                Error = false;
                IsRefreshing = false;
            }
        }
        public async Task SaveDb(ReviewShow[] data)
        {
            //заносим в локальную БД
            using (var db = new ApplicationContext())
            {
                db.ReviewShow.RemoveRange(db.ReviewShow);
                await db.SaveChangesAsync();

                await db.ReviewShow.AddRangeAsync(data);
                await db.SaveChangesAsync();
            }
        }
    }
}
