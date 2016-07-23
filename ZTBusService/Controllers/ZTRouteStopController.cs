/*  ZTRouteStopController.cs
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
 * Class to show all selected route's stops and manage them
 * Zhenzhen Tang, 2015.10.10: Created
 */

namespace ZTBusService.Controllers
{
    /// <summary>
    /// Class to manage route stop
    /// </summary>
    public class ZTRouteStopController : Controller
    {
        private BusServiceContext db = new BusServiceContext();

        /// <summary>
        /// Show route stops
        /// </summary>
        /// <param name="busRouteName">route name</param>
        /// <param name="busRouteCode">bus route code</param>
        /// <returns></returns>
        public ActionResult Index(string busRouteName, string busRouteCode="")
        {
            if (busRouteCode != "")
            {
                Session["busRouteCode"] = busRouteCode;
                Session["busRouteName"] = busRouteName;
            }
            else
            {
                if (Session["busRouteCode"] != null)
                {
                    busRouteCode = (string)Session["busRouteCode"];
                    busRouteName = (string)Session["busRouteName"];
                }
                else
                {
                    TempData["Message"] = "Please select a bus route";
                    return RedirectToAction("Index", "ZTBusRoute");
                }
            }

            var routeStops = db.routeStops.Where(b=>b.busRouteCode==busRouteCode).OrderBy(s=>s.offsetMinutes).Include(r => r.busRoute).Include(r => r.busStop);            
            return View(routeStops.ToList());
        }

        /// <summary>
        /// Show detail of the clicked route stop
        /// </summary>
        /// <param name="id">route stop id</param>
        /// <returns></returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            routeStop routeStop = db.routeStops.Find(id);
            if (routeStop == null)
            {
                return HttpNotFound();
            }
            return View(routeStop);
        }

        /// <summary>
        /// Show create route stop page
        /// </summary>
        /// <param name="busRouteCode">bus route code</param>
        /// <returns></returns>
        public ActionResult Create(string busRouteCode)
        {
            ViewBag.busStopNumber = new SelectList(db.busStops, "busStopNumber", "location");
            return View();
        }

        /// <summary>
        /// Create route stop and save to database
        /// </summary>
        /// <param name="routeStop">route stop object</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "routeStopId,busRouteCode,busStopNumber,offsetMinutes")] routeStop routeStop)
        {
            if (ModelState.IsValid)
            {
                db.routeStops.Add(routeStop);
                routeStop.busRouteCode = (string)Session["busRouteCode"];
                db.SaveChanges();
                return RedirectToAction("Index", new { busRouteCode = (string)Session["busRouteCode"], busRouteName = (string)Session["busRouteName"] });
            }

            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routeStop.busRouteCode);
            ViewBag.busStopNumber = new SelectList(db.busStops, "busStopNumber", "location", routeStop.busStopNumber);
            return View(routeStop);
        }

        /// <summary>
        /// Show edit route stop page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            routeStop routeStop = db.routeStops.Find(id);
            if (routeStop == null)
            {
                return HttpNotFound();
            }
            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routeStop.busRouteCode);
            ViewBag.busStopNumber = new SelectList(db.busStops, "busStopNumber", "location", routeStop.busStopNumber);
            return View(routeStop);
        }

        /// <summary>
        /// Edit route stop and save
        /// </summary>
        /// <param name="routeStop"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "routeStopId,busRouteCode,busStopNumber,offsetMinutes")] routeStop routeStop)
        {
            if (ModelState.IsValid)
            {
                db.Entry(routeStop).State = EntityState.Modified;
                routeStop.busRouteCode = (string)Session["busRouteCode"];
                db.SaveChanges();
                return RedirectToAction("Index", new { busRouteCode = (string)Session["busRouteCode"], busRouteName = (string)Session["busRouteName"] });
            }
            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routeStop.busRouteCode);
            ViewBag.busStopNumber = new SelectList(db.busStops, "busStopNumber", "location", routeStop.busStopNumber);
            return View(routeStop);
        }

        /// <summary>
        /// Show delete page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            routeStop routeStop = db.routeStops.Find(id);
            if (routeStop == null)
            {
                return HttpNotFound();
            }
            return View(routeStop);
        }

        /// <summary>
        /// Delete route stop and save
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            routeStop routeStop = db.routeStops.Find(id);
            db.routeStops.Remove(routeStop);
            db.SaveChanges();
            return RedirectToAction("Index", new { busRouteCode = (string)Session["busRouteCode"], busRouteName = (string)Session["busRouteName"] });            
        }

        /// <summary>
        /// Close database connection
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
