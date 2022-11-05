using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MobileApp.Models;

namespace MobileApp.Services
{
    public interface IBillsService
    {
        Task<List<Bill>> GetPayedBills(Guid userId);

        Task<Bill> GetBill(Guid id);

        Task PayBill(Guid id);
    }
}