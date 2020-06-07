using EmployeeTimeSheetMSSQLBL.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace EmployeeTimeSheetMSSQLBL.Models.ViewModels
{
    public class EmployeeViewModel
    {
        public int ID { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }     
        public string UserID { get; set; }    
        [RequiredInAddMode]
        [Username]
        public string Username { get; set; }
        [RequiredInAddMode]
        [EmailAddress]
        public string Email { get; set; }        
        [RequiredInAddMode]
        [Password]
        public string Password { get; set; }
        [RequiredInAddMode]
        [Password]
        public string ConfirmPassword { get; set; }
        [Required]
        public string RankID { get; set; }     
        [SupervisorRequired]
        public int? SupervisorID { get; set; }
        public string Supervisor { get; set; }
        public bool Active { get; set; }
        public SelectList Supervisors { get; set; }
        public SelectList Users { get; set; }
        public SelectList Ranks { get; set; }
    }
}