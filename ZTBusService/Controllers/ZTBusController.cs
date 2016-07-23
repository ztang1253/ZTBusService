/*  ZTBusController.cs
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
 * Class to show all buses and manage them
 * Zhenzhen Tang, 2015.10.10: Created
 */

namespace ZTBusService.Controllers
{
    /// <summary>
    /// Class to manage buses
    /// </summary>
    public class ZTBusController : Controller
    {
        private BusServiceContext db = new BusServiceContext();

        /// <summary>
        /// Show all bus listing
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View(db.buses.ToList());
        }

        /// <summary>
        /// Details page of the selected bus
        /// </summary>
        /// <param name="id">bus id</param>
        /// <returns></returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bus bus = db.buses.Find(id);
            if (bus == null)
            {
                return HttpNotFound();
            }
            return View(bus);
        }

        /// <summary>
        /// Create bus
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// save to db with entered infos
        /// </summary>
        /// <param name="bus">bus infos</param>
        /// <returns></returns>
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "busId,busNumber,status,comments")] bus bus)
        {
            if (ModelState.IsValid)
            {
                db.buses.Add(bus);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bus);
        }

        /// <summary>
        /// Edit the selected bus
        /// </summary>
        /// <param name="id">bus id</param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bus bus = db.buses.Find(id);
            if (bus == null)
            {
                return HttpNotFound();
            }
            return View(bus);
        }

        /// <summary>
        /// Save the info entered to db
        /// </summary>
        /// <param name="bus">bus infos</param>
        /// <returns></returns>
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "busId,busNumber,status,comments")] bus bus)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bus).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bus);
        }

        /// <summary>
        /// Delete the selected bus
        /// </summary>
        /// <param name="id">bus id</param>
        /// <returns></returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bus bus = db.buses.Find(id);
            if (bus == null)
            {
                return HttpNotFound();
            }
            return View(bus);
        }

        /// <summary>
        /// Confirm to Delete the selected bus
        /// </summary>
        /// <param name="id">bus id</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            bus bus = db.buses.Find(id);
            db.buses.Remove(bus);
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
