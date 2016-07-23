/*  ZTBusStopController.cs
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
 * Class to show all bus stops and manage them
 * Zhenzhen Tang, 2015.10.10: Created
 */

namespace ZTBusService.Controllers
{
    /// <summary>
    /// Class to manage bus stops
    /// </summary>
    public class ZTBusStopController : Controller
    {
        private BusServiceContext db = new BusServiceContext();

        /// <summary>
        /// Show all bus stop listing
        /// </summary>
        /// <param name="orderBy">querystring to indicate order rule</param>
        /// <returns></returns>
        public ActionResult Index(string orderBy = "busStopNumber")
        {
            try
            {
                if (orderBy == "busStopNumber")
                {
                    var busStop = db.busStops.OrderBy(s => s.busStopNumber);
                    return View("Index", busStop);
                }
                else
                {
                    var busStop = db.busStops.OrderBy(s => s.location);
                    return View("Index", busStop);
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "Error on sorting: " + ex.GetBaseException().Message;
                return RedirectToAction("Index", "ZTBusStop");
            }
        }

        /// <summary>
        /// Show the selected bus stop's route
        /// </summary>
        /// <param name="id">bus stop id</param>
        /// <returns></returns>
        public ActionResult RouteSelector(int id = 0)
        {
            try
            {
                if (id == 0)
                {
                    TempData["message"] = "Please select a bus stop. ";
                    return RedirectToAction("Index", "ZTBusStop");
                }
                
                var selectedBusStop = db.busStops.Find(id);

                var myRouteStop = (from m in db.routeStops
                                   where m.busStopNumber==id
                                   orderby m.busRoute.routeName
                                   select m).ToArray();

                if (myRouteStop.Count()==0)
                {
                    TempData["message"] = "There is no bus route using the " + id + " bus stop which located at " + selectedBusStop.location + " .";
                    return RedirectToAction("Index", "ZTBusStop");
                }

                var selectedBusStopInRouteStop = selectedBusStop.routeStops.ToList();

                Session["busStopNumber"] = id;
                Session["location"] = selectedBusStop.location;
                Session["offSetMinutes"] = double.Parse(selectedBusStopInRouteStop[0].offsetMinutes.ToString());

                if (myRouteStop.Count()==1)
                {
                    routeStop rs = myRouteStop[0];
                    return RedirectToAction("RouteStopSchedule", "ZTRouteSchedule", new { 
                        busRouteCode = rs.busRouteCode, busStopNumber = id, 
                        location = Session["location"], 
                        offSetMinutes = double.Parse(selectedBusStopInRouteStop[0].offsetMinutes.ToString()) 
                    });
                }
                else
                {
                    ViewBag.busStopNumber = id;
                    ViewBag.location = Session["location"];
                    ViewBag.myBus = myRouteStop.ToList();                    
                    return View();
                }

            }
            catch (Exception ex)
            {
                TempData["message"] = "Error on bus stop: " + ex.GetBaseException().Message;
                return RedirectToAction("Index", "ZTBusStop");
            }
        }

        /// <summary>
        /// Details page of the selected bus stop
        /// </summary>
        /// <param name="id">bus stop id</param>
        /// <returns></returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            busStop busStop = db.busStops.Find(id);
            if (busStop == null)
            {
                return HttpNotFound();
            }
            return View(busStop);
        }

        /// <summary>
        /// Create bus stop
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Create bus stop to db with selected infos
        /// </summary>
        /// <param name="busStop">bus stop infos</param>
        /// <returns></returns>
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "busStopNumber,location,locationHash,goingDowntown")] busStop busStop)
        {
            if (ModelState.IsValid)
            {
                db.busStops.Add(busStop);
                busStop.locationHash = GenerateLocationHash(busStop.location);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(busStop);
        }

        /// <summary>
        /// Edit the selected bus stop
        /// </summary>
        /// <param name="id">bus stop id</param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            busStop busStop = db.busStops.Find(id);
            if (busStop == null)
            {
                return HttpNotFound();
            }
            return View(busStop);
        }

        /// <summary>
        /// Edit the selected bus stop
        /// </summary>
        /// <param name="busStop">bus stop infos</param>
        /// <returns></returns>
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "busStopNumber,location,locationHash,goingDowntown")] busStop busStop)
        {
            if (ModelState.IsValid)
            {
                db.Entry(busStop).State = EntityState.Modified;
                busStop.locationHash = GenerateLocationHash(busStop.location);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(busStop);
        }

        /// <summary>
        /// Delete the selected bus stop
        /// </summary>
        /// <param name="id">bus stop id</param>
        /// <returns></returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            busStop busStop = db.busStops.Find(id);
            if (busStop == null)
            {
                return HttpNotFound();
            }
            return View(busStop);
        }

        /// <summary>
        /// Confirm to Delete the selected bus stop
        /// </summary>
        /// <param name="id">bus stop id</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            busStop busStop = db.busStops.Find(id);
            try
            {
                db.busStops.Remove(busStop);
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Cannot delete. (Foreign Key Constraint)'); window.location='../Index';</script>");
            }
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
        /// <summary>
        /// Generate the hash value for the location name of the bus stop
        /// </summary>
        /// <param name="location">location name of the bus stop</param>
        /// <returns>the hash value for the location name of the bus stop</returns>
        private int GenerateLocationHash(string location)
        {
            int locationHash = 0;

            foreach (char letter in location)
            {
                locationHash += Convert.ToInt32(letter);
            }

            return locationHash;
        }

    }
}
