using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Runtime.CompilerServices;


namespace MvcPlanningApplication.Models
{
    public class LogHelper
    {
        /*REQUIRES .NET 4.5 - .NET 4.5 has an attribute called filepath, and so we give it "string filename = """ below.  This is an optional parameter and so what this does is if we don't add something to the filename parameter
         * then the 'CallerFilePath' attribute takes over and gives it the full path to the file that is calling this method. NOTE that if you have more than 1 class in a file then you will recieve the same file name
         * So it is important to follow standard convention and have only 1 class per file.
         */
        public static log4net.ILog GetLogger([CallerFilePath]string filename = "")
        {
            return log4net.LogManager.GetLogger(filename);
        }
    }
}