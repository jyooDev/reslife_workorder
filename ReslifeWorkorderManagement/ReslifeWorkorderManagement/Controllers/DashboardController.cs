using Microsoft.AspNetCore.Mvc;

namespace ReslifeWorkorderManagement.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
