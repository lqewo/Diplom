using Diplom.Common.Entities;
using Flurl.Http;
using Plugin.Connectivity;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MobileWorker.ViewModels
{
    // подключенная  PropertyChanged.Fody и файлик FodyWeavers.xml
    [AddINotifyPropertyChangedInterface]
    public class AdditionProductdViewModel
    {
        public ObservableCollection<AdditionMenu> AdditionList { get; set; }
        public Product Product { get; set; }
        public AdditionMenu[] AdditionMenus { get; set; }
        public bool IsBusy { get; set; } // последняя запись в БД или нет
        public bool IsRefreshing { get; set; } // иконка загрузки

        public ICommand RefreshCommand { get; set; }

        public AdditionProductdViewModel(Product product)
        {
            Product = product;

            if (!CrossConnectivity.Current.IsConnected)
            {
                return;
            }

            AdditionMenus = RequestBuilder.Create()
                                                .AppendPathSegments("api", "product", "productAdditionGet")
                                                .SetQueryParam("menuId", Product.ProductId)
                                                .GetJsonAsync<AdditionMenu[]>()
                                                .GetAwaiter().GetResult(); ; //  http://192.168.1.12:5002/api/menu/menuGet
            AdditionList = new ObservableCollection<AdditionMenu>(AdditionMenus);

            // Обновление списка
            RefreshCommand = new Command<AdditionMenu>(commandParameter =>
            {
                IsBusy = true;
                IsRefreshing = true;

                if (!CrossConnectivity.Current.IsConnected)
                {
                    return;
                }
                AdditionMenus = RequestBuilder.Create()
                                                .AppendPathSegments("api", "product", "productAdditionGet")
                                                .SetQueryParam("menuId", Product.ProductId)
                                                .GetJsonAsync<AdditionMenu[]>()
                                                .GetAwaiter().GetResult(); ; //  http://192.168.1.12:5002/api/menu/menuGet
                AdditionList = new ObservableCollection<AdditionMenu>(AdditionMenus);
                IsRefreshing = false;
            });
        }
        //Удаление целой записи
        public async Task DeleteAddition(AdditionMenu del)
        {
            _ = await RequestBuilder.Create()
                                    .AppendPathSegments("api", "product", "productAdditionDell") // добавляет к ендпоинт
                                    .PostJsonAsync(del);

            AdditionMenus = RequestBuilder.Create()
                                                .AppendPathSegments("api", "product", "productAdditionGet")
                                                .SetQueryParam("menuId", Product.ProductId)
                                                .GetJsonAsync<AdditionMenu[]>()
                                                .GetAwaiter().GetResult(); ; //  http://192.168.1.12:5002/api/menu/menuGet
            AdditionList = new ObservableCollection<AdditionMenu>(AdditionMenus);
        }
    }
}
