using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Diplom.Mobile.Models;
using Diplom.Mobile.Views;

namespace Diplom.Mobile.ViewModels
{
    public class DetailMenuViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<PageMenuItem> MenuItems { get; }

        public DetailMenuViewModel()
        {
            MenuItems = new ObservableCollection<PageMenuItem>(new[]
            {
                new PageMenuItem { Id = 0, Title = "Новости", TargetType = typeof(NewsPage) },
                new PageMenuItem { Id = 0, Title = "Отзывы", TargetType = typeof(ReviewsPage) },
                new PageMenuItem { Id = 0, Title = "Аккаунт", TargetType = typeof(AccountInfoPage) },
                new PageMenuItem { Id = 0, Title = "Меню", TargetType = typeof(ProductsPage)},
                new PageMenuItem { Id = 0, Title = "Корзина", TargetType = typeof(BasketPage)},
                new PageMenuItem { Id = 0, Title = "Мои заказы", TargetType = typeof(OrderListPage)},
            });
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