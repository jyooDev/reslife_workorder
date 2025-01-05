using Microsoft.AspNetCore.Mvc;

namespace ReslifeWorkorderManagement.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
