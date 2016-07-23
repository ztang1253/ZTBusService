/*  ZTRouteScheduleController.cs
 *  Assignment 4
 * 
 *  Revision History
 *      Zhenzhen Tang, 2015.10.10: Created
 */

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
 * Class to show all selected route's schedule and manage them
 * Zhenzhen Tang, 2015.10.10: Created
 */

namespace ZTBusService.Controllers
{
    /// <summary>
    /// Class to manage route schedule
    /// </summary>
    public class ZTRouteScheduleController : Controller
    {
        private BusServiceContext db = new BusServiceContext();

        /// <summary>
        /// Show all route schedule listing
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var routeSchedules = db.routeSchedules.Include(r => r.busRoute);
            return View(routeSchedules.ToList());
        }

        /// <summary>
        /// Show the selected bus stop's route schedule
        /// </summary>
        /// <param name="busRouteCode">bus route code</param>
        /// <param name="busStopNumber">bus stop number</param>
        /// <param name="location">location</param>
        /// <param name="offSetMinutes">offSetMinutes</param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public ActionResult RouteStopSchedule(string busRouteCode = "", int busStopNumber = 0, string location = "", double offSetMinutes = 0, bool flag = false)
        {

            try
            {
                Session["busStopNumber"] = busStopNumber;
                Session["location"] = location;
                Session["offSetMinutes"] = offSetMinutes;
                Session["flag"] = flag;

                var selectedRouteSchedule = db.routeSchedules.Where(r => r.busRouteCode == busRouteCode).OrderBy(o => o.startTime);
                if (busRouteCode == "")
                {
                    int bus = int.Parse(Request.Form["dropdownlist"]);
                    var myBus = db.routeStops.Find(bus);
                    selectedRouteSchedule = db.routeSchedules.Where(r => r.busRouteCode == myBus.busRouteCode).OrderBy(o => o.startTime);
                    Session["offSetMinutes"] = double.Parse(myBus.offsetMinutes.ToString());
                }

                return View(selectedRouteSchedule.ToList());
            }
            catch (Exception)
            {
                TempData["message"] = "Please select a bus stop first";
                return RedirectToAction("Index", "ZTBusStop");
            }
        }

        /// <summary>
        /// Details page of the selected route schedule
        /// </summary>
        /// <param name="id">route schedule id</param>
        /// <returns></returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            routeSchedule routeSchedule = db.routeSchedules.Find(id);
            if (routeSchedule == null)
            {
                return HttpNotFound();
            }
            return View(routeSchedule);
        }

        /// <summary>
        /// Create route schedule
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName");
            return View();
        }

        /// <summary>
        /// Create route schedule to db with selected infos
        /// </summary>
        /// <param name="routeSchedule">route schedule infos</param>
        /// <returns></returns>
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "routeScheduleId,busRouteCode,startTime,isWeekDay,comments")] routeSchedule routeSchedule)
        {
            if (ModelState.IsValid)
            {
                db.routeSchedules.Add(routeSchedule);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routeSchedule.busRouteCode);
            return View(routeSchedule);
        }

        /// <summary>
        /// Edit the selected route schedule
        /// </summary>
        /// <param name="id">route schedule id</param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            routeSchedule routeSchedule = db.routeSchedules.Find(id);
            if (routeSchedule == null)
            {
                return HttpNotFound();
            }
            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routeSchedule.busRouteCode);
            return View(routeSchedule);
        }

        /// <summary>
        /// Edit the selected route schedule
        /// </summary>
        /// <param name="routeSchedule">route schedule infos</param>
        /// <returns></returns>
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "routeScheduleId,busRouteCode,startTime,isWeekDay,comments")] routeSchedule routeSchedule)
        {
            if (ModelState.IsValid)
            {
                db.Entry(routeSchedule).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routeSchedule.busRouteCode);
            return View(routeSchedule);
        }

        /// <summary>
        /// Delete the selected route schedule
        /// </summary>
        /// <param name="id">route schedule id</param>
        /// <returns></returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            routeSchedule routeSchedule = db.routeSchedules.Find(id);
            if (routeSchedule == null)
            {
                return HttpNotFound();
            }
            return View(routeSchedule);
        }

        /// <summary>
        /// Confirm to Delete the selected route schedule
        /// </summary>
        /// <param name="id">route schedule id</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            routeSchedule routeSchedule = db.routeSchedules.Find(id);
            db.routeSchedules.Remove(routeSchedule);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Dispose the session
        /// </summary>
        /// <param name="disposing">whether disconnect to db</param>
        /// <returns></returns>
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
