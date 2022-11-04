using EmailReaderApi.Helpers;
using EmailReaderApi.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;

namespace EmailReaderApi.Services;

public class EmailService : IEmailService
{
    public async Task<IEnumerable<Email>> GetUnreadEmails(GoogleCredential cred)
    {
        var gmailService = new GmailService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = cred
        });
        
        var emailListRequest = gmailService.Users.Messages.List("me");
        emailListRequest.LabelIds = "INBOX";
        emailListRequest.Q = "is:unread";
        emailListRequest.IncludeSpamTrash = false;

        var emails = new List<Email>();
        var emailListResponse = await emailListRequest.ExecuteAsync();

        foreach (var email in emailListResponse.Messages)
        {
            var emailReq = gmailService.Users.Messages.Get("me", email.Id);
            var emailRes = await emailReq.ExecuteAsync();
            var mail = new Email();
            
            foreach (var mParts in emailRes.Payload.Headers)
            {
                switch (mParts.Name)
                {
                    case "Date":
                        mail.Date = DateTime.Parse(mParts.Value);
                        break;
                    case "From":
                        mail.From = mParts.Value;
                        break;
                    case "Subject":
                        mail.Subject = mParts.Value;
                        break;
                }

                if (string.IsNullOrEmpty(mail.From)) continue;
                
                if (emailRes.Payload.Parts == null && emailRes.Payload.Body != null)
                    mail.Body = Decoders.DecodeURLEncodedBase64EncodedString(emailRes.Payload.Body.Data);
                else
                    mail.Body = Decoders.GetNestedBodyParts(emailRes.Payload.Parts, "");

                emails.Add(mail);
            }
        }

        return emails;
    }
    
    
}