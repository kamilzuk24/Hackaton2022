using MobileApp.Models;
using MobileApp.Services;
using MobileApp.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MobileApp.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        private Bill _selectedItem;

        public ObservableCollection<Bill> Items { get; }
        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command<Bill> ItemTapped { get; }

        public ItemsViewModel(Guid userId, IBillsService billsService)
        {
            Title = "Browse";
            Items = new ObservableCollection<Bill>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand(userId, billsService));

            ItemTapped = new Command<Bill>(OnItemSelected);

            AddItemCommand = new Command(OnAddItem);
        }

        private async Task ExecuteLoadItemsCommand(Guid userId, IBillsService billsService)
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await billsService.GetPayedBills(userId);
                foreach (var item in items)
                { 
                    item.Account = item.Account?.Trim()?.Replace(" ", "");
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedItem = null;
        }

        public Bill SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnItemSelected(value);
            }
        }

        private async void OnAddItem(object obj)
        {
            await Shell.Current.GoToAsync(nameof(NewItemPage));
        }

        private async void OnItemSelected(Bill item)
        {
            if (item == null)
                return;

            // This will push the ItemDetailPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(NewTransaction)}?{nameof(NewTransactionViewModel.ItemId)}={item.Id}");
            //await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.Id}");
        }
    }
}