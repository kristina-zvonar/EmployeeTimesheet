namespace EmployeeTimeSheetMSSQLBL.Attributes
{
    using System;
    using System.Globalization;
    using System.ComponentModel.DataAnnotations;
    using System.Text.RegularExpressions;
    using System.Linq;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    sealed public class UsernameAttribute : ValidationAttribute
    {      
        public override bool IsValid(object Value)
        {
            var username = Value as string;
            bool result = true;
            ErrorMessage = "";

            if (!string.IsNullOrEmpty(username))
            {
                if (username.Length < 6 || username.Length > 15)
                {
                    ErrorMessage = "Username length must be between 6 and 15 characters. ";
                    result = false;
                }

                if (username.Any(x => !char.IsLower(x)))
                {
                    ErrorMessage += "Username cannot begin with a number and may contain only lowercase letters.";
                    result = false;
                }
            }
            return result;
        }
               

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture, ErrorMessageString, name);
        }

    }
}