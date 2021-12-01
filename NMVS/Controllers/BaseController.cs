using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NMVS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Controllers
{
    public class BaseController : Controller
    {

        protected ApplicationDbContext _db;

        public BaseController(ApplicationDbContext db)
        {
            _db = db;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                var usrName = HttpContext.Session.GetString("sUserName");
                var usrSession = HttpContext.Session.GetString("sUserHost");
                var activeHostName = _db.Users.FirstOrDefault(x => x.UserName == usrName).ActiveHostName;
                if (activeHostName.Contains(".local"))
                {
                    activeHostName = activeHostName[0..^6];
                }
                if (usrSession != activeHostName)
                {
                    filterContext.Result = RedirectToAction("LogOffConflict", "Account", new {message = activeHostName +", yours is: " + usrSession });
                }
                base.OnActionExecuting(filterContext);
            }
            catch
            {

            }
        }

    }
}
