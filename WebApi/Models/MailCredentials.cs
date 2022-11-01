namespace WebApi.Models
{
    public class MailCredentials
    {
        public string MailServer { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public bool SSL { get; set; }
    }
}