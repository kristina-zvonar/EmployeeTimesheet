using EmployeeTimeSheetMSSQLBL.Models.ViewModels;
using EmployeeTimeSheetMSSQLBL.Models.Helpers;
using EmployeeTimeSheetMSSQLBL.DAL;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EmployeeTimeSheetMSSQL.Controllers
{
    public class TimeLogController : BaseController
    {
        [Authorize(Roles = "admin,supervisor,employee")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "admin,supervisor,employee")]
        [HttpPost]
        public JsonResult Get(int Skip = 0, int Take = 20)
        {
            try
            {
                var employeeID = _employeeLogic.GetEmployeeID(User.Identity.Name);
                int totalCount = 0;

                List<TimeLogEntryViewModel> model;
                if (User.IsInRole("admin"))
                {
                    model = _timeLogEntryLogic.GetAll(x => true, Skip, Take, out totalCount);
                }
                else if (User.IsInRole("supervisor"))
                {
                    model = _timeLogEntryLogic.GetAll(x => x.WorkItem.SupervisorID == employeeID, Skip, Take, out totalCount);
                }
                else
                {
                    model = _timeLogEntryLogic.GetAll(x => x.EmployeeID == employeeID, Skip, Take, out totalCount);
                }

                return Json(new { data = model, totalCount = totalCount });
            }
            catch(Exception ex)
            {
                LoggerHelper.WriteError(ex, ex.Message);
                return Json(new { totalCount = 0 });
            }
        }

        [Authorize(Roles = "employee")]
        [HttpGet]
        public ActionResult Add()
        {
            ViewBag.Mode = "Add";
            
            int supervisorID = _employeeLogic.GetSupervisorID(User.Identity.Name);
            int employeeID = _employeeLogic.GetEmployeeID(User.Identity.Name);

            var dropdowns = _timeLogEntryLogic.GetDropdowns(supervisorID);
            var model = new TimeLogEntryViewModel
            {
                EmployeeID = employeeID,
                EntryDate = DateTime.Today.ToString("dd/MM/yyyy"),
                WorkItems = new SelectList(dropdowns.WorkItems, "ID", "Text")
            };
            
            return View("AddEdit", model);
        }

        [Authorize(Roles = "employee")]
        [HttpPost]
        public ActionResult Add(TimeLogEntryViewModel Model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _timeLogEntryLogic.CreateTimeLogEntry(Model);
                    return RedirectToAction("Index", "TimeLog");
                }
                catch(Exception ex)
                {
                    LoggerHelper.WriteError(ex, ex.Message);                    
                }
            }
            return View();
        }

        [Authorize(Roles = "employee")]
        [HttpGet]
        public ActionResult Edit(int ID)
        {
            var model = _timeLogEntryLogic.GetModel(ID);            

            var supervisorID = _employeeLogic.GetSupervisorID(User.Identity.Name);
            var dropdowns = _timeLogEntryLogic.GetDropdowns(supervisorID);

            model.WorkItems = new SelectList(dropdowns.WorkItems, "ID", "Text");
            
            ViewBag.Mode = "Edit";
            return View("AddEdit", model);
        }

        [Authorize(Roles = "employee")]
        [HttpPost]
        public ActionResult Edit(TimeLogEntryViewModel Model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _timeLogEntryLogic.UpdateModel(Model);
                    return RedirectToAction("Index", "TimeLog");
                }
                catch(Exception ex)
                {
                    LoggerHelper.WriteError(ex, ex.Message);
                    return View("AddEdit", Model);
                }
            }
            else
            {
                return View("AddEdit", Model);
            }
        }
    }
}