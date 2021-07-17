using Diplom.Common.Models;
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
    public class WorkerListViewModel
    {
        public ObservableCollection<WorkerList> WorkerList { get; set; }
        public bool IsBusy { get; set; } // последняя запись в БД или нет
        public bool IsRefreshing { get; set; } // иконка загрузки

        public ICommand RefreshCommand { get; set; }

        public WorkerListViewModel()
        {
            //получение записей 
            var list = RequestBuilder.Create()
                                     .AppendPathSegments("api", "account", "workerGet") // добавляет к ендпоинт
                                     .GetJsonAsync<WorkerList[]>()
                                     .GetAwaiter().GetResult(); //  http://192.168.1.12:5002/api/account/workerGet
            if (list is null) { return; }
            WorkerList = new ObservableCollection<WorkerList>(list);

            // Обновление списка
            RefreshCommand = new Command<WorkerList>(commandParameter =>
            {
                IsBusy = true;
                IsRefreshing = true;

                if (!CrossConnectivity.Current.IsConnected)
                {
                    return;
                }
                //получение записей 
                var listt = RequestBuilder.Create()
                                         .AppendPathSegments("api", "account", "workerGet") // добавляет к ендпоинт
                                         .GetJsonAsync<WorkerList[]>()
                                         .GetAwaiter().GetResult(); //  http://192.168.1.12:5002/api/account/workerGet
                if (listt is null) { return; }
                WorkerList = new ObservableCollection<WorkerList>(listt);
                IsRefreshing = false;
            });
        }

        //Удаление целой записи
        public async Task DeleteWorker(WorkerList del)
        {
            _ = await RequestBuilder.Create()
                                    .AppendPathSegments("api", "account", "workerDell") // добавляет к ендпоинт
                                    .PostJsonAsync(del);

            var list = RequestBuilder.Create()
                                     .AppendPathSegments("api", "account", "workerGet") // добавляет к ендпоинт
                                     .GetJsonAsync<WorkerList[]>()
                                     .GetAwaiter().GetResult(); //  http://192.168.1.12:5002/api/account/workerGet
            if (list is null) { return; }
            WorkerList = new ObservableCollection<WorkerList>(list);
        }

    }
}
