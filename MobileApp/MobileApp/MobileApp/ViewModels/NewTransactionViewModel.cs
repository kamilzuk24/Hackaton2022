using Xamarin.Forms;

namespace MobileApp.ViewModels
{
    [QueryProperty(nameof(Id), nameof(Id))]
    public class NewTransactionViewModel : BaseViewModel
    {
        private string id;

        public string Id
        {
            get => id;
            set => SetProperty(ref id, value);
        }
    }
}