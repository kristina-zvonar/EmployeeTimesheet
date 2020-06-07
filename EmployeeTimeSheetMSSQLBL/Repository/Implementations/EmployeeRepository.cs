using EmployeeTimeSheetMSSQLBL.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Linq.Expressions;
using EmployeeTimeSheetMSSQLBL.Repository.Interfaces;
using EmployeeTimeSheetMSSQLBL.Models.Dropdowns;
using EmployeeTimeSheetMSSQLBL.Models.Dropdowns.Generic;

namespace EmployeeTimeSheetMSSQLBL.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private EmployeeTimesheetEntities _context = null;
        private DbSet<Employee> _table = null;

        public EmployeeRepository()
        {
            this._context = new EmployeeTimesheetEntities();
            this._table = _context.Set<Employee>();
        }

        public EmployeeRepository(EmployeeTimesheetEntities _context)
        {
            this._context = _context;
            this._table = _context.Set<Employee>();
        }
                
        public Employee GetEmployee(string Username)
        {
            var employee = _context.Set<Employee>().FirstOrDefault(x => x.AspNetUser.UserName == Username);
            return employee;
        }

        public EmployeeDropdowns GetDropdowns()
        {
            var supervisors = _table.Where(x => x.AspNetUser.AspNetRoles.Any(r => r.Name == "supervisor" || r.Name == "admin"))
                .Select(x => new GenericDropdown
                {
                    ID = x.ID,
                    Text = x.FirstName + " " + x.LastName
                }).ToList();
            var users = _context.Set<AspNetUser>()
                .Select(x => new GenericStringDropdown
                {
                    ID = x.Id,
                    Text = x.UserName
                }).ToList();

            var ranks = new List<string>() { "supervisor", "employee" };
            return new EmployeeDropdowns
            {
                Supervisors = supervisors,
                Users = users,
                Ranks = ranks.Select(x => new GenericStringDropdown
                {
                    ID = x,
                    Text = x
                }).ToList()
            };

        }

        public bool HasDataAttached(int ID)
        {
            var employee = _table.FirstOrDefault(x => x.ID == ID);
            if (employee != null)
            {
                var hasDataAttached = employee.WorkItems.Any() || employee.Employee1.Any() || employee.TimeLogEntries.Any();
                return hasDataAttached;
            }

            return false;
        }

        public void SoftDeleteEmployee(int ID)
        {
            var employee = _table.FirstOrDefault(x => x.ID == ID);
            if (employee != null)
            {
                employee.Active = false;
                _context.SaveChanges();
            }
        }
    }
}