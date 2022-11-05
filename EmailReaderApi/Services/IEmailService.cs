using EmailReaderApi.Models;
using Google.Apis.Auth.OAuth2;

namespace EmailReaderApi.Services;

public interface IEmailService
{
    public Task<IEnumerable<Email>> GetUnreadEmails();
    public Task<EmailAttachmentViewModel> GetAttachment(string messageId, string fileId, string name);
}