/*  ZTProvinceController.cs
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
 * Class to show all provinces and manage them
 * Zhenzhen Tang, 2015.11.02: Created
 */

namespace ZTBusService.Controllers
{
    public class ZTProvinceController : Controller
    {
        private BusServiceContext db = new BusServiceContext();

        // list all provinces
        public ActionResult Index()
        {
            var provinces = db.provinces.Include(p => p.country);
            return View(provinces.ToList());
        }

        // Show details of the selected province record
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            province province = db.provinces.Find(id);
            if (province == null)
            {
                return HttpNotFound();
            }
            return View(province);
        }

        // return a page to add a new province
        public ActionResult Create()
        {
            ViewBag.countryCode = new SelectList(db.countries, "countryCode", "name");
            return View();
        }

        // if the data entered all are valid, add the new province to the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="provinceCode,name,countryCode,taxCode,taxRate,capital")] province province)
        {
            if (ModelState.IsValid)
            {
                db.provinces.Add(province);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.countryCode = new SelectList(db.countries, "countryCode", "name", province.countryCode);
            return View(province);
        }

        // modify an existing province record
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            province province = db.provinces.Find(id);
            if (province == null)
            {
                return HttpNotFound();
            }
            ViewBag.countryCode = new SelectList(db.countries, "countryCode", "name", province.countryCode);
            return View(province);
        }

        // if the updated province information passes edits, update it on the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="provinceCode,name,countryCode,taxCode,taxRate,capital")] province province)
        {
            if (ModelState.IsValid)
            {
                db.Entry(province).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.countryCode = new SelectList(db.countries, "countryCode", "name", province.countryCode);
            return View(province);
        }

        // present a confirmation request to delete the selected province
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            province province = db.provinces.Find(id);
            if (province == null)
            {
                return HttpNotFound();
            }
            return View(province);
        }

        // delete selected province
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            province province = db.provinces.Find(id);
            db.provinces.Remove(province);
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
