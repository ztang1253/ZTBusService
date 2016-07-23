/*  ZTValidations.cs
 *  Assignment 6
 * 
 *  Revision History
 *      Zhenzhen Tang, 2015.11.13: Created
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

/* 
 * To verify the names and format them
 * 
 * Zhenzhen Tang, 2015-11-13
 */

namespace ZTClassLibrary
{
    /// <summary>
    /// class to verify the names and format them
    /// </summary>
    public class ZTValidations
    {
        /// <summary>
        /// to verify the names and capitalise the first letter of each word in it
        /// </summary>
        /// <param name="message">name</param>
        /// <returns>null or formated name</returns>
        public static string Capitalise(string message)
        {
            if (message == null)
            {
                return null;
            }

            message = message.Trim().ToLower();

            message = System.Threading.Thread.CurrentThread.CurrentCulture.
                    TextInfo.ToTitleCase(message.ToLower());
            return message;
        }
    }
}
