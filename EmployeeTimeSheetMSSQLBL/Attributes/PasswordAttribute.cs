using System;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EmployeeTimeSheetMSSQLBL.Attributes
{   

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    sealed public class PasswordAttribute : ValidationAttribute
    {      
        public override bool IsValid(object Value)
        {
            var password = Value as string;
            bool result = true;
            ErrorMessage = "";

            if (!string.IsNullOrEmpty(password))
            {
                if (password.Length < 8 || password.Length > 25)
                {
                    ErrorMessage = "Password length must be between 8 and 25 characters. ";
                    result = false;
                }

                bool containsLowerCaseLetter = password.Any(c => char.IsLetter(c));
                bool containsUpperCaseLetter = password.Any(c => char.IsUpper(c));
                bool containsDigit = password.Any(c => char.IsDigit(c));
                bool containsSpecialCharacter = password.Any(c => char.IsSymbol(c) || char.IsPunctuation(c));

                if (!containsLowerCaseLetter || !containsUpperCaseLetter || !containsDigit || !containsSpecialCharacter)
                {
                    ErrorMessage += "Password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character.";
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