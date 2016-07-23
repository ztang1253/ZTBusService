/*  DateNotInFutureAttribute.cs
 *  Assignment 6
 * 
 *  Revision History
 *      Zhenzhen Tang, 2015.11.13: Created
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

/* 
 * To verify the hire date not in the future
 * 
 * Zhenzhen Tang, 2015-11-13
 */
namespace ZTClassLibrary
{
    /// <summary>
    /// class to verify the hire date not in the future
    /// </summary>
    public class DateNotInFutureAttribute : ValidationAttribute
    {
        /// <summary>
        /// constructor to initialize the ErrorMessage
        /// </summary>
        public DateNotInFutureAttribute()
        {
            ErrorMessage = "{0} cannot be in the future";
        }

        /// <summary>
        /// to verify the hire date not in the future
        /// </summary>
        /// <param name="value">hire date</param>
        /// <param name="validationContext">validationContext</param>
        /// <returns>ValidationResult</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || value.ToString() == "")
            {
                return ValidationResult.Success;
            }

            DateTime date;
            try
            {
                date = DateTime.Parse(value.ToString());
            }
            catch (Exception)
            {
                ErrorMessage = "{0} is not a valid date formate : 2005-12-31";
                return new ValidationResult(string.Format(ErrorMessage, validationContext.DisplayName));
            }

            if (date > DateTime.Now)
            {
                return new ValidationResult(string.Format(ErrorMessage, validationContext.DisplayName));
            }
            return ValidationResult.Success;
        }
    }
}
