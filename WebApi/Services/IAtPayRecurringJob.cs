using System.Threading.Tasks;

namespace WebApi.Services;

    public interface IAtPayRecurringJob
    {
        Task ProcessUnreadEmails();
    }

