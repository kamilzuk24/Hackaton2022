using MobileApp.Models;
using System.Diagnostics;
using System;
using MobileApp.Services;
using Xamarin.Forms;
using static System.Net.Mime.MediaTypeNames;
using MobileApp.Views;

namespace MobileApp.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class NewTransactionViewModel : BaseViewModel
    {
        private IBillsService billsService;

        public NewTransactionViewModel(IBillsService billsService)
        {
            this.billsService = billsService;

            PayCommand = new Command(OnPay);
            CancelCommand = new Command(OnCancel);
            this.PropertyChanged +=
                (_, __) => PayCommand.ChangeCanExecute();
        }

        private string id;
        private string itemId;
        private bool notPayed;

        private string email;
        private string company;
        private string account;
        private string title;
        private decimal amount;
        private string currency;
        private string statusOfPayment;
        public string PaymentDate => "05.11.2022";

        public string StatusOfPayment
        {
            get => statusOfPayment;
            set => SetProperty(ref statusOfPayment, value);
        }

        public string TitleBand => NotPayed ? "Zapłać rachunek" : "Sczegóły rachunku";
        public string AmmountFormatted => string.Format("{0:N2} {1}", this.Amount, this.Currency);
        public string AccountFormatted => this.Account;

        public string Id
        {
            get => id;
            set => SetProperty(ref id, value);
        }

        public string Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }

        public string Company
        {
            get => company;
            set => SetProperty(ref company, value);
        }

        public string Account
        {
            get => account;
            set => SetProperty(ref account, value);
        }

        public string TitleFor
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        public decimal Amount
        {
            get => amount;
            set => SetProperty(ref amount, value);
        }

        public string Currency
        {
            get => currency;
            set => SetProperty(ref currency, value);
        }

        public bool NotPayed
        {
            get => notPayed;
            set => SetProperty(ref notPayed, value);
        }

        public string ItemId
        {
            get
            {
                return itemId;
            }
            set
            {
                itemId = value;
                LoadItemId(value);
            }
        }

        public async void LoadItemId(string itemId)
        {
            try
            {
                var item = await this.billsService.GetBill(new Guid(itemId));
                item.Account = item.Account?.Trim()?.Replace(" ", "");
                Id = item.Id.ToString();
                NotPayed = !item.Payed;
                Email = item.Email;
                Company = item.Company;
                Account = item.AccountFormatted;
                TitleFor = item.Title;
                Amount = item.Amount;
                Currency = item.Currency;
                StatusOfPayment = item.Payed ? "Zapłacono" : "Nie opłacone";
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }

        public Command PayCommand { get; }
        public Command CancelCommand { get; }

        private async void OnCancel()
        {
            await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
        }

        private async void OnPay()
        {
            await this.billsService.PayBill(new Guid(this.Id));

            //await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
            await Shell.Current.GoToAsync($"{nameof(ConfirmPayPage)}");
        }
    }
}