using EmployeeTimeSheetMSSQLBL.Models.Helpers;
using EmployeeTimeSheetMSSQLBL.Models.ViewModels;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace EmployeeTimeSheetMSSQLBL.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    sealed public class SupervisorRequiredAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object Value, ValidationContext ValidationContext)
        {
            var value = Value as string;
            var employee = (EmployeeViewModel)ValidationContext.ObjectInstance;                       

            if (!employee.SupervisorID.HasValue && employee.RankID == UserRankUtil.EMPLOYEE)
            {
                string errorMessage = "Supervisor field is required for employees.";
                var errorResult = new ValidationResult(errorMessage);
                return errorResult;
            }

            return ValidationResult.Success;
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture, ErrorMessageString, name);
        }

    }
}