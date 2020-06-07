using EmployeeTimeSheetMSSQLBL.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace EmployeeTimeSheetMSSQLAPI.Controllers
{
    public class BaseController : ApiController
    {
        protected EmployeeLogic _employeeLogic;
        protected WorkItemLogic _workItemLogic;
        protected TimeLogEntryLogic _timeLogEntryLogic;

        public BaseController()
        {
            _employeeLogic = new EmployeeLogic();
            _workItemLogic = new WorkItemLogic();
            _timeLogEntryLogic = new TimeLogEntryLogic();
        }

        public BaseController(EmployeeLogic EmployeeLogic, WorkItemLogic WorkItemLogic, TimeLogEntryLogic TimeLogEntryLogic)
        {
            _employeeLogic = EmployeeLogic;
            _workItemLogic = WorkItemLogic;
            _timeLogEntryLogic = TimeLogEntryLogic;
        }
    }
}