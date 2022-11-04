using EmailReaderApi.Models;
using Google.Apis.Auth.OAuth2;

namespace EmailReaderApi.Services;

public interface IEmailService
{
    public Task<IEnumerable<Email>> GetUnreadEmails(GoogleCredential cred);
    public Task<byte[]> GetAttachment(GoogleCredential cred, string messageId, string fileId);
}