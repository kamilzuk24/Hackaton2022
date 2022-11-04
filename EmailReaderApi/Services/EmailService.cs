using System.Net.Mail;
using EmailReaderApi.Helpers;
using EmailReaderApi.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using MimeTypes;

namespace EmailReaderApi.Services;

public class EmailService : IEmailService
{
    private readonly string FILE_ENDPOINT = "https://localhost:7215/attachment";
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
            }

            if (emailRes.Payload.Parts == null) continue;
            foreach (var part in emailRes.Payload.Parts)
            {
                if(string.IsNullOrEmpty(part.Filename)) continue;
                
                var attachId = part.Body.AttachmentId;
                mail.Attachments.Add(new EmailAttachment(part.Filename, $"{FILE_ENDPOINT}/{emailRes.Id}/{attachId}/{part.Filename}"));
            }
            
            emails.Add(mail);
        }

        return emails;
    }

    public async Task<EmailAttachmentViewModel> GetAttachment(GoogleCredential cred, string messageId, string fileId, string name)
    {
        var gmailService = new GmailService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = cred
        });
        
        var attachPart = await gmailService.Users.Messages.Attachments.Get("me",messageId,fileId).ExecuteAsync();
        byte[] data = Decoders.GetBytesFromPart(attachPart.Data);
        var ext = Path.GetExtension(name);

        var model = new EmailAttachmentViewModel()
        {
            Name = name,
            Data = data,
            Extension = ext,
            MimeType = MimeTypeMap.GetMimeType(ext)
        };
        
        return model;
    }
}