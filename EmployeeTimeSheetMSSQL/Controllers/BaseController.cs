using EmployeeTimeSheetMSSQLBL.BusinessLogic;
using Microsoft.AspNet.Identity.Owin;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeTimeSheetMSSQL.Controllers
{    
    public class BaseController : Controller
    {               
        private ApplicationUserManager _userManager;
        protected EmployeeLogic _employeeLogic;
        protected WorkItemLogic _workItemLogic;
        protected TimeLogEntryLogic _timeLogEntryLogic;

        public BaseController()
        {
            _employeeLogic = new EmployeeLogic();
            _workItemLogic = new WorkItemLogic();
            _timeLogEntryLogic = new TimeLogEntryLogic();
        }

        public BaseController(ApplicationUserManager UserManager, EmployeeLogic EmployeeLogic, WorkItemLogic WorkItemLogic, TimeLogEntryLogic TimeLogEntryLogic)
        {            
            _userManager = UserManager;
            _employeeLogic = EmployeeLogic;
            _workItemLogic = WorkItemLogic;
            _timeLogEntryLogic = TimeLogEntryLogic;
        }

        protected ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [Authorize(Roles = "admin,supervisor,employee")]
        public void AddErrors()
        {
            foreach (var key in ModelState.Keys)
            {
                var value = ModelState[key];
                if (value.Errors.Any())
                {
                    ModelState.AddModelError(key, string.Join(", ", value.Errors));
                }
            }
        }
    }
}