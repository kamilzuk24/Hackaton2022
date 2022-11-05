using System.Net.Mail;
using System;
using System.Collections.Generic;

namespace Models;

public class Email
{
    public string From { get; set; }
    public string Body { get; set; }
    public string Subject { get; set; }
    public DateTime Date { get; set; }
    public List<EmailAttachment> Attachments { get; set; } = new List<EmailAttachment>();
}