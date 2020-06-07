using EmployeeTimeSheetMSSQLBL.Models.Helpers;
using EmployeeTimeSheetMSSQLBL.Models.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace EmployeeTimeSheetMSSQLAPI.Controllers
{
    [Authorize(Roles = "admin,supervisor")]
    [RoutePrefix("api/Report")]
    public class ReportController : BaseController
    {
        [HttpGet]        
        [Route("AnalyticsByWorkItem/{TimePeriod}")]
        public IEnumerable<SummaryObject> AnalyticsByWorkItem(string TimePeriod)
        {
            
            List<SummaryObject> analytics = new List<SummaryObject>();
            try
            {
                if (User.IsInRole("supervisor"))
                {
                    int employeeID = _employeeLogic.GetEmployeeID(User.Identity.Name);
                    analytics = _timeLogEntryLogic.GetAnalyticsByWorkItem(TimePeriod, employeeID);
                }
                else
                {
                    analytics = _timeLogEntryLogic.GetAnalyticsByWorkItem(TimePeriod, 0);
                }
            }
            catch(Exception ex)
            {
                LoggerHelper.WriteError(ex, ex.Message);
            }
           
            return analytics;
        }

        [HttpGet]
        [Route("AnalyticsByEmployeeWorkItem/{TimePeriod}")]
        public IEnumerable<SummaryObject> AnalyticsByEmployeeWorkItem(string TimePeriod)
        {
            List<SummaryObject> analytics = new List<SummaryObject>();
            try
            {
                if (User.IsInRole("supervisor"))
                {
                    int employeeID = _employeeLogic.GetEmployeeID(User.Identity.Name);
                    analytics = _timeLogEntryLogic.GetAnalyticsByEmployeeAndWorkItem(TimePeriod, employeeID);
                }
                else
                {
                    analytics = _timeLogEntryLogic.GetAnalyticsByEmployeeAndWorkItem(TimePeriod, 0);
                }
            }
            catch(Exception ex)
            {
                LoggerHelper.WriteError(ex, ex.Message);
            }

            return analytics;
        }

    }
}