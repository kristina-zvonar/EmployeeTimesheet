using EmployeeTimeSheetMSSQLBL.Models;
using EmployeeTimeSheetMSSQLBL.Models.Helpers;
using EmployeeTimeSheetMSSQLBL.Models.ViewModels;
using EmployeeTimeSheetMSSQLBL.Repository;
using EmployeeTimeSheetMSSQLBL.DAL;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EmployeeTimeSheetMSSQL.Models;

namespace EmployeeTimeSheetMSSQL.Controllers
{
    [Authorize(Roles = "admin,supervisor")]
    public class EmployeeController : BaseController
    {           
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
                
        [HttpPost]
        public JsonResult Get(int Skip = 0, int Take = 20)
        {
            try
            {
                List<EmployeeViewModel> model;
                int totalCount = 0;
                if (User.IsInRole("supervisor"))
                {
                    // Supervisor should see only employees that s/he created and/or managed
                    var employeeID = _employeeLogic.GetEmployeeID(User.Identity.Name);
                    model = _employeeLogic.GetAll(x => x.SupervisorID == employeeID, out totalCount, Skip, Take);
                }
                else
                {
                    model = _employeeLogic.GetAll(x => true, out totalCount, Skip, Take);
                }

                return Json(new { data = model, totalCount = totalCount });
            }
            catch(Exception ex)
            {
                LoggerHelper.WriteError(ex, ex.Message);
                return Json( new {  totalCount = 0});
            }
        }
                
        [HttpGet]
        public ActionResult Add()
        {
            ViewBag.Mode = "Add";
            var dropdowns = _employeeLogic.GetDropdowns();
            var model = new EmployeeViewModel
            {
                RankID = UserRankUtil.EMPLOYEE,
                Supervisors = new SelectList(dropdowns.Supervisors, "ID", "Text"),
                Users = new SelectList(dropdowns.Users, "ID", "Text"),
                Ranks = new SelectList(dropdowns.Ranks, "ID", "Text")
            };
            
            return View("AddEdit", model);
        }
                
        [HttpPost]
        public ActionResult Add(EmployeeViewModel Model)
        {            
           
            if (ModelState.IsValid)
            {
                try
                {
                    if (Model.Password != Model.ConfirmPassword)
                    {
                        ModelState.AddModelError("ConfirmPassword", "Confirm Password field does not match Password field.");
                        UpdateDropdowns(ref Model);
                        ViewBag.Mode = "Add";
                        return View("AddEdit", Model);
                    }

                    var user = new ApplicationUser { UserName = Model.Username, Email = Model.Email };

                    var result = UserManager.Create(user, Model.Password);
                    var currentUser = UserManager.FindByName(user.UserName);
                    var currentEmployeeID = _employeeLogic.GetEmployeeID(User.Identity.Name);
                    var userID = currentUser.Id;

                    if (result.Succeeded)
                    {
                        var roleResult = UserManager.AddToRole(currentUser.Id, Model.RankID);
                        if (roleResult.Succeeded)
                        {
                            Model.UserID = userID;

                            // When the supervisor is creating an employee, he/she is the supervisor of that employee
                            // as the supervisor is not aware of other supervisors or admins
                            if (User.IsInRole(UserRankUtil.SUPERVISOR))
                            {
                                Model.SupervisorID = currentEmployeeID;
                                Model.RankID = UserRankUtil.EMPLOYEE;
                            }

                            _employeeLogic.CreateEmployee(Model);
                            return RedirectToAction("Index", "Employee");
                        }
                    }
                }
                catch(Exception ex)
                {
                    LoggerHelper.WriteError(ex, ex.Message);                    
                }
            }
            else
            {
                AddErrors();
            }

            UpdateDropdowns(ref Model);
            ViewBag.Mode = "Add";
            return View("AddEdit", Model);
        }
                
        [HttpGet]
        public ActionResult Edit(int ID)
        {
            var model = _employeeLogic.GetEmployeeModel(ID);

            ViewBag.Mode = "Edit";
            return View("AddEdit", model);
        }
                
        [HttpPost]
        public ActionResult Edit(EmployeeViewModel Model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var employee = _employeeLogic.CreateEmployeeObject(Model);

                    var associatiedUser = UserManager.FindById(employee.UserID);
                    if (associatiedUser != null)
                    {
                        bool isDemoted = Model.RankID == UserRankUtil.EMPLOYEE && UserManager.IsInRole(associatiedUser.Id, UserRankUtil.SUPERVISOR);
                        bool isPromoted = Model.RankID == UserRankUtil.SUPERVISOR && UserManager.IsInRole(associatiedUser.Id, UserRankUtil.EMPLOYEE);

                        if (isDemoted || isPromoted)
                        {
                            associatiedUser.Roles.Clear();
                            UserManager.AddToRole(associatiedUser.Id, employee.RankID);

                            // Supervisor's "supervisor" is the admin doing the promotion from employee to supervisor
                            if (employee.RankID == UserRankUtil.SUPERVISOR)
                            {
                                var currentEmployeeID = _employeeLogic.GetEmployeeID(User.Identity.Name);
                                employee.SupervisorID = currentEmployeeID;
                            }
                        }
                    }

                    _employeeLogic.UpdateModel(employee);
                    return RedirectToAction("Index", "Employee");
                } 
                catch(Exception ex)
                {
                    LoggerHelper.WriteError(ex, ex.Message);
                    AddErrors();
                    UpdateDropdowns(ref Model);
                    return View("AddEdit", Model);
                }
            }
            else
            {
                AddErrors();
                UpdateDropdowns(ref Model);
                return View("AddEdit", Model);
            }
        }

        [HttpPost]
        public JsonResult CheckIfUniqueUsername(string Username)
        {
            try
            {
                bool isUnique = UserManager.FindByName(Username) == null;
                return Json(new JSONResult { Success = isUnique });
            }
            catch(Exception ex)
            {
                LoggerHelper.WriteError(ex, ex.Message);
                return Json(new JSONResult { Success = false });
            }
        }
                        
        [HttpPost]
        public JsonResult Delete(int ID)
        {
            try
            {
                bool hasData = _employeeLogic.HasDataAttached(ID);

                var employeeUserID = _employeeLogic.GetUserID(ID);

                if (hasData)
                {
                    _employeeLogic.SoftDeleteEmployee(ID);

                    var lockoutEndDate = new DateTime(2999, 01, 01);
                    UserManager.SetLockoutEnabled(employeeUserID, true);
                    UserManager.SetLockoutEndDate(employeeUserID, lockoutEndDate);
                }
                else
                {
                    _employeeLogic.Delete(ID);

                    var user = UserManager.FindById(employeeUserID);
                    _employeeLogic.DeleteTokens(user.Id);
                    
                    UserManager.Delete(user);
                }

                return Json(new JSONResult { Success = true });
            }
            catch(Exception ex)
            {
                LoggerHelper.WriteError(ex, ex.Message);
                return Json(new JSONResult { Success = false, ErrorMessage = "There was an error processing your request." });
            }
        }               
                
        private void UpdateDropdowns(ref EmployeeViewModel Model)
        {
            var dropdowns = _employeeLogic.GetDropdowns();
            Model.Supervisors = new SelectList(dropdowns.Supervisors, "ID", "Text");
            Model.Users = new SelectList(dropdowns.Users, "ID", "Text");
            Model.Ranks = new SelectList(dropdowns.Ranks, "ID", "Text");
        }
    }
}