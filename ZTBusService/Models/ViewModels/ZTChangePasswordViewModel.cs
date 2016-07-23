/*  ZTChangePasswordViewModel.cs
 *  Assignment 7
 * 
 *  Revision History
 *      Zhenzhen Tang, 2015.12.01: Created
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

/*
 * Class to check and store the values for change password
 * Zhenzhen Tang, 2015.12.01: Created
 */

namespace ZTBusService.Models
{
    /// <summary>
    /// Class to check and store the values for change password
    /// </summary>
    public class ZTChangePasswordViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}