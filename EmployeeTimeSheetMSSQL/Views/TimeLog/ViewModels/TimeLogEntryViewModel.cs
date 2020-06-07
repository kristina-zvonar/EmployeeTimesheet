using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeTimeSheetMSSQL.Models.ViewModels
{
    public class TimeLogEntryViewModel
    {
        public int ID { get; set; }
        public int EmployeeID { get; set; }
        public string Employee { get; set; }
        public int WorkItemID { get; set; }
        public string WorkItem { get; set; }
        public decimal Hours { get; set; }
    }
}