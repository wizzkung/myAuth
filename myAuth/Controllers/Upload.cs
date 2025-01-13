using Microsoft.AspNetCore.Mvc;

namespace myAuth.Controllers
{
    public class Upload : Controller
    {
        public IActionResult ReportExcel()
        {
            return View();
        }

        public IActionResult ReportCsv()
        {
            return View();
        }
    }
}
