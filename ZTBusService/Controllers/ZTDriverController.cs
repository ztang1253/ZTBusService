/*  ZTDriverController.cs
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
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using ZTBusService.Models;
using ZTBusService.App_GlobalResources;

/*
 * Class to show all drivers and manage them
 * Zhenzhen Tang, 2015.11.02: Created
 */

namespace ZTBusService.Controllers
{
    /// <summary>
    /// class to manage drivers
    /// </summary>
    public class ZTDriverController : Controller
    {
        private BusServiceContext db = new BusServiceContext();

        // set language before any action runs
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext); // get Request variables,  QueryStrings, cookies, etc.

            // if the language cookie exists … set the UI language and culture
            if (Request.Cookies["language"] != null)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture =
                    new System.Globalization.CultureInfo(Request.Cookies["language"].Value);

                System.Threading.Thread.CurrentThread.CurrentCulture =
                    System.Globalization.CultureInfo.CreateSpecificCulture(Request.Cookies["language"].Value);
            }
        }

        // list all drivers, ordered by full name
        public ActionResult Index()
        {
            var drivers = db.drivers.OrderBy(o => o.fullName)
                    .Include(d => d.province);
            return View(drivers.ToList());
        }

        // Show details of the selected driver record
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                TempData["message"] = "Please select a driver first. ";
                return RedirectToAction("Index");
            }
            driver driver = db.drivers.Find(id);
            if (driver == null)
            {
                TempData["message"] = "The driver (id: " + id + ") does not exist. ";
                return RedirectToAction("Index");
            }
            return View(driver);
        }

        // return a page to add a new driver
        public ActionResult Create()
        {
            driver driver = new driver();
            driver.dateHired = DateTime.Now;
            return View(driver);
        }

        // if the data entered all are valid, add the new driver to the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "driverId,firstName,lastName,fullName,homePhone,workPhone,street,city,postalCode,provinceCode,dateHired")] driver driver)
        {
            try
            {
                if (TryUpdateModel(driver))
                {
                    db.drivers.Add(driver);
                    db.SaveChanges();
                    TempData["message"] = string.Format(ZTTranslations.NewXRecordAdded, ZTTranslations.driverLowerCase) +
                            string.Format(ZTTranslations.TempMessageXisX, ZTTranslations.id, driver.driverId) +
                            string.Format(ZTTranslations.TempMessageXisX, ZTTranslations.fullNameDriver, driver.fullName) +
                            string.Format(ZTTranslations.TempMessageXisX, ZTTranslations.homePhone, driver.homePhone) +
                            string.Format(ZTTranslations.TempMessageXisX, ZTTranslations.dateHired, driver.dateHired.ToString("dd MMM yyyy"));
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", string.Format(ZTTranslations.exceptionTryingToAddX, ZTTranslations.driverLowerCase) + ex.GetBaseException().Message);
                TempData["message"] = string.Format(ZTTranslations.exceptionTryingToAddX, ZTTranslations.driverLowerCase) + ex.GetBaseException().Message;
                return View(driver);
            }
            return View(driver);
        }

        // modify an existing driver record
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                TempData["message"] = "Please select a driver first. ";
                return RedirectToAction("Index");
            }
            driver driver = db.drivers.Find(id);
            if (driver == null)
            {
                TempData["message"] = "The driver (id: " + id + ") does not exist. ";
                return RedirectToAction("Index");
            }
            Session["driverId"] = id;
            var provinceCodeList = new SelectList(db.provinces.OrderBy(o => o.name), "provinceCode", "name", driver.provinceCode).ToList();
            provinceCodeList.Insert(0, new SelectListItem() { Value = "", Text = "--- Please select ---" });
            ViewBag.provinceCode = provinceCodeList;
            return View(driver);
        }

        // if the updated driver information passes edits, update it on the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(driver driver)
        {
            var provinceCodeList = new SelectList(db.provinces.OrderBy(o => o.name), "provinceCode", "name", driver.provinceCode).ToList();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(driver).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["message"] = string.Format(ZTTranslations.TempMessageUpdateSuccessIdName, driver.driverId, driver.fullName);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", 
                        string.Format(ZTTranslations.TempMessageErrorOnEditIdName, driver.driverId, driver.fullName) + 
                        ex.GetBaseException().Message);
                TempData["message"] = string.Format(ZTTranslations.TempMessageErrorOnEditIdName, driver.driverId, driver.fullName) +
                        ex.GetBaseException().Message;

                provinceCodeList.Insert(0, new SelectListItem() { Value = "", Text = string.Format(ZTTranslations.pleaseSelect) });
                ViewBag.provinceCode = provinceCodeList;
                return View(driver);
            }

            provinceCodeList.Insert(0, new SelectListItem() { Value = "", Text = string.Format(ZTTranslations.pleaseSelect) });
            ViewBag.provinceCode = provinceCodeList;
            return View(driver);
        }

        // present a confirmation request to delete the selected driver
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                TempData["message"] = "Please select a driver first. ";
                return RedirectToAction("Index");
            }
            driver driver = db.drivers.Find(id);
            if (driver == null)
            {
                TempData["message"] = "The driver (id: " + id + ") does not exist. ";
                return RedirectToAction("Index");
            }
            return View(driver);
        }

        // delete selected driver
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            driver driver = db.drivers.Find(id);
            try
            {
                db.drivers.Remove(driver);
                db.SaveChanges();
                TempData["message"] = string.Format(ZTTranslations.TempMessageDeleteSuccessIdName, driver.driverId, driver.fullName);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", string.Format(ZTTranslations.TempMessageErrorOnDeleteIdName, driver.driverId, driver.fullName) + ex.GetBaseException().Message);
                TempData["message"] = string.Format(ZTTranslations.TempMessageErrorOnDeleteIdName, driver.driverId, driver.fullName) + ex.GetBaseException().Message;
                return RedirectToAction("Index");
            }
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

        // verify the province code inputted is right or not
        public JsonResult ProvinceCodeCheck(string provinceCode)
        {
            // check if province code is empty string
            if (provinceCode == "")
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            // check if province code is null
            if (provinceCode == null)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            // check if province code is all spaces/blanks
            if (provinceCode.Length > 0 && provinceCode.Trim() == "")
            {
                return Json(String.Format(
                        ZTTranslations.XCannotBeBlanks, ZTTranslations.provinceCode), 
                        JsonRequestBehavior.AllowGet);
            }

            // check if province code is exactly 2 English letters long
            Regex pattern = new Regex(@"^[a-zA-Z]{2}$");
            if (provinceCode.Trim().Length != 2 || !pattern.IsMatch(provinceCode.Trim()))
            {
                return Json(String.Format(
                        ZTTranslations.XCanOnlyBe2EnglishLetters, ZTTranslations.provinceCode), 
                        JsonRequestBehavior.AllowGet);
            }

            // check if province code is in the database
            List<string> dbProvinceCode = new List<string>();
            try
            {
                var pro = db.provinces.ToList();
                for (int i = 0; i < db.provinces.Count(); i++)
                {
                    dbProvinceCode.Add(pro[i].provinceCode);
                }
                if (!dbProvinceCode.Contains(provinceCode.Trim().ToUpper()))
                {
                    return Json(ZTTranslations.WrongProvincePattern, JsonRequestBehavior.AllowGet);
                }
            }
            // catch exception on the select and return ex base message
            catch (Exception ex)
            {
                return Json(String.Format(
                        ZTTranslations.ErrorOnValidatingX, ZTTranslations.provinceCode)
                        + ex.GetBaseException().Message, JsonRequestBehavior.AllowGet);
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}