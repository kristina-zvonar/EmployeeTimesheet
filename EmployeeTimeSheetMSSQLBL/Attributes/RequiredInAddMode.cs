using EmployeeTimeSheetMSSQLBL.Models.ViewModels;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace EmployeeTimeSheetMSSQLBL.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    sealed public class RequiredInAddModeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object Value, ValidationContext ValidationContext)
        {
            var value = Value as string;
            var employee = (EmployeeViewModel)ValidationContext.ObjectInstance;                       

            if (string.IsNullOrEmpty(employee.UserID) && string.IsNullOrEmpty(value))
            {
                string errorMessage = "The " + ValidationContext.MemberName + " field is required.";
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