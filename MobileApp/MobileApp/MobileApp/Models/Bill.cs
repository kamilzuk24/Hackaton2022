using System;
using System.Globalization;

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
        public string AmmountFormatted => string.Format(new CultureInfo("pl-PL"), "{0:N2} {1}", this.Amount, this.Currency);
        public string AccountFormatted => GetAccountNumberFormatted();

        public string Label => string.Format("{0} {1} {2}", this.Company, this.Email, this.AmmountFormatted);

        public bool Payed { get; set; }

        private string GetAccountNumberFormatted()
        {
            var account = this.Account;
            switch (account?.Length)
            {   //PL 89 9170 0005 2829 5924 2190 2290
                case 28:
                    return account.Substring(0, 2) + " " + account.Substring(2, 2) + " " + account.Substring(4, 4)
                        + " " + account.Substring(8, 4) + " " + account.Substring(12, 4) + " " + account.Substring(16, 4)
                        + " " + account.Substring(20, 4) + " " + account.Substring(24, 4);

                case 26:
                    return account.Substring(0, 2) + " " + account.Substring(2, 4) + " " + account.Substring(6, 4)
                        + " " + account.Substring(10, 4) + " " + account.Substring(14, 4) + " " + account.Substring(18, 4)
                        + " " + account.Substring(22, 4);

                default:
                    return "";
            }
        }
    }
}