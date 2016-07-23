/*  ZTRoleMaintenanceController.cs
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

/*
 * Class to manage roles
 * Zhenzhen Tang, 2015.11.28: Created
 */

namespace ZTBusService.Controllers
{
    /// <summary>
    /// Class to manage roles
    /// </summary>
    [Authorize(Roles = "administrators")]
    public class ZTRoleMaintenanceController : Controller
    {
        public static ApplicationDbContext db = ZTUserMaintenanceController.db;
        private UserManager<ApplicationUser> userManager =
                new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        private RoleManager<IdentityRole> roleManager =
                new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

        /// <summary>
        /// to show all roles sorted by role name
        /// </summary>
        /// <returns>role listing page</returns>
        public ActionResult Index()
        {
            List<IdentityRole> roles = roleManager.Roles.OrderBy(o => o.Name).ToList();
            return View(roles);
        }

        // add a new role, if role name is not already on file
        public ActionResult AddRole(string roleName)
        {
            if (roleName == null || roleName.Trim() == "" || roleManager.RoleExists(roleName))
            {
                TempData["message"] =
                        "Please specify a non-blank role name that's not already on file. ";
            }
            else
            {
                try
                {
                    roleName = roleName.Trim();
                    IdentityResult result = roleManager.Create(new IdentityRole(roleName));
                    if (result.Succeeded)
                    {
                        TempData["message"] = "Role added : " + roleName;
                    }
                    else
                    {
                        TempData["message"] = "Role not added : " + result.Errors.ToList()[0];
                    }
                }
                catch (Exception ex)
                {
                    TempData["message"] = "Exception thrown adding role :" +
                            ex.GetBaseException().Message;
                }
            }

            return RedirectToAction("Index");
        }

