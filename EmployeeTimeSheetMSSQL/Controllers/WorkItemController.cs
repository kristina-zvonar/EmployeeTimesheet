using EmployeeTimeSheetMSSQLBL.Models;
using EmployeeTimeSheetMSSQLBL.Models.Helpers;
using EmployeeTimeSheetMSSQLBL.Models.ViewModels;
using EmployeeTimeSheetMSSQLBL.DAL;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System;

namespace EmployeeTimeSheetMSSQL.Controllers
{
    [Authorize(Roles = "admin,supervisor")]
    public class WorkItemController : BaseController
    {                
        public ActionResult Index()
        {
            return View();
        }
                
        [HttpPost]
        public JsonResult Get(int Skip = 0, int Take = 20)
        {
            try
            {
                List<WorkItemViewModel> model;
                int totalCount = 0;

                if (User.IsInRole(UserRankUtil.SUPERVISOR))
                {
                    // Supervisor should see only work items that s/he created
                    model = _workItemLogic.GetAll(x => x.AspNetUser.UserName == User.Identity.Name, Skip, Take, out totalCount);
                }
                else
                {
                    model = _workItemLogic.GetAll(x => true, Skip, Take, out totalCount);
                }

                return Json(new { data = model, totalCount = totalCount });
            }
            catch(Exception ex)
            {
                LoggerHelper.WriteError(ex, ex.Message);
                return Json(new { totalCount = 0});
            }
        }
                
        [HttpGet]
        public ActionResult Add()
        {
            var model = new WorkItemViewModel
            {
                TypeID = (int)WorkItemTypeUtil.Project,
                StatusID = (int)WorkItemStatusUtil.Active,
                ParentID = 0
            };

            if(User.IsInRole(UserRankUtil.SUPERVISOR))
            {
                var employeeID = _employeeLogic.GetEmployeeID(User.Identity.Name);
                model.SupervisorID = employeeID;
            }

            UpdateDropdowns(ref model);
            
            ViewBag.Mode = "Add";
            return View("AddEdit", model);
        }
            
        [HttpPost]
        public ActionResult Add(WorkItemViewModel Model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userID = UserManager.FindByName(User.Identity.Name).Id;
                    Model.UserID = userID;

                    _workItemLogic.CreateWorkItem(Model);
                    return RedirectToAction("Index", "WorkItem");
                }
                catch(Exception ex)
                {
                    LoggerHelper.WriteError(ex, ex.Message);
                }
            }
            else
            {
                AddErrors();
                UpdateDropdowns(ref Model);
            }

            return View("AddEdit", Model);
        }
                
        [HttpGet]
        public ActionResult Edit(int ID)
        {
            // To stop the supervisor from editing someone else's work item
            var currentEmployeeID = _employeeLogic.GetEmployeeID(User.Identity.Name);
            if(_workItemLogic.IsOwnWorkItem(currentEmployeeID, ID) || User.IsInRole("admin"))
            {
                var model = _workItemLogic.GetModel(ID);
                UpdateDropdowns(ref model);

                ViewBag.Mode = "Edit";
                return View("AddEdit", model);
            }

            return RedirectToAction("Index");
        }
                
        [HttpPost]
        public ActionResult Edit(WorkItemViewModel Model)
        {
            var currentEmployeeID = _employeeLogic.GetEmployeeID(User.Identity.Name);
            if (_workItemLogic.IsOwnWorkItem(currentEmployeeID, Model.ID) || User.IsInRole("admin"))
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        _workItemLogic.UpdateModel(Model);
                        return RedirectToAction("Index", "WorkItem");
                    }
                    catch(Exception ex)
                    {
                        LoggerHelper.WriteError(ex, ex.Message); 
                    }
                }
                else
                {
                    AddErrors();
                    UpdateDropdowns(ref Model);
                    return View("AddEdit", Model);
                }
            }

            return RedirectToAction("Index");
        }
                
        [HttpPost]
        public JsonResult Delete(int ID)
        {
            try
            {
                var currentEmployeeID = _employeeLogic.GetEmployeeID(User.Identity.Name);
                if (_workItemLogic.IsOwnWorkItem(currentEmployeeID, ID) || User.IsInRole("admin"))
                {
                    bool hasData = _workItemLogic.HasDataAttached(ID);

                    if (!hasData)
                    {
                        _workItemLogic.Delete(ID);
                        return Json(new JSONResult { Success = true });
                    }
                    else
                    {
                        return Json(new JSONResult { Success = false, ErrorMessage = ErrorMessages.ERROR_PROCESSING_REQUEST });
                    }
                }

                return Json(new JSONResult { Success = false, ErrorMessage = "You cannot delete someone else's work item." });
            }
            catch(Exception ex)
            {
                LoggerHelper.WriteError(ex, ex.Message);
                return Json(new JSONResult { Success = false, ErrorMessage = ErrorMessages.ERROR_PROCESSING_REQUEST });
            }
            
        }

        [HttpPost]
        public JsonResult CheckIfUnique(string Name)
        {
            try
            {
                bool isUnique = _workItemLogic.IsNameUnique(Name);
                return Json(new JSONResult { Success = isUnique });
            }
            catch(Exception ex)
            {
                LoggerHelper.WriteError(ex, ex.Message);
                return Json(new JSONResult { Success = false, ErrorMessage = ErrorMessages.ERROR_PROCESSING_REQUEST });
            }
        }
                
        [HttpPost]
        public JsonResult UpdateStatus(int[] WorkItemIDs, int Status)
        {
            try
            {
                _workItemLogic.UpdateStatus(WorkItemIDs, Status);
                return Json(new JSONResult { Success = true });
            }
            catch(Exception ex)
            {
                LoggerHelper.WriteError(ex, ex.Message);
                return Json(new JSONResult { Success = false, ErrorMessage = ErrorMessages.ERROR_PROCESSING_REQUEST });
            }
        }
                
        private void UpdateDropdowns(ref WorkItemViewModel Model)
        {
            WorkItemDropdowns dropdowns;
            if(User.IsInRole(UserRankUtil.SUPERVISOR))
            {
                // A parent work item can only be a project, and only one created by this supervisor
                dropdowns = _workItemLogic.GetDropdowns(x => x.AspNetUser.UserName == User.Identity.Name && x.TypeID == (int) WorkItemTypeUtil.Project);
            } else
            {
                dropdowns = _workItemLogic.GetDropdowns(x => true);
            }
            
            Model.WorkItemTypes = new SelectList(dropdowns.WorkItemTypes, "ID", "Text");
            Model.WorkItemStatues = new SelectList(dropdowns.WorkItemStatuses, "ID", "Text");
            Model.Supervisors = new SelectList(dropdowns.Supervisors, "ID", "Text");
            Model.Parents = new SelectList(dropdowns.WorkItems, "ID", "Text");
        }
    }
}