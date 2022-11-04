using System;
using MobileApp.Services;
using MobileApp.ViewModels;
using Xamarin.Forms;

namespace MobileApp.Views
{
    public partial class AboutPage : ContentPage
    {
        private ItemsViewModel _viewModel;
        private readonly IBillsService billsService;

        public AboutPage()
        {
            InitializeComponent();
            billsService = ServiceContainer.Resolve<IBillsService>();

            BindingContext = _viewModel = new ItemsViewModel(new Guid("1fa7367d-ba99-45be-9228-762c6230706b"), billsService);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}