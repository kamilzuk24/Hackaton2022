using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public class BillsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
