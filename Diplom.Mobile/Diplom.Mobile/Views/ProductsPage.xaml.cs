using System;
using System.Linq;
using Diplom.Common.Entities;
using Diplom.Common.Models;
using Diplom.Mobile.ViewModels;
using Flurl.Http;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diplom.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProductsPage : ContentPage
    {
        public Product[] Menuss { get; set; }
        private readonly ProductsViewModel _productsViewModel;


        public ProductsPage()
        {
            //productGet
            InitializeComponent();
            //picker.SelectedIndex = 0;
            var type = (MenuType)picker.SelectedIndex;
            _productsViewModel = new ProductsViewModel(type);
            BindingContext = _productsViewModel;
            //BindingContext = this;
        }

        //protected override async void OnAppearing()
        //{
        //    picker.SelectedIndex = 0;

        //    // если нет подключение к интернету
        //    if(!CrossConnectivity.Current.IsConnected)
        //    {
        //        InsertDataFromLocalDb();
        //        base.OnAppearing();
        //        return;
        //    }

        //    var response = await RequestBuilder.Create()
        //                                       .AppendPathSegments("api", "product", "productAllGet") // добавляет к ендпоинт
        //                                       .GetAsync(); //  http://192.168.1.12:5002/api/menu/menuGet

        //    var data = JsonConvert.DeserializeObject<Product[]>(await response.Content.ReadAsStringAsync());

        //    //если ошбка или пришла пустота берем данные из локальной БД
        //    if(!response.IsSuccessStatusCode || !data.Any())
        //    {
        //        InsertDataFromLocalDb();
        //        base.OnAppearing();
        //        return;
        //    }

        //    //занесение в локальную БД новых данных
        //    using(var db = new ApplicationContext())
        //    {
        //        db.Product.RemoveRange(db.Product);
        //        await db.SaveChangesAsync();

        //        await db.Product.AddRangeAsync(data);
        //        await db.SaveChangesAsync();

        //        menuList.ItemsSource = db.Product.ToList();
        //        InsertDataFromLocalDb();
        //    }

        //    //если все ок то данные из инета
        //    //menuList.ItemsSource = data;

        //    void InsertDataFromLocalDb()
        //    {
        //        using(var db = new ApplicationContext())
        //        {
        //            menuList.ItemsSource = db.Product.Where(x => x.Type == (MenuType) picker.SelectedIndex).ToList();
        //        }
        //    }

        //    //var selectedIndex = picker.SelectedIndex;

        //    //var type = MenuType.Food;
        //    //if (selectedIndex == 0)
        //    //{
        //    //    type = MenuType.Food;
        //    //}
        //    //else if (selectedIndex == 1)
        //    //{
        //    //    type = MenuType.Drink;
        //    //}

        //    //Menus = await RequestBuilder.Create()
        //    //                            .AppendPathSegments("api", "product", "productGet") // добавляет к ендпоинт
        //    //                            .SetQueryParam("type", type)
        //    //                            .GetJsonAsync<Product[]>(); //  http://192.168.1.12:5002/api/menu/menuGet
        //Menus = await RequestBuilder.Create()
        //                            .AppendPathSegments("api", "product", "productGet") // добавляет к ендпоинт
        //                            .SetQueryParam("type", type)
        //                            .GetJsonAsync<Product[]>(); //  http://192.168.1.12:5002/api/menu/menuGet
        //menuList.ItemsSource = Menus;
        //}

        private async void ProductList_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            var itemTypeObject = e.Item as Product;
            if (_productsViewModel.ProductList.Last() == itemTypeObject && _productsViewModel.ProductList.Count() != 1 && _productsViewModel.ProductList.Count() != 2)
            {
                if (_productsViewModel.IsBusy)
                {
                    if (!CrossConnectivity.Current.IsConnected)
                    {
                        // Показываем из локальной БД 
                        using (var db = new ApplicationContext())
                        {
                            _productsViewModel.LoadMoreEmployerResultInLockal((MenuType)picker.SelectedIndex);
                            if (_productsViewModel.Error) { await DisplayAlert("Ошибочка", "Нет записей", "OK"); }
                        }
                        return;
                    }
                    // Загружать из интернета
                    await _productsViewModel.LoadMoreEmployerResult((MenuType)picker.SelectedIndex);
                }
            }
        }

        public void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            _productsViewModel.Products((MenuType)picker.SelectedIndex);
            //using (var db = new ApplicationContext())
            //{
            //    //берем данные из локальной БД
            //    menuList.ItemsSource = db.Product.Where(x => x.Type == (MenuType) picker.SelectedIndex).ToList();
            //}
        }

        private async void MenuList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var selectedProduct = e.Item as Product;
            await Navigation.PushAsync(new AdditionProductsPage(selectedProduct));
        }
    }
}