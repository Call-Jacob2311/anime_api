using Microsoft.AspNetCore.Mvc;

namespace anime_api.Controllers
{
    public class anime_controller : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
