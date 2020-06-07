using EmployeeTimeSheetMSSQLBL.DAL;
using EmployeeTimeSheetMSSQLBL.Models.Dropdowns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTimeSheetMSSQLBL.Repository.Interfaces
{
    public interface IEmployeeRepository
    {
        Employee GetEmployee(string Username);
        EmployeeDropdowns GetDropdowns();
        bool HasDataAttached(int ID);
        void SoftDeleteEmployee(int ID);
    }
}
