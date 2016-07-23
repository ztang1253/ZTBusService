/*  ZTUserMaintenanceController.cs
 *  Assignment 7
 * 
 *  Revision History
 *      Zhenzhen Tang, 2015.11.28: Created
 */

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ZTBusService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

/*
 * Class to manage users
 * Zhenzhen Tang, 2015.11.28: Created
 */

namespace ZTBusService.Controllers
{
    /// <summary>
    /// Class to manage users
    /// </summary>
    [Authorize(Roles = "administrators")]
    public class ZTUserMaintenanceController : Controller
    {
        public static ApplicationDbContext db = new ApplicationDbContext();
        private UserManager<ApplicationUser> userManager =
                new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        private RoleManager<IdentityRole> roleManager =
                new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

        /// <summary>
        /// to show all users sorted by their lock status then user name
        /// </summary>
        /// <returns>user listing page</returns>
        public ActionResult Index()
        {
            List<ApplicationUser> users =
                    userManager.Users.OrderByDescending(o => o.LockoutEnabled)
                    .ThenBy(t => t.UserName).ToList();
            ViewBag.userManager = userManager;
            return View(users);
        }

        /// <summary>
        /// delete user from all roles and system
        /// </summary>
        /// <returns>user listing page with delete result message on top</returns>
        public ActionResult DeleteUser(string userName)
        {
            if (userName == null)
            {
                TempData["message"] = "Please select a user first. ";
            }

            ApplicationUser user = userManager.FindByName(userName);
            if (user == null)
            {
                TempData["message"] = "User not on file : " + userName;
            }
            else
            {
                try
                {
                    IdentityResult identityResult = userManager.Delete(user);
                    if (identityResult.Succeeded)
                    {
                        TempData["message"] = "User deleted : " + userName;
                    }
                    else
                    {
                        TempData["message"] = "deleted failed : " +
                                identityResult.Errors.ToList()[0];
                    }
                }
                catch (Exception ex)
                {
                    TempData["message"] = "Exception on deleting user '" +
                            user.UserName + "' – " + ex.GetBaseException().Message;
                }
            }
            return RedirectToAction("Index");
        }

        // return a page to change password
        public ActionResult ChangePassword(string userName)
        {
            // Only members of the administrators role can reset passwords
            // return them to the Index listing with a message to that effect if they aren’t in that role.
            bool isAdministrator = User.IsInRole("administrators");
            if (!isAdministrator)
            {
                TempData["message"] = "Only adminidtrators can reset password. ";
                return RedirectToAction("Index");
            }

            if (userName == null)
            {
                TempData["message"] = "Please select a user first. ";
                return RedirectToAction("Index");
            }

            ApplicationUser user = userManager.FindByName(userName);
            if (user != null)
            {
                ViewBag.changePasswordUserId = user.Id;
                ViewBag.changePasswordUserName = user.UserName;
                return View();
            }
            else
            {
                TempData["message"] = "User name : " + userName + ". This user is not on file. " + "Cannot change password. ";
                return RedirectToAction("Index");
            }
        }

        // if the data entered all are valid, change the password in the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ZTChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.changePasswordUserName = model.UserName;
                return View(model);
            }

            ApplicationUser user = userManager.FindById(model.Id);
            if (user == null)
            {
                TempData["message"] = "User is not on file. Cannot change password. ";
                return RedirectToAction("Index");
            }

            try
            {
                var provider = new Microsoft.Owin.Security.DataProtection.
                            DpapiDataProtectionProvider("BusService");
                userManager.UserTokenProvider =
                        new Microsoft.AspNet.Identity.Owin.DataProtectorTokenProvider<ApplicationUser>(provider.Create("PasswordReset"));
                string passwordToken = userManager.GeneratePasswordResetToken(user.Id);
                IdentityResult identityResult = userManager.ResetPassword(user.Id, passwordToken, model.NewPassword);
                if (identityResult.Succeeded)
                {
                    TempData["message"] = user.UserName + "'s password has been reset.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Password change failed : " + identityResult.Errors.ToList()[0]; ;
                    return View(model);
                }                
            }
            catch (Exception ex)
            {
                TempData["message"] = "Exception on changing " + user.UserName + "'s password " +
                        " – " + ex.GetBaseException().Message;
                return View(model);
            }
        }

        // reverse the lock out status
        public ActionResult LockOut(string userName)
        {
            if (userName == null)
            {
                TempData["message"] = "Please select a user first. ";
                return RedirectToAction("Index");
            }

            ApplicationUser user = userManager.FindByName(userName);
            if (user == null)
            {
                TempData["message"] = "User not on file : " + userName;
                return RedirectToAction("Index");
            }
            else
            {
                try
                {
                    IdentityResult identityResult;
                    if (user.LockoutEnabled)
                    {
                        user.LockoutEndDateUtc = null;
                        identityResult = userManager.SetLockoutEnabled(user.Id, !user.LockoutEnabled);
                        if (identityResult.Succeeded)
                        {
                            TempData["message"] = userName + " is now unlocked. ";                            
                        }
                        else
                        {
                            TempData["message"] = userName + " unlock failed : " + identityResult.Errors.ToList()[0];                            
                        }                        
                    }
                    else
                    {
                        user.LockoutEndDateUtc = null;
                        identityResult = userManager.SetLockoutEnabled(user.Id, !user.LockoutEnabled);
                        if (identityResult.Succeeded)
                        {
                            TempData["message"] = userName + " is now locked. ";
                        }
                        else
                        {
                            TempData["message"] = userName + "lock failed : " + identityResult.Errors.ToList()[0];
                        }
                    }
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["message"] = "Exception on updating the lockout status of user '"
                            + user.UserName + "' – " + ex.GetBaseException().Message;
                    return RedirectToAction("Index");
                }
            }
        }
    }
}