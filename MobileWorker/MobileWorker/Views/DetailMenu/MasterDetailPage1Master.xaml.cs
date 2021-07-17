using MobileWorker.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileWorker.Views.DetailMenu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterDetailPage1Master : ContentPage
    {
        public ListView ListView;

        public MasterDetailPage1Master()
        {
            InitializeComponent();

            BindingContext = new DetailMenuViewModel();
            ListView = MenuItemsListView;
        }

        private  void Button_Clicked(object sender, System.EventArgs e)
        {
            MySettings.Clear();
            //new NavigationPage(new LoginPage());
            //Navigation.PushModalAsync(new LoginPage());
            Navigation.PopToRootAsync();
        }
    }
}