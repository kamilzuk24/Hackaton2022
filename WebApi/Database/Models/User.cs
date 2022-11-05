using System;
using System.Collections.Generic;

namespace WebApi.Database.Models
{
    public class User
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<Bill> Bills { get; set; }

        public List<Recipient> Recipients { get; set; }
    }
}