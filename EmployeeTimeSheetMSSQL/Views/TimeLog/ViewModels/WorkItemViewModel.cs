using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeTimeSheetMSSQL.Models.ViewModels
{
    public class WorkItemViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public decimal Estimate { get; set; }
        public string UserID { get; set; }
        public int SupervisorID { get; set; }
        public string Supervisor { get; set; }
        public int TypeID { get; set; }
        public string Type { get; set; }
        public int StatusID { get; set; }
        public string Status { get; set; }
        public int ParentID { get; set; }
        public SelectList WorkItemTypes { get; set; }
        public SelectList WorkItemStatues { get; set; }
        public SelectList Supervisors { get; set; }
        public SelectList Parents { get; set; }
    }
}