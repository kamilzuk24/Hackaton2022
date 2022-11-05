using System.Net.Mail;

namespace EmailReaderApi.Models;

public class EmailAttachment
{
    public EmailAttachment() {}

    public EmailAttachment(string name, string url)
    {
        Name = name;
        Url = url;
    }
    
    public string Name { get; set; }
    public string Url { get; set; }
}