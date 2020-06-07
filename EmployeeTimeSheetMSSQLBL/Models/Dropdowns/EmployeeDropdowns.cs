using System.Collections.Generic;
using EmployeeTimeSheetMSSQLBL.Models.Dropdowns.Generic;

namespace EmployeeTimeSheetMSSQLBL.Models.Dropdowns
{
    public class EmployeeDropdowns
    {
        public List<GenericDropdown> Supervisors { get; set; }
        public List<GenericStringDropdown> Users { get; set; }
        public List<GenericStringDropdown> Ranks { get; set; }
    }
}