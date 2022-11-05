using MobileApp.Services;
using MobileApp.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace MobileApp.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        private readonly IBillsService billsService;

        public ItemDetailPage()
        {
            InitializeComponent();
            billsService = ServiceContainer.Resolve<IBillsService>();

            BindingContext = new ItemDetailViewModel(billsService);
        }
    }
}