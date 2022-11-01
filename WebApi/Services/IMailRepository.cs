using System.Collections.Generic;
using MimeKit;
using WebApi.Models;

namespace WebApi.Services
{
    public interface IMailRepository
    {
        IEnumerable<MimeMessage> GetUnreadMails(MailCredentials credentials);

        IEnumerable<MimeMessage> GetAllMails(MailCredentials credentials);
    }
}