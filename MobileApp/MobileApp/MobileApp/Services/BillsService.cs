using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MobileApp.Models;
using Newtonsoft.Json;

namespace MobileApp.Services
{
    public class BillsService : IBillsService
    {
        public static Guid testUserId = new Guid("1fa7367d-ba99-45be-9228-762c6230706b");

        private const string payedBillsUrl = "api/bills/payed-bills/";
        private const string billUrl = "api/bills/bill-details/";
        private const string payBillUrl = "api/bills/pay-bill/";

        private string _baseApiUrl;
        private HttpClient _client;

        public BillsService(string baseApiUri)
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("Accept", "application/json");

            _baseApiUrl = baseApiUri;
        }

        public async Task<List<Bill>> GetPayedBills(Guid userId)
        {
            var response = await _client.SendAsync(new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_baseApiUrl + payedBillsUrl + userId.ToString())
            });

            return JsonConvert.DeserializeObject<List<Bill>>(await response.Content.ReadAsStringAsync());
        }

        public async Task<Bill> GetBill(Guid id)
        {
            var response = await _client.SendAsync(new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_baseApiUrl + billUrl + id.ToString())
            });

            return JsonConvert.DeserializeObject<Bill>(await response.Content.ReadAsStringAsync());
        }

        public async Task PayBill(Guid id)
        {
            await _client.SendAsync(new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_baseApiUrl + payBillUrl + id.ToString())
            });
        }
    }
}