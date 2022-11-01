using System.Collections.Generic;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using WebApi.Models;

namespace WebApi.Services
{
    public class MailRepository : IMailRepository
    {
        public IEnumerable<MimeMessage> GetUnreadMails(MailCredentials credentials)
        {
            var messages = new List<MimeMessage>();

            using (var client = new ImapClient())
            {
                client.Connect(credentials.MailServer, credentials.Port, credentials.SSL);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                client.Authenticate(credentials.Login, credentials.Password);

                // The Inbox folder is always available on all IMAP servers...
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);

                var results = inbox.Search(SearchOptions.All, SearchQuery.Not(SearchQuery.Seen));
                foreach (var uniqueId in results.UniqueIds)
                {
                    var message = inbox.GetMessage(uniqueId);

                    messages.Add(message);

                    //Mark message as read
                    //inbox.AddFlags(uniqueId, MessageFlags.Seen, true);
                }

                client.Disconnect(true);
            }

            return messages;
        }

        public IEnumerable<MimeMessage> GetAllMails(MailCredentials credentials)
        {
            var messages = new List<MimeMessage>();

            using (var client = new ImapClient())
            {
                client.Connect(credentials.MailServer, credentials.Port, credentials.SSL);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                client.Authenticate(credentials.Login, credentials.Password);
                var folder = client.GetFolder(SpecialFolder.Sent);
                // The Inbox folder is always available on all IMAP servers...
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);

                var query = SearchQuery.New;
                var uids = client.Inbox.Search(query);
                var items = client.Inbox.Fetch(uids, MessageSummaryItems.UniqueId | MessageSummaryItems.BodyStructure);

                foreach (var item in items)
                {
                    var bodyPart = item.TextBody;

                    // download the 'text/plain' body part
                    var body = (TextPart)client.Inbox.GetBodyPart(item.UniqueId, bodyPart);
                    var text = body.Text;

                    foreach (var attachment in item.Attachments)
                    {
                        // download the attachment just like we did with the body
                        var entity = client.Inbox.GetBodyPart(item.UniqueId, attachment);

                        // attachments can be either message/rfc822 parts or regular MIME parts
                        if (entity is MessagePart)
                        {
                            var rfc822 = (MessagePart)entity;

                            //var path = Path.Combine(directory, attachment.PartSpecifier + ".eml");

                            //rfc822.Message.WriteTo(path);
                        }
                        else
                        {
                            var part = (MimePart)entity;

                            // note: it's possible for this to be null, but most will specify a filename
                            var fileName = part.FileName;

                            //var path = Path.Combine(directory, fileName);

                            // decode and save the content to a file
                            //using (var stream = File.Create(path))
                            //    part.Content.DecodeTo(stream);
                        }
                    }
                }

                client.Disconnect(true);
            }

            return messages;
        }
    }
}