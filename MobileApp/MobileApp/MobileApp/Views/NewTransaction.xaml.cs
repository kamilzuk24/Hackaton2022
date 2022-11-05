using System.Diagnostics;
using System;
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
        private NewTransactionViewModel viewModel;

        public NewTransaction()
        {
            InitializeComponent();
            billsService = ServiceContainer.Resolve<IBillsService>();

            this.BindingContext = viewModel = new NewTransactionViewModel(billsService);
        }

        private async void OnDecision(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("", "Czy napewno zapłacić rachunek", "Zapłać", "Anuluj");
            if (answer)
            {
                await this.billsService.PayBill(new Guid(viewModel.Id));

                //await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
                await Shell.Current.GoToAsync($"{nameof(ConfirmPayPage)}");
            }
        }
    }
}