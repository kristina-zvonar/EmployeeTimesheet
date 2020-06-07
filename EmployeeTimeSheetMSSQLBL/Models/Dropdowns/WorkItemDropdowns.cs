using EmployeeTimeSheetMSSQLBL.Models.Dropdowns.Generic;
using System.Collections.Generic;

namespace EmployeeTimeSheetMSSQLBL.Models
{
    public class WorkItemDropdowns
    {
        public List<GenericDropdown> WorkItemTypes { get; set; }
        public List<GenericDropdown> WorkItemStatuses { get; set; }
        public List<GenericDropdown> Supervisors { get; set; }
        public List<GenericDropdown> WorkItems { get; set; }
    }
}