/*  PostalCodeValidationAttribute.cs
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
using System.Text.RegularExpressions;

/* 
 * To verify the postal code
 * 
 * Zhenzhen Tang, 2015-11-13
 */

namespace ZTClassLibrary
{
    /// <summary>
    /// class to verify the postal code is a valid Canadian postal code
    /// </summary>
    public class PostalCodeValidationAttribute : ValidationAttribute
    {
        /// <summary>
        /// constructor to initialize the ErrorMessage
        /// </summary>
        public PostalCodeValidationAttribute()
        {
            ErrorMessage = "{0} is not a valid Canadian postal pattern: A3A 3A3";
        }

        /// <summary>
        /// to verify the postal code is a valid Canadian postal code
        /// </summary>
        /// <param name="value">postal code</param>
        /// <param name="validationContext">validationContext</param>
        /// <returns>ValidationResult</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // check if the inputs are all blanks/spaces
            if (value.ToString().Length > 0 && value.ToString().Trim() == "")
            {
                ErrorMessage = "{0} cannot be just blanks/spaces. ";
                return new ValidationResult(ErrorMessage);
            }

            // check if the inputs are a valid Canadian postal code
            Regex pattern = new Regex(@"^[a-ceghj-npr-zA-CEGHJ-NPR-Z]\d[a-ceghj-npr-zA-CEGHJ-NPR-Z] ?\d[a-ceghj-npr-zA-CEGHJ-NPR-Z]\d$");
            if (value == null || pattern.IsMatch(value.ToString().Trim()))
            {
                return ValidationResult.Success;
            }
            
            return new ValidationResult(string.Format(ErrorMessage, validationContext.DisplayName));
        }
    }
}