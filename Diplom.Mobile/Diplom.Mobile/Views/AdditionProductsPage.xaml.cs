using System;
using Diplom.Common.Entities;
using Flurl.Http;
using Plugin.Connectivity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diplom.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdditionProductsPage : ContentPage
    {
        public AdditionMenu[] AdditionMenus { get; set; }
        public Product Product { get; set; }

        public AdditionProductsPage(Product product)
        {
            InitializeComponent();
            Product = product;
        }

        protected override async void OnAppearing()
        {
            await DisplayAlert("Выбрано блюдо ", $"{Product.Name} - Хороший выбор ", "OK");

            //вывод основной информации
            image1.Source = ImageSource.FromUri(new Uri(Product.Img));
            nameProd.Text = Product.Name;
            LongDescription.Text = Product.LongDescription;

            if(!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Внимание", "Отсутствует подключение к интернету", "OK");
                base.OnAppearing();
                return;
            }

            AdditionMenus = await RequestBuilder.Create()
                                                .AppendPathSegments("api", "product", "productAdditionGet")
                                                .SetQueryParam("menuId", Product.ProductId)
                                                .GetJsonAsync<AdditionMenu[]>(); //  http://192.168.1.12:5002/api/menu/menuGet
            additionList.ItemsSource = AdditionMenus;
        }

        // добовление в локальнцюю БД корзины
        private async void AdditionList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var selectedProduct = e.Item as AdditionMenu;
            if(selectedProduct == null)
            {
                return;
            }

            await DisplayAlert("Выбранная блюдо ", $"Цена: {selectedProduct.Price}  рублей под номером {selectedProduct.AdditionMenuId}", "OK");
            var body = new Basket
            {
                UserId = MySettings.UserId,
                AdditionMenuId = selectedProduct.AdditionMenuId,
                Quantity = 1,
                OrderId = null,
            };
            var response = await RequestBuilder.Create()
                                               .AppendPathSegments("api", "basket", "basketAdd") // добавляет к ендпоинт
                                               .PostJsonAsync(body); //   https://192.168.1.12:5002/api/basket/basketAdd

            if(!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                await DisplayAlert("a", error, "cancel");
            }
            else
            {
                var ok = await response.Content.ReadAsStringAsync();
                await DisplayAlert("a", $"{ok} Запись добавлена в корзину", "закрыть");
            }


            //using (var db = new ApplicationContext())
            //{
            //    //проверяет есть ли уже эта запись в корзине 
            //    if (db.Basket.Any(x => x.AdditionMenuId == selectedProduct.AdditionMenuId))
            //    {
            //        await DisplayAlert("Выбранная блюдо", $"{selectedProduct.Price} - {selectedProduct.AdditionMenuId} уже добавлена в карзину", "OK");
            //    }

            //    //если такой записи в корзине нет то добавляем
            //    else
            //    {
            //        var data = new Basket
            //        {
            //            UserId = MySettings.UserId,
            //            AdditionMenuId = selectedProduct.AdditionMenuId,
            //            Quantity = 1,
            //            OrderId = -1,
            //        };
            //        await db.Basket.AddRangeAsync(data);
            //        await db.SaveChangesAsync();
            //        await DisplayAlert("Выбранная модель", $"{selectedProduct.Price} - добавлена в корзину", "OK");

            //        var basketId = db.Basket.FirstOrDefault(x => x.UserId == MySettings.UserId && x.AdditionMenuId == selectedProduct.AdditionMenuId).BasketId;
            //        var OverallPrice = Convert.ToInt32(selectedProduct.Price);
            //        var list = new BasketList
            //        {
            //            Price = selectedProduct.Price, // цена
            //            Name = Product.Name, //название
            //            ShortDescription = Product.ShortDescription, //описание
            //            Img = Product.Img, //картинка
            //            Quantity = 1, //количесство
            //            BasketId = basketId, //запись из корзины
            //            OverallPrice = OverallPrice //цена с учетом количества

            //        };
            //        await db.BasketList.AddRangeAsync(list);
            //        await db.SaveChangesAsync();
            //        await DisplayAlert("занесено", $"{list.BasketId} - добавлена в отображение", "OK");
            //    }
            //}
        }
    }
}