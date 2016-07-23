/*  ZTTripStopController.cs
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
 * Class to show all selected trip's stops and manage them
 * Zhenzhen Tang, 2015.10.10: Created
 */

namespace ZTBusService.Controllers
{
    /// <summary>
    /// Class to manage trip stops
    /// </summary>
    public class ZTTripStopController : Controller
    {
        private BusServiceContext db = new BusServiceContext();

        /// <summary>
        /// Show all trip stops listing
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var tripStops = db.tripStops.Include(t => t.busStop).Include(t => t.trip);
            return View(tripStops.ToList());
        }

        /// <summary>
        /// Details page of the selected trip stop
        /// </summary>
        /// <param name="id">trip stop id</param>
        /// <returns></returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tripStop tripStop = db.tripStops.Find(id);
            if (tripStop == null)
            {
                return HttpNotFound();
            }
            return View(tripStop);
        }

        /// <summary>
        /// Create trip stop to db
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            ViewBag.busStopNumber = new SelectList(db.busStops, "busStopNumber", "location");
            ViewBag.tripId = new SelectList(db.trips, "tripId", "comments");
            return View();
        }

        /// <summary>
        /// Create trip stop to db with selected infos
        /// </summary>
        /// <param name="tripStop">trip stop infos</param>
        /// <returns></returns>
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "tripStopId,tripId,busStopNumber,tripStopTime,comments")] tripStop tripStop)
        {
            if (ModelState.IsValid)
            {
                db.tripStops.Add(tripStop);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.busStopNumber = new SelectList(db.busStops, "busStopNumber", "location", tripStop.busStopNumber);
            ViewBag.tripId = new SelectList(db.trips, "tripId", "comments", tripStop.tripId);
            return View(tripStop);
        }

        /// <summary>
        /// Edit the selected trip stop
        /// </summary>
        /// <param name="id">trip stop id</param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tripStop tripStop = db.tripStops.Find(id);
            if (tripStop == null)
            {
                return HttpNotFound();
            }
            ViewBag.busStopNumber = new SelectList(db.busStops, "busStopNumber", "location", tripStop.busStopNumber);
            ViewBag.tripId = new SelectList(db.trips, "tripId", "comments", tripStop.tripId);
            return View(tripStop);
        }

        /// <summary>
        /// Edit the selected trip stop
        /// </summary>
        /// <param name="tripStop">trip stop infos</param>
        /// <returns></returns>
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "tripStopId,tripId,busStopNumber,tripStopTime,comments")] tripStop tripStop)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tripStop).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.busStopNumber = new SelectList(db.busStops, "busStopNumber", "location", tripStop.busStopNumber);
            ViewBag.tripId = new SelectList(db.trips, "tripId", "comments", tripStop.tripId);
            return View(tripStop);
        }

        /// <summary>
        /// Delete the selected trip stop
        /// </summary>
        /// <param name="id">trip stop id</param>
        /// <returns></returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tripStop tripStop = db.tripStops.Find(id);
            if (tripStop == null)
            {
                return HttpNotFound();
            }
            return View(tripStop);
        }

        /// <summary>
        /// Confirm to Delete the selected trip stop
        /// </summary>
        /// <param name="id">trip stop id</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tripStop tripStop = db.tripStops.Find(id);
            db.tripStops.Remove(tripStop);
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
