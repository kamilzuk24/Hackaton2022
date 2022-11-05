using MailScanner;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebApi.Database;
using Newtonsoft.Json;
using System.Collections.Generic;
using Models;
using System.IO;
using System.Reflection;

namespace WebApi.Services;

public class AtPayRecurringJob : IAtPayRecurringJob
{
    private DatabaseContext _db { get; set; }
    private readonly INotificationService _notificationService;
    private Guid testUserId = new Guid("1fa7367d-ba99-45be-9228-762c6230706b");
    private static readonly HttpClient client = new HttpClient();

    public AtPayRecurringJob(DatabaseContext database, INotificationService notificationService)
    {
        _db = database;
        _notificationService = notificationService;
    }

    public async Task ProcessUnreadEmails()
    {
        string executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        

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

            foreach (var attachment in mail.Attachments)
            {
                var client = new System.Net.WebClient();
                var responseBytes = client.DownloadData(
                    attachment.Url);

                var pathToFile = Path.Combine(executableLocation, attachment.Name);
                System.IO.File.WriteAllBytes(pathToFile, responseBytes);

                var result = await MailScannerClass.ScanAttachment(
                    filePath: pathToFile, 
                    companyName: senderFromSubject);
                System.IO.File.Delete(pathToFile);

                if (result.IsSuccess)
                {
                    var ResultRecords = result.Bills.Select(q => new Database.Models.Bill()
                    {
                        Account = q.BillAccountNumber,
                        Amount = (decimal.TryParse(q.CashAmount, out var res) ? res : 25),
                        Currency = q.Currency,
                        Payed = false,
                        Title = q.PaymentName,
                        Company = senderFromSubject,
                        UserId = testUserId,
                        Email = mail.From
                    }).ToList();

                    _db.Bills.AddRange(ResultRecords);

                    _db.SaveChanges();

                    foreach (var item in ResultRecords)
                    {
                        await _notificationService.RequestNotificationAsync(new Models.NotificationRequest()
                        {
                            Action = "NewTransaction",
                            Text = "Masz nową propozycje płatności",
                            Id = item.Id
                        });
                    }
                }
            }
        }

        Console.WriteLine("");
    }
}