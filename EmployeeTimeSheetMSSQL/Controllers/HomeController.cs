using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeTimeSheetMSSQL.Controllers
{
    public class HomeController : Controller
    {
        [Authorize(Roles = "admin,supervisor,employee")]
        public ActionResult Index()
        {            
            if (User.IsInRole("supervisor"))
            {
                return RedirectToAction("Index", "WorkItem");
            } 
            else if(User.IsInRole("employee"))
            {
                return RedirectToAction("Index", "TimeLog");
            }
            else if(User.IsInRole("admin"))
            {
                return RedirectToAction("Index", "Employee");
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}