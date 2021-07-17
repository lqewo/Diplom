using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Diplom.Common;
using MobileWorker.Models;
using MobileWorker.Views;

namespace MobileWorker.ViewModels
{
    public class DetailMenuViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<PageMenuItem> MenuItems { get; }

        public DetailMenuViewModel()
        {
            if (MySettings.Role == RoleNames.Worker)
            {
                MenuItems = new ObservableCollection<PageMenuItem>(new[]
                {
                    new PageMenuItem { Id = 0, Title = "Заказы", TargetType = typeof(OrderListPage)},
                    new PageMenuItem { Id = 0, Title = "Новости", TargetType = typeof(NewsPage) },
                    new PageMenuItem { Id = 0, Title = "Меню", TargetType = typeof(ProductsPage)},
                    new PageMenuItem { Id = 0, Title = "Аккаунт", TargetType = typeof(AccountInfoPage) },
                });
            }
            if (MySettings.Role == RoleNames.Director)
            {
                MenuItems = new ObservableCollection<PageMenuItem>(new[]
                {
                    new PageMenuItem { Id = 0, Title = "Заказы", TargetType = typeof(OrderListPage)},
                    new PageMenuItem { Id = 0, Title = "Новости", TargetType = typeof(NewsPage) },
                    new PageMenuItem { Id = 0, Title = "Меню", TargetType = typeof(ProductsPage)},
                    new PageMenuItem { Id = 0, Title = "Аккаунт", TargetType = typeof(AccountInfoPage) },
                    new PageMenuItem { Id = 0, Title = "Работники", TargetType = typeof(WorkerListPage) },
                    new PageMenuItem { Id = 0, Title = "Статистика", TargetType = typeof(StatisticsPage) },
                });
            }
            //MenuItems = new ObservableCollection<PageMenuItem>(new[]
            //{
            //    new PageMenuItem { Id = 0, Title = "Новости", TargetType = typeof(NewsPage) },
            //    new PageMenuItem { Id = 0, Title = "Отзывы", TargetType = typeof(ReviewsPage) },
            //    new PageMenuItem { Id = 0, Title = "Аккаунт", TargetType = typeof(AccountInfoPage) },
            //    new PageMenuItem { Id = 0, Title = "Меню", TargetType = typeof(ProductsPage)},
            //    new PageMenuItem { Id = 0, Title = "Корзина", TargetType = typeof(BasketPage)},
            //    new PageMenuItem { Id = 0, Title = "Мои заказы", TargetType = typeof(OrderListPage)},
            //    new PageMenuItem { Id = 0, Title = "Регистрация", TargetType = typeof(RegistrationPage)},
            //});
        }

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}