using EmployeeTimeSheetMSSQLBL.DAL;
using EmployeeTimeSheetMSSQLBL.Models.Dropdowns;
using EmployeeTimeSheetMSSQLBL.Models.Dropdowns.Generic;
using EmployeeTimeSheetMSSQLBL.Models.Helpers;
using EmployeeTimeSheetMSSQLBL.Repository.Interfaces;
using System.Data.Entity;
using System.Linq;

namespace EmployeeTimeSheetMSSQL.Repository
{
    public class TimeLogRepository : ITimeLogEntryRepository
    {
        private EmployeeTimesheetEntities _context = null;
        private DbSet<TimeLogEntry> _table = null;

        public TimeLogRepository()
        {
            this._context = new EmployeeTimesheetEntities();
            this._table = _context.Set<TimeLogEntry>();
        }

        public TimeLogRepository(EmployeeTimesheetEntities _context)
        {
            this._context = _context;
            this._table = _context.Set<TimeLogEntry>();
        }
             
        public TimeLogDropdowns GetDropdowns(int SupervisorID)
        {            
            var workItems = _context.Set<WorkItem>().Where(x => x.SupervisorID == SupervisorID && x.StatusID == (int) WorkItemStatusUtil.Active).Select(x => new GenericDropdown
            {
                ID = x.ID,
                Text = x.WorkItemType.Name + " " + x.Name
            }).ToList();

            var dropdowns = new TimeLogDropdowns
            {                
                WorkItems = workItems
            };

            return dropdowns;
        }

        public Employee GetEmployee(string UserID)
        {
            var employee = _context.Set<Employee>().FirstOrDefault(x => x.UserID == UserID);
            return employee;
        }
                
    }
}