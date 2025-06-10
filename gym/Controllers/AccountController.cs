using Microsoft.AspNetCore.Mvc;

namespace gym.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
