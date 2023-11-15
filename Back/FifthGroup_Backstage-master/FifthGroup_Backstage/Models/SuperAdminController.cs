using Microsoft.AspNetCore.Mvc;

namespace FifthGroup_Backstage.Models
{
    public class SuperAdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List()
        {
            return View();
        }
    }
}
