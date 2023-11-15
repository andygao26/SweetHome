using Microsoft.AspNetCore.Mvc;

namespace FifthGroup_front.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
