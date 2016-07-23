/*  driver.cs
 *  Assignment 5
 * 
 *  Revision History
 *      Zhenzhen Tang, 2015.11.02: Created
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using ZTBusService.App_GlobalResources;

/*
 * Class to add annotations to fields and validate them
 * Zhenzhen Tang, 2015.11.02: Created
 */
namespace ZTBusService.Models
{
    /// <summary>
    /// partial class to overlay for the model with the same name with the generated one
    /// </summary>
    [MetadataType(typeof(ZTDriverMetadata))]
    public partial class driver : IValidatableObject
    {
        string wrongPhoneNumber = "Wrong phone number";
        string homePhoneError = string.Format(ZTTranslations.XIsInvalidPhoneNumber, ZTTranslations.homePhone);
        string workPhoneError = string.Format(ZTTranslations.XIsInvalidPhoneNumber, ZTTranslations.workPhone);
        string blankPhoneError = string.Format(ZTTranslations.XCannotBeBlanks, ZTTranslations.workPhone);
        const int PHONE_NUMBER_CHARS = 10;        

        /// <summary>
        /// validates all the properties and format them correctly before writing into db
        /// </summary>
        /// <param name="validationContext">validation context</param>
        /// <returns>validation result</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //check and format names
            firstName = ZTValidations.Capitalise(firstName);
            lastName = ZTValidations.Capitalise(lastName);
            fullName = lastName + ", " + firstName;

            //check and format phones
            homePhone = phoneNumberFormat(homePhone);
            if (homePhone == wrongPhoneNumber)
            {
                yield return new ValidationResult(homePhoneError, new[] { "homePhone" });
            }
            if (workPhone != null)
            {
                if (workPhone.Length > 0 && workPhone.Trim() == "")
                {
                    yield return new ValidationResult(blankPhoneError, new[] { "workPhone" });                    
                }

                workPhone = phoneNumberFormat(workPhone);
                if (workPhone == wrongPhoneNumber)
                {
                    yield return new ValidationResult(workPhoneError, new[] { "workPhone" });
                }
            }

            //check and format province code
            if (provinceCode != null && provinceCode.Trim() == "")
            {
                yield return new ValidationResult(
                        string.Format(ZTTranslations.XCannotBeBlanks, ZTTranslations.provinceCode), 
                        new[] { "provinceCode" });
            }

            if (provinceCode != null)
            {
                provinceCode = provinceCode.Trim().ToUpper();
            }

            //check and format postal code
            if (postalCode == null || postalCode == "")
            {
                yield return new ValidationResult(
                        string.Format(ZTTranslations.Required, ZTTranslations.postalCode), 
                        new[] { "postalCode" });
            }
            else
            {
                postalCode = postalCode.Trim();
                postalCode = postalCode.ToUpper();
                if (postalCode.Length == 6)
                {
                    postalCode = postalCode.Substring(0, 3) + " " + postalCode.Substring(3);
                }
            }

            if (street !=null)
            {
                street = street.Trim();
            }

            if (city != null)
            {
                city = city.Trim();
            }
        }

        /// <summary>
        /// check phone number is valid or not and format it
        /// </summary>
        /// <param name="phoneNumber">phone number inputted</param>
        /// <returns>if valid, return phone number</returns>
        private string phoneNumberFormat(string phoneNumber)
        {
            List<char> numbers = new List<char>();
            foreach (char c in phoneNumber)
            {
                if (c >= '0' && c <= '9')
                {
                    numbers.Add(c);
                }
            }

            if (numbers.Count == PHONE_NUMBER_CHARS)
            {
                string formatedPhone = "";
                for (int i = 0; i < PHONE_NUMBER_CHARS; i++)
                {
                    formatedPhone += numbers[i];
                    if (i == 2 || i == 5)
                        formatedPhone += "-";
                }
                return formatedPhone;
            }
            return wrongPhoneNumber;
        }
    }

    /// <summary>
    /// metadata class to manage annotations without touching the generated one
    /// </summary>
    public class ZTDriverMetadata
    {
        [Display(Name = "id", ResourceType = typeof(ZTTranslations))]
        public int driverId { get; set; }

        [Required(ErrorMessageResourceName = "Required",
            ErrorMessageResourceType = typeof(ZTTranslations))]
        [StringLength(50, MinimumLength = 2, ErrorMessageResourceName = "StringLength",
            ErrorMessageResourceType = typeof(ZTTranslations))]
        [Display(Name = "firstName", ResourceType = typeof(ZTTranslations))]
        public string firstName { get; set; }

        [Required(ErrorMessageResourceName = "Required",
            ErrorMessageResourceType = typeof(ZTTranslations))]
        [StringLength(50, MinimumLength = 2, ErrorMessageResourceName = "StringLength",
            ErrorMessageResourceType = typeof(ZTTranslations))]
        [Display(Name = "lastName", ResourceType = typeof(ZTTranslations))]
        public string lastName { get; set; }

        [Display(Name = "fullNameDriver", ResourceType = typeof(ZTTranslations))]
        public string fullName { get; set; }

        [Required(ErrorMessageResourceName = "Required",
            ErrorMessageResourceType = typeof(ZTTranslations))]
        [Display(Name = "homePhone", ResourceType = typeof(ZTTranslations))]
        public string homePhone { get; set; }

        [Display(Name = "workPhone", ResourceType = typeof(ZTTranslations))]
        public string workPhone { get; set; }

        [StringLength(50, MinimumLength = 2, ErrorMessageResourceName = "StringLength",
            ErrorMessageResourceType = typeof(ZTTranslations))]
        [Display(Name = "streetAddress", ResourceType = typeof(ZTTranslations))]
        public string street { get; set; }

        [StringLength(50, MinimumLength = 2, ErrorMessageResourceName = "StringLength",
            ErrorMessageResourceType = typeof(ZTTranslations))]
        [Display(Name = "city", ResourceType = typeof(ZTTranslations))]
        public string city { get; set; }

        [PostalCodeValidationAttribute]
        [Display(Name = "postalCode", ResourceType = typeof(ZTTranslations))]
        public string postalCode { get; set; }

        [Display(Name = "provinceCode", ResourceType = typeof(ZTTranslations))]
        [Remote("ProvinceCodeCheck", "ZTDriver")]
        public string provinceCode { get; set; }

        [Display(Name = "dateHired", ResourceType = typeof(ZTTranslations))]
        [DisplayFormat(DataFormatString = "{0: dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [DateNotInFutureAttribute]
        public System.DateTime dateHired { get; set; }
    }
}