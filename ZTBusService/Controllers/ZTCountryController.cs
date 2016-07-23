/*  ZTCountryController.cs
 *  Assignment 5
 * 
 *  Revision History
 *      Zhenzhen Tang, 2015.11.02: Created
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
 * Class to show all countries and manage them
 * Zhenzhen Tang, 2015.11.02: Created
 */

namespace ZTBusService.Controllers
{
    /// <summary>
    /// class to manage countries
    /// </summary>
    public class ZTCountryController : Controller
    {
        private BusServiceContext db = new BusServiceContext();

        // list all countries
        public ActionResult Index()
        {
            return View(db.countries.ToList());
        }

        // Show details of the selected country
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            country country = db.countries.Find(id);
            if (country == null)
            {
                return HttpNotFound();
            }
            return View(country);
        }

        // return a page to add a new country
        public ActionResult Create()
        {
            return View();
        }

        // if the data entered all are valid, add the new country to the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="countryCode,name,postalPattern,phonePattern")] country country)
        {
            if (ModelState.IsValid)
            {
                db.countries.Add(country);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(country);
        }

        // modify an existing country record
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            country country = db.countries.Find(id);
            if (country == null)
            {
                return HttpNotFound();
            }
            return View(country);
        }

        // if the updated country information passes edits, update it on the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="countryCode,name,postalPattern,phonePattern")] country country)
        {
            if (ModelState.IsValid)
            {
                db.Entry(country).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(country);
        }

        // present a confirmation request to delete the selected country
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            country country = db.countries.Find(id);
            if (country == null)
            {
                return HttpNotFound();
            }
            return View(country);
        }

        // delete selected country
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            country country = db.countries.Find(id);
            db.countries.Remove(country);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // release rousources for this session: memory & connections
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
