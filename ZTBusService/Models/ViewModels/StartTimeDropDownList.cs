/*  StartTimeDropDownList.cs
 *  Assignment 4
 * 
 *  Revision History
 *      Zhenzhen Tang, 2015.10.10: Created
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/*
 * Class to manage the dropdown list form start time
 * Zhenzhen Tang, 2015.10.10: Created
 */

namespace ZTBusService.Models.ViewModels
{
    /// <summary>
    /// Class to manage the dropdown list form start time
    /// </summary>
    public class StartTimeDropDownList
    {
        public int routeScheduleId { get; set; }
        public string startTime { get; set; }
    }
}