using MailScanner;
using System;
using System.Linq;
using WebApi.Database;

namespace WebApi.Services
{
    public class AtPayRecurringJob : IAtPayRecurringJob
    {
        private DatabaseContext _db { get; set; }
        private Guid testUserId = new Guid("1fa7367d-ba99-45be-9228-762c6230706b");

        public AtPayRecurringJob(DatabaseContext database) { 
            _db = database;
        }

        public void ProcessUnreadEmails()
        {
            var result = MailScannerClass.ScanAttachment(companyName: "UPC").Result;

            if (result.IsSuccess) {
                var ResultRecords = result.Bills.Select(q => new WebApi.Database.Models.Bill()
                {
                    Account = q.BillAccountNumber,
                    Amount = (decimal.TryParse(q.CashAmount,out var res) ? res : 0),
                    Currency = q.Currency,
                    Payed = false,
                    Title = q.PaymentName,
                    Company = "UPC",
                    UserId = testUserId
                }).ToList();

                _db.Bills.AddRange(ResultRecords);
                _db.SaveChanges();
            }            

            Console.WriteLine("");
        }
    }
}
