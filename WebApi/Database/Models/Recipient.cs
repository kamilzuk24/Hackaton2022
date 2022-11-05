using System;

namespace WebApi.Database.Models
{
    public class Recipient
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Account { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}