        // delete role ... if no memebers
        // if it has members, throw up a confirmation view
        public ActionResult DeleteRole(string roleName)
        {
            if (roleName == null)
            {
                TempData["message"] = "Please select a role first. ";
                return RedirectToAction("Index");
            }

            // Will not delete administrators role, displays message on role listing if tried
            if (roleName == "administrators")
            {
                TempData["message"] = "administrators cannot be deleted. ";
                return RedirectToAction("Index");
            }

            IdentityRole role = roleManager.FindByName(roleName);
            if (role == null)
            {
                TempData["message"] = "role not on file : " + roleName;
                return RedirectToAction("Index");
            }

            // Deletes role if empty
            if (role.Users.Count == 0)
            {
                try
                {
                    IdentityResult result = roleManager.Delete(role);
                    if (result.Succeeded)
                    {
                        TempData["message"] = "Role deleted : " + roleName;
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["message"] = "Deleted failed : " +
                                result.Errors.ToList()[0];
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    TempData["message"] = "Exception deleting role : " +
                            ex.GetBaseException().Message;
                    return RedirectToAction("Index");
                }
            }
            // displays users in role if not empty: deletes on confirm
            else
            {
                ViewBag.roleName = roleName;
                List<ApplicationUser> members = new List<ApplicationUser>();
                foreach (var item in role.Users)
                {
                    members.Add(userManager.FindById(item.UserId));
                }
                return View(members);
            }
        }

        
        // Confirm to Delete the selected role
        public ActionResult DeleteConfirmed(string roleName)
        {
            if (roleName == null)
            {
                TempData["message"] = "Please select a role first. ";
                return RedirectToAction("Index");
            }

            // Will not delete administrators role, displays message on role listing if tried
            if (roleName == "administrators")
            {
                TempData["message"] = "administrators cannot be deleted. ";
                return RedirectToAction("Index");
            }

            IdentityRole role = roleManager.FindByName(roleName);
            if (role == null)
            {
                TempData["message"] = "role not on file : " + roleName;
                return RedirectToAction("Index");
            }

            try
            {
                IdentityResult result = roleManager.Delete(role);
                if (result.Succeeded)
                {
                    TempData["message"] = "Role deleted : " + roleName;
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Deleted failed : " +
                            result.Errors.ToList()[0];
                    return RedirectToAction("DeleteConfirmed", new { roleName = roleName });
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "Exception deleting role : " +
                        ex.GetBaseException().Message;
                return RedirectToAction("DeleteRole", new { roleName = roleName });
            }
        }

        // list all members of the role
        // allow users to be added or removed
        public ActionResult MemberList(string roleName)
        {
            if (roleName == null)
            {
                TempData["message"] = "Please select a role first. ";
                return RedirectToAction("Index");
            }

            IdentityRole role = roleManager.FindByName(roleName);

            List<IdentityUserRole> userRoles = role.Users.ToList();
            List<ApplicationUser> members = new List<ApplicationUser>();
            foreach (var item in userRoles)
            {
                members.Add(userManager.FindById(item.UserId));
            }

            List<ApplicationUser> allUsers = userManager.Users.ToList();
            List<ApplicationUser> nonMembers = new List<ApplicationUser>();

            foreach (var item in allUsers)
            {
                if (!members.Contains(item))
                {
                    nonMembers.Add(item);
                }
            }
            ViewBag.roleName = roleName;
            ViewBag.userName = User.Identity.Name;
            ViewBag.userId = new SelectList(nonMembers, "Id", "userName");

            return View(members);
        }

        // add selected user to current selected role
        public ActionResult AddToRole(string id)
        {
            string roleName;
            if (Session["roleName"] == null)
            {
                TempData["message"] = "Please select a role first. ";
                return RedirectToAction("Index");
            }
            else
            {
                roleName = Session["roleName"].ToString();
            }

            if (id == null)
            {
                TempData["message"] = "Cannot add to role. No user selected. ";
                return RedirectToAction("MemberList", new { roleName = roleName });
            }

            try
            {
                ApplicationUser user = userManager.FindById(id);
                if (user != null)
                {
                    IdentityResult identityResult =
                            userManager.AddToRole(userId: user.Id, role: roleName);
                    if (identityResult.Succeeded)
                    {
                        TempData["message"] = user.UserName + " has been added to role : " + roleName + ". ";
                    }
                    else
                    {
                        TempData["message"] = "Failed to add " + user.UserName +
                                " to role : " + roleName + ". " + "\n\n" +
                                identityResult.Errors.ToList()[0];
                    }
                }
                else
                {
                    TempData["message"] = "User does not exist. Failed to add to role - " + roleName + ". ";
                }
                return RedirectToAction("MemberList", new { roleName = roleName });
            }
            catch (Exception ex)
            {
                TempData["message"] = "Exception on adding to role : " + roleName +
                            ex.GetBaseException().Message;
                return RedirectToAction("MemberList", new { roleName = roleName });
            }
        }

        // remove selected user from current selected role
        public ActionResult RemoveFromRole(string id)
        {
            string roleName;
            if (Session["roleName"] == null)
            {
                TempData["message"] = "Please select a role first. ";
                return RedirectToAction("Index");
            }
            else
            {
                roleName = Session["roleName"].ToString();
            }

            if (id == null)
            {
                TempData["message"] = "Please select a user first. ";
                return RedirectToAction("MemberList", new { roleName = roleName });
            }

            try
            {
                ApplicationUser user = userManager.FindById(id);
                IdentityUser identityUser = userManager.FindById(User.Identity.GetUserId());

                // A user cannot remove herself/himself from the administrators role
                if (id == identityUser.Id && roleName == "administrators")
                {
                    TempData["message"] =
                            "Administrators cannot delete themselves from administrators role. ";
                    return RedirectToAction("MemberList", new { roleName = roleName });
                }

                if (user != null)
                {
                    IdentityResult identityResult =
                            userManager.RemoveFromRole(userId: user.Id, role: roleName);
                    if (identityResult.Succeeded)
                    {
                        TempData["message"] = user.UserName + " has been removed from role : " + roleName + ". ";
                    }
                    else
                    {
                        TempData["message"] = "Failed to remove " + user.UserName +
                                " from role : " + roleName + ". " + "\n\n" +
                                identityResult.Errors.ToList()[0];
                    }
                }
                else
                {
                    TempData["message"] = "User does not exist. Failed to remove from role - " + roleName + ". ";
                }
                return RedirectToAction("MemberList", new { roleName = roleName });
            }
            catch (Exception ex)
            {
                TempData["message"] = "Exception on removing from role : " + roleName +
                            ex.GetBaseException().Message;
                return RedirectToAction("MemberList", new { roleName = roleName });
            }
        }
    }
}
