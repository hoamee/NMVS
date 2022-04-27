using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NMVS.Controllers
{
    public class IssueUnplanned : Controller
    {
        [Authorize]
        public IActionResult New()
        {            
            return View();
        }
    }
}