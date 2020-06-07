using EmployeeTimeSheetMSSQLBL.Models.Helpers;
using EmployeeTimeSheetMSSQLBL.Models.Results;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;

namespace EmployeeTimeSheetMSSQL.Controllers
{
    [Authorize(Roles = "admin,supervisor")]
    public class ReportController : BaseController
    {
        public ActionResult Index(string TimeSpan = "WEEK")
        {
            return View();
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<JsonResult> GetAnalyticsAsync(string Criteria, string TimePeriod = "WEEK")
        {
            var baseAPIAddress = System.Web.Configuration.WebConfigurationManager.AppSettings["APIBaseURL"];

            var employeeID = _employeeLogic.GetEmployeeID(User.Identity.Name);
            var token = _employeeLogic.GetToken(employeeID);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAPIAddress);
                client.DefaultRequestHeaders.Accept.Clear();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);                
                var result = await client.GetAsync($"/api/Report/{Criteria}/{TimePeriod}");
                string resultContent = await result.Content.ReadAsStringAsync();

                List<SummaryObject> analytics = JsonConvert.DeserializeObject<List<SummaryObject>>(resultContent);
                var dataHorizontal = analytics.Select(x => x.X);
                var dataVertical = analytics.Select(x => x.Y);

                return Json(new JSONResult { Success = true, Data = new { X = dataHorizontal, Y = dataVertical } });
            }

        }
    }
}