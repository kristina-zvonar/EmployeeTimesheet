using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeTimeSheetMSSQL.Models.ViewModels
{
    public class EmployeeViewModel
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RankID { get; set; }
        public int SupervisorID { get; set; }
        public string Supervisor { get; set; }

    }
}