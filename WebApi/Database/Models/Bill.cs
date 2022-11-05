using System;

namespace WebApi.Database.Models
{
    public class Bill
    {
        public Guid Id { get; set; }
        public string Account { get; set; }
        public string Email { get; set; }
        public string Company { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Title { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public bool Payed { get; set; }
    }
}