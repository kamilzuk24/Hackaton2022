using MobileApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewTransaction : ContentPage
    {
        public NewTransaction()
        {
            InitializeComponent();
            this.BindingContext = new NewTransactionViewModel();
        }
    }
}