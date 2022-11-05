using MobileApp.Services;
using MobileApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewTransaction : ContentPage
    {
        private readonly IBillsService billsService;

        public NewTransaction()
        {
            InitializeComponent();
            billsService = ServiceContainer.Resolve<IBillsService>();

            this.BindingContext = new NewTransactionViewModel(billsService);
        }
    }
}