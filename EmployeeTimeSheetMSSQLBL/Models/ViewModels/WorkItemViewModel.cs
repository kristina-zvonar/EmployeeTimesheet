using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace EmployeeTimeSheetMSSQLBL.Models.ViewModels
{
    public class WorkItemViewModel
    {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]        
        public decimal Estimate { get; set; }
        public string UserID { get; set; }
        public string Creator { get; set; }
        [Required]
        public int SupervisorID { get; set; }
        public string Supervisor { get; set; }
        [Required]
        public int TypeID { get; set; }
        public string Type { get; set; }
        public int StatusID { get; set; }
        public string Status { get; set; }
        public int? ParentID { get; set; }
        public decimal TimeLogged { get; set; }
        public SelectList WorkItemTypes { get; set; }
        public SelectList WorkItemStatues { get; set; }
        public SelectList Supervisors { get; set; }
        public SelectList Parents { get; set; }
    }
}