/*  ZTTripController.cs
 *  Assignment 4
 * 
 *  Revision History
 *      Zhenzhen Tang, 2015.10.10: Created
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZTBusService.Models;
using ZTBusService.Models.ViewModels;

/*
 * Class to show all selected route's trips and manage them
 * Zhenzhen Tang, 2015.10.10: Created
 */

namespace ZTBusService.Controllers
{
    /// <summary>
    /// class to manage bus trip
    /// </summary>
    public class ZTTripController : Controller
    {
        private BusServiceContext db = new BusServiceContext();

        /// <summary>
        /// Index page to show bus trip of selected bus route
        /// </summary>
        /// <param name="busRouteCode">bus route code</param>
        /// <param name="busRouteName">bus route name</param>
        /// <returns>The page listing the selected route's trip</returns>
        public ActionResult Index(string busRouteCode = "", string busRouteName = "")
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

            var trip = from t in db.trips
                       where t.routeSchedule.busRouteCode == busRouteCode
                       orderby t.tripDate descending, t.routeSchedule.startTime
                       select t;

            return View(trip.ToList());
        }

        /// <summary>
        /// Create bus trip of selected bus route code
        /// </summary>
        /// <returns>The page to create new trip</returns>
        public ActionResult Create()
        {
            string busRouteCode = (string)Session["busRouteCode"];

            var startTimeDropDown = from d in db.routeSchedules
                                    where d.busRouteCode == busRouteCode
                                    select new StartTimeDropDownList
                                    {
                                        routeScheduleId = d.routeScheduleId,
                                        startTime = (d.isWeekDay ? "Weekday" : "Weekend") + " " + d.startTime.ToString().Substring(0, 5)
                                    };
            ViewBag.startTimeDropDownList = new SelectList(startTimeDropDown, "routeScheduleId", "startTime");

            var driverDropdown = from dr in db.drivers
                                 group dr by dr.fullName into ddr
                                 select ddr.FirstOrDefault();
            ViewBag.driverDropdownList = new SelectList(driverDropdown, "driverId", "fullName");

            ViewBag.busRadioList = from br in db.buses
                                   where br.status == "available"
                                   orderby br.busNumber
                                   group br by br.busNumber into brd
                                   select brd.FirstOrDefault();

            return View();
        }

        /// <summary>
        /// Create bus trip of selected bus trip
        /// </summary>
        /// <param name="trip">trip infomations</param>
        /// <returns>Save new trip to database</returns>
        [HttpPost]
        public ActionResult Create([Bind(Include = "tripId,routeScheduleId,tripDate,driverId,busId,comments")]trip trip)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.trips.Add(trip);
                    db.SaveChanges();
                    TempData["message"] = "New trip for " + @Session["busRouteCode"] + " - " + @Session["busRouteName"]
                                            + " was added. \n" + "Trip Id is " + trip.tripId.ToString() + ".";
                    return RedirectToAction("Index", "ZTTrip", new { busRouteCode = (string)Session["busRouteCode"], busRouteName = (string)Session["busRouteName"] });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.GetBaseException().Message);
                    TempData["message"] = "Error on adding: " + ex.GetBaseException().Message;
                    //return RedirectToAction("Create", "ZTTrip", new { busRouteCode = (string)Session["busRouteCode"], busRouteName = (string)Session["busRouteName"] });
                    return View(trip);
                }
            }

            string busRouteCode = (string)Session["busRouteCode"];

            var startTimeDropDown = from d in db.routeSchedules
                                    where d.busRouteCode == busRouteCode                                    
                                    select new StartTimeDropDownList
                                    {
                                        routeScheduleId = d.routeScheduleId,
                                        startTime = (d.isWeekDay ? "Weekday" : "Weekend") + " " + d.startTime.ToString().Substring(0, 5)
                                    };
            ViewBag.startTimeDropDownList = new SelectList(startTimeDropDown, "routeScheduleId", "startTime");

            var driverDropdown = from dr in db.drivers
                                 group dr by dr.fullName into ddr
                                 select ddr.FirstOrDefault();
            ViewBag.driverDropdownList = new SelectList(driverDropdown, "driverId", "fullName");

            ViewBag.busRadioList = from br in db.buses
                                   where br.status == "available"
                                   orderby br.busNumber
                                   group br by br.busNumber into brd
                                   select brd.FirstOrDefault();
            return View(trip);
        }
    }
}