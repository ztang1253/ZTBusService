/*  ZTLanguageController.cs
 *  Assignment 8
 * 
 *  Revision History
 *      Zhenzhen Tang, 2015.12.04: Created
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

/*
 * Class to manage languages and to allow the user to select their language preference  
 * Zhenzhen Tang, 2015.11.02: Created
 */

namespace ZTBusService.Controllers
{
    public class ZTLanguageController : Controller
    {
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

        // display drop-down with the available languages
        public ActionResult ChangeLanguage()
        {
            // if the language cookie exists, set the UI language and culture
            if (Request.Cookies["language"] != null)	
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture =
                        new System.Globalization.CultureInfo(Request.Cookies["language"].Value);
                System.Threading.Thread.CurrentThread.CurrentCulture =
                        System.Globalization.CultureInfo.CreateSpecificCulture(Request.Cookies["language"].Value);
            }

            SelectListItem en = new SelectListItem() { Text = "English", Value = "en" };
            SelectListItem fr = new SelectListItem() { Text = "French", Value = "fr" };
            SelectListItem zh = new SelectListItem() { Text = "中文", Value = "zh" };

            en.Selected = true;
            if (Request.Cookies["language"] != null)
            {
                switch (Request.Cookies["language"].Value)
                {
                    case "en":
                        en.Selected = true;
                        break;
                    case "fr":
                        fr.Selected = true;
                        break;
                    case "zh":
                        zh.Selected = true;
                        break;
                    default:
                        break;
                }
            }

            SelectListItem[] language = new SelectListItem[] { en, fr, zh };
            ViewBag.language = language;

            if (Request.UrlReferrer != null)
                Response.Cookies.Add(new HttpCookie("returnURL", Request.UrlReferrer.PathAndQuery));
            else
                Response.Cookies.Remove("returnURL");

            return View();
        }

        // save the selected language and return to the page we came from
        [HttpPost]
        public void ChangeLanguage(string language)
        {
            Response.Cookies.Add(new HttpCookie("language", language));
            if (Request.Cookies["returnURL"] != null)
                Response.Redirect(Request.Cookies["returnURL"].Value);
            else
                Response.Redirect("/");
        }
    }
}