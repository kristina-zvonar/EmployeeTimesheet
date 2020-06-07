using EmployeeTimeSheetMSSQLBL.DAL;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using EmployeeTimeSheetMSSQLBL.Repository.Interfaces;
using EmployeeTimeSheetMSSQLBL.Models;
using EmployeeTimeSheetMSSQLBL.Models.Dropdowns.Generic;

namespace EmployeeTimeSheetMSSQL.Repository
{
    public class WorkItemRepository : IWorkItemRepository
    {
        private EmployeeTimesheetEntities _context = null;
        private DbSet<WorkItem> _table = null;
        
        public WorkItemRepository()
        {
            this._context = new EmployeeTimesheetEntities();
            this._table = _context.Set<WorkItem>();            
        }

        public WorkItemRepository(EmployeeTimesheetEntities _context)
        {
            this._context = _context;
            this._table = _context.Set<WorkItem>();
        }
                
        public WorkItemDropdowns GetDropdowns(Expression<Func<WorkItem, bool>> Criteria)
        {
            var workItemTypes = _context.Set<WorkItemType>().Select(x => new GenericDropdown
            {
                ID = x.ID,
                Text = x.Name
            }).ToList();

            var workItemStatuses = _context.Set<WorkItemStatu>().Select(x => new GenericDropdown
            {
                ID = x.ID,
                Text = x.Name
            }).ToList();

            var supervisors = _context.Set<Employee>().Where(x => x.RankID == "supervisor" || x.RankID == "admin").Select(x => new GenericDropdown
            {
                ID = x.ID,
                Text = x.FirstName + " " + x.LastName
            }).ToList();
            
            var workItems = _context.Set<WorkItem>().Where(Criteria).Select(x => new GenericDropdown
            {
                ID = x.ID,
                Text = x.Name
            }).ToList();
            
            var dropdowns = new WorkItemDropdowns
            {
                WorkItemTypes = workItemTypes,
                WorkItemStatuses = workItemStatuses,
                Supervisors = supervisors,
                WorkItems = workItems
            };

            return dropdowns;

        }

        public bool IsNameUnique(string Name)
        {
            bool exists = _table.Any(x => x.Name == Name);
            return !exists;
        }

        public bool HasDataAttached(int ID)
        {
            var workItem = _table.FirstOrDefault(x => x.ID == ID);
            if (workItem != null)
            {
                var hasDataAttached = workItem.TimeLogEntries.Any();
                return hasDataAttached;
            }

            return false;
        }

        public Employee GetEmployee(string Username)
        {
            var employee = _context.Set<Employee>().FirstOrDefault(x => x.AspNetUser.UserName == Username);
            return employee;
        }
    }
}