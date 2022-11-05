using System;

namespace MobileApp.Models
{
    public class Bill
    {
        public Guid Id { get; set; }
        public string Account { get; set; }
        public string Email { get; set; }
        public string Company { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Title { get; set; }
        public string AmmountFormatted => string.Format("{0:N2} {1}", this.Amount, this.Currency);
        public string AccountFormatted => this.Account;

        public string Label => string.Format("{0} {1} {2}", this.Company, this.Email, this.AmmountFormatted);

        public bool Payed { get; set; }
    }
}