/*  ZTBusRouteController.cs
 *  Assignment 4
 * 
 *  Revision History
 *      Zhenzhen Tang, 2015.10.10: Created
 */

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ZTBusService.Models;

/*
 * Class to show all bus routes and manage them
 * Zhenzhen Tang, 2015.10.10: Created
 */

namespace ZTBusService.Controllers
{
    [Authorize]
    /// <summary>
    /// Class to manage bus routes
    /// </summary>
    public class ZTBusRouteController : Controller
    {
        private BusServiceContext db = new BusServiceContext();

        [AllowAnonymous]
        /// <summary>
        /// Show all bus routes
        /// </summary>
        /// <returns>bus routes listing page</returns>
        public ActionResult Index()
        {
            return View(db.busRoutes.ToList());
        }

        /// <summary>
        /// Show clicked bus route details
        /// </summary>
        /// <param name="id">bus route code</param>
        /// <returns>detail page</returns>
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            busRoute busRoute = db.busRoutes.Find(id);
            if (busRoute == null)
            {
                return HttpNotFound();
            }
            return View(busRoute);
        }

        [Authorize(Roles = "administrators, staff")]
        /// <summary>
        /// Show create bus route page
        /// </summary>
        /// <returns>bus route create page</returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Create new boute and save to database
        /// </summary>
        /// <param name="busRoute"></param>
        /// <returns>bus route listing page with new route added</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "busRouteCode,routeName")] busRoute busRoute)
        {
            if (ModelState.IsValid)
            {
                db.busRoutes.Add(busRoute);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(busRoute);
        }

        [Authorize(Roles = "administrators, staff")]
        /// <summary>
        /// Show bus route edit page
        /// </summary>
        /// <param name="id"></param>
        /// <returns>bus route edit page</returns>
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            busRoute busRoute = db.busRoutes.Find(id);
            if (busRoute == null)
            {
                return HttpNotFound();
            }
            return View(busRoute);
        }

        /// <summary>
        /// Edit bus route and save to database
        /// </summary>
        /// <param name="busRoute"></param>
        /// <returns>bus route listing page with edited bus route</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "busRouteCode,routeName")] busRoute busRoute)
        {
            if (ModelState.IsValid)
            {
                db.Entry(busRoute).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(busRoute);
        }

        [Authorize(Roles = "administrators, staff")]
        /// <summary>
        /// Show bus route delete page
        /// </summary>
        /// <param name="id"></param>
        /// <returns>bus route delete page</returns>
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            busRoute busRoute = db.busRoutes.Find(id);
            if (busRoute == null)
            {
                return HttpNotFound();
            }
            return View(busRoute);
        }

        /// <summary>
        /// Delete bus route and save to database
        /// </summary>
        /// <param name="id">bus route listing page</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            busRoute busRoute = db.busRoutes.Find(id);
            try
            {
                db.busRoutes.Remove(busRoute);
                db.SaveChanges();
            }
            catch (Exception)
            {
                
                return Content("<script language='javascript' type='text/javascript'>alert('Cannot delete. (Foreign Key Constraint)'); window.location='../Index';</script>");
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Close the connection with database
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
