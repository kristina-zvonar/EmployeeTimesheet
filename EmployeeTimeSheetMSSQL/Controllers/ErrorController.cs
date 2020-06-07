using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeTimeSheetMSSQL.Controllers
{
    [HandleError]
    public class ErrorController : Controller
    {       
        public ActionResult DisplayError(string TypeOfError)
        {
            return View("Error");
        }
    }
}