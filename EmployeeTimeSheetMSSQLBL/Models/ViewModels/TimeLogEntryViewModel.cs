using System.Web.Mvc;

namespace EmployeeTimeSheetMSSQLBL.Models.ViewModels
{
    public class TimeLogEntryViewModel
    {
        public int ID { get; set; }
        public int EmployeeID { get; set; }
        public string Employee { get; set; }
        public int WorkItemID { get; set; }
        public string WorkItem { get; set; }
        public bool IsWorkItemActive { get; set; }
        public decimal Hours { get; set; }        
        public string EntryDate { get; set; }
        public SelectList WorkItems { get; set; }
    }
}