/*  HomeController.cs
 *  Assignment 4
 * 
 *  Revision History
 *      Zhenzhen Tang, 2015.10.10: Created
 */

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZTBusService.Models;

namespace ZTBusService.Controllers
{
    public class HomeController : Controller
    {
        public static ApplicationDbContext db = ZTUserMaintenanceController.db;
        private UserManager<ApplicationUser> userManager =
                new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        public ActionResult Index()
        {
            ViewBag.isAdministrator = false;
            if (User.Identity.GetUserId() != null)
            {
                ViewBag.isAdministrator =
                  userManager.IsInRole(User.Identity.GetUserId(), "administrators");
                bool a = ViewBag.isAdministrator;
            }
            
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}