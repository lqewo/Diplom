using System;
using System.Linq;
using Diplom.Common.Entities;
using Diplom.Common.Models;
using Diplom.Mobile.Views;
using Diplom.Mobile.Views.DetailMenu;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace Diplom.Mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            using(var db = new ApplicationContext())
            {
                // Создаем бд, если она отсутствует
                db.Database.EnsureCreated();
                if(!db.ReviewShow.Any())
                {
                    db.ReviewShow.Add(new ReviewShow
                    {
                         Text = "пывапьыждвьапждфвьапдфвпждапь", Rating = 4, Date = DateTime.Parse("11.11.2001"), FirstName = "sssss"
                    });
                    db.ReviewShow.Add(new ReviewShow
                    {
                        Text = "qweqweqweqweqweqweqweqweqweqwe", Rating = 5, Date = DateTime.Parse("09.09.2009"), FirstName = "rrrrr"
                    });
                }

                //try
                //{
                //    db.OrderList.Add(new OrderList
                //    {
                //        OrderTime = DateTime.Parse("09.09.2009"),
                //        LeadTime = DateTime.Parse("10.10.2010"),
                //        TotalPrice = 2256,
                //        Comment = "sdfdfhdfh dsfgsfgr dfg dfg",
                //        TypePayment = PaymentType.Cash,
                //        Status = StatusType.Processing,
                //    });
                //    db.OrderList.Add(new OrderList
                //    {
                //        OrderTime = DateTime.Parse("08.08.2008"),
                //        LeadTime = DateTime.Parse("08.08.2008"),
                //        TotalPrice = 2256,
                //        Comment = "sdfdsfsdffs sdffg",
                //        TypePayment = PaymentType.Cash,
                //        Status = StatusType.Processing,
                //    });
                //}
                //catch (Exception ex)
                //{
                //    var x = ex;
                //}


                if (!db.Product.Any())
                {
                    db.Product.Add(new Product
                    {
                        Name = "SQL SQL SQL",
                        ShortDescription = "SQL SQL SQL",
                        LongDescription = "sdfsdfdsfasdfsdfsdf",

                        //Img = "http://192.168.0.4:5002/images/kon.JPG",
                        Type = MenuType.FirstDish
                    });
                    db.Product.Add(new Product
                    {
                        Name = "SQL SQL SQL",
                        ShortDescription = "SQL SQL SQL",
                        LongDescription = "sdfsdfdsfasdfsdfsdf",

                        //Img = "http://192.168.0.4:5002/images/ping.jpg",
                        Type = MenuType.FirstDish
                    });
                }
                db.SaveChanges();
            }

            if(string.IsNullOrWhiteSpace(MySettings.Token))
            {
                MainPage = new NavigationPage(new LoginPage());
            }
            else
            {
                MainPage = new NavigationPage(new MasterDetailPage1());
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}