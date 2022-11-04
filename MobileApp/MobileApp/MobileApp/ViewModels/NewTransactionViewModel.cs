using MobileApp.Models;
using System.Diagnostics;
using System;
using MobileApp.Services;
using Xamarin.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace MobileApp.ViewModels
{
    [QueryProperty(nameof(Id), nameof(Id))]
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
        private string text;
        private string description;
        private bool notPayed;

        public string Id
        {
            get => id;
            set => SetProperty(ref id, value);
        }

        public string Text
        {
            get => text;
            set => SetProperty(ref text, value);
        }

        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
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
                Id = item.Id.ToString();
                Text = item.Label;
                NotPayed = !item.Payed;
                Description = item.AccountFormatted;
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
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        private async void OnPay()
        {
            Item newItem = new Item()
            {
                Id = Guid.NewGuid().ToString(),
                Text = Text,
                Description = Description
            };

            await this.billsService.PayBill(new Guid(this.ItemId));

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }
    }
}