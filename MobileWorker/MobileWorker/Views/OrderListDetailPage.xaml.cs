using System;
using System.Linq;
using Diplom.Common.Models;
using MobileWorker.ViewModels;
using Plugin.Connectivity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileWorker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrderListDetailPage : ContentPage
    {
        public OrderList OrderDetail { get; set; }
        private readonly OrderListDetailViewModel _basketViewModel;

        public OrderListDetailPage(OrderList dell)
        {
            InitializeComponent();
            OrderDetail = dell;

            _basketViewModel = new OrderListDetailViewModel(OrderDetail);
            BindingContext = _basketViewModel;
        }

    }
}