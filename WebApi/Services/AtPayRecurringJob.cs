using MailScanner;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebApi.Database;
using Newtonsoft.Json;
using System.Collections.Generic;
using Models;

namespace WebApi.Services;
public class AtPayRecurringJob : IAtPayRecurringJob
{
    private DatabaseContext _db { get; set; }
    private Guid testUserId = new Guid("1fa7367d-ba99-45be-9228-762c6230706b");
    static readonly HttpClient client = new HttpClient();

    public AtPayRecurringJob(DatabaseContext database)
    {
        _db = database;
    }

    public async Task ProcessUnreadEmails()
    {
        HttpResponseMessage response = await client.GetAsync("https://emailreaderapi20221105085845.azurewebsites.net/emails");
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();

        var emails = JsonConvert.DeserializeObject<List<Email>>(responseBody);

        foreach (var mail in emails)
        {
            string senderFromSubject = string.Empty;

            if (mail.Subject.Contains("UPC", StringComparison.InvariantCultureIgnoreCase))
            {
                senderFromSubject = "UPC";
            }
            if (mail.Subject.Contains("PGE", StringComparison.InvariantCultureIgnoreCase))
            {
                senderFromSubject = "PGE";
            }
            if (mail.Subject.Contains("TOYA", StringComparison.InvariantCultureIgnoreCase))
            {
                senderFromSubject = "TOYA";
            }
            if (mail.Subject.Contains("Play", StringComparison.InvariantCultureIgnoreCase))
            {
                senderFromSubject = "Play";
            }

            var result = await MailScannerClass.ScanAttachment(companyName: senderFromSubject);

            if (result.IsSuccess)
            {
                var ResultRecords = result.Bills.Select(q => new Database.Models.Bill()
                {
                    Account = q.BillAccountNumber,
                    Amount = (decimal.TryParse(q.CashAmount, out var res) ? res : 0),
                    Currency = q.Currency,
                    Payed = false,
                    Title = q.PaymentName,
                    Company = senderFromSubject,
                    UserId = testUserId,
                    Email = mail.From
                }).ToList();

                _db.Bills.AddRange(ResultRecords);
            }
        }

        _db.SaveChanges();

        Console.WriteLine("");
    }
}
