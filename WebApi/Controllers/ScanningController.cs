using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MailScanner;

namespace WebApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ScanningController : ControllerBase
    {
        [HttpGet]
        [Route("test")]
        public async Task<IActionResult> Index()
        {
            //var result = await MailScannerClass.ScanAttachment();

            return new JsonResult(new object());
        }
    }
}
