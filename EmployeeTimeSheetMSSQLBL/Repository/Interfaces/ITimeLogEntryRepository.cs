using EmployeeTimeSheetMSSQLBL.DAL;
using EmployeeTimeSheetMSSQLBL.Models.Dropdowns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTimeSheetMSSQLBL.Repository.Interfaces
{
    public interface ITimeLogEntryRepository
    {
        TimeLogDropdowns GetDropdowns(int SupervisorID);
        Employee GetEmployee(string UserID);
    }
}
