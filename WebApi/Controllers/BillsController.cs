using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WebApi.Database;
using WebApi.Database.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillsController : ControllerBase
    {
        private Guid testUserId = new Guid("1fa7367d-ba99-45be-9228-762c6230706b");
        private DatabaseContext database;

        public BillsController(DatabaseContext database)
        {
            this.database = database;
        }

        [HttpGet]
        [Route("payed-bills/{userId}")]
        public List<Bill> GetHistory(Guid userId)
        {
            return this.database.Bills.Where(x => x.UserId == userId).OrderBy(x => x.Payed).ToList();
        }

        [HttpGet]
        [Route("bill-details/{Id}")]
        public Bill GetBill(Guid Id)
        {
            return this.database.Bills.FirstOrDefault(x => x.Id == Id) ?? new Bill();
        }

        [HttpPost]
        [Route("pay-bill/{Id}")]
        public bool PayBill(Guid Id)
        {
            var bill = this.database.Bills.FirstOrDefault(x => x.Id == Id);
            bill.Payed = true;
            this.database.SaveChanges();

            return true;
        }
    }
}