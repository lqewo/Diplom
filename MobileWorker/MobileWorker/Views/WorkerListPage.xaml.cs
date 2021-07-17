using Diplom.Common.Models;
using MobileWorker.ViewModels;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileWorker.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class WorkerListPage : ContentPage
	{
        private readonly WorkerListViewModel _workerListViewModel;
        public WorkerListPage ()
		{
			InitializeComponent ();
            _workerListViewModel = new WorkerListViewModel();
            BindingContext = _workerListViewModel;
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
            var del = mi.CommandParameter as WorkerList;
            await DisplayAlert("Delete Context Action", mi.CommandParameter + " delete context action", "OK");
            if (del != null)
            {
                await _workerListViewModel.DeleteWorker(del);
                workList.ItemsSource = _workerListViewModel.WorkerList;
            }
            else
            {
                await DisplayAlert("Ошибочка", "объект не выбран", "OK");
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
            await Navigation.PushAsync(new RegistrationPage());
        }
    }
}