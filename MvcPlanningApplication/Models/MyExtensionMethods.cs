using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Text;
using MvcPlanningApplication.Models.Haworth;


namespace MvcPlanningApplication.Models
{
    public static class MyExtensionMethods
    {

        public static string AddSingleQuotesAndPadLeft(this string source, int intWidth)
        {
            string[] strArray;
            StringBuilder objStrBldr;


            strArray = source.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length > 0)
            {
                objStrBldr = new StringBuilder();

                foreach (string strTemp in strArray)
                {
                    objStrBldr.Append("'" + strTemp.Trim().PadLeft(intWidth, ' ') + "',");
                }

                return objStrBldr.Remove(objStrBldr.Length - 1, 1).ToString();
            }
            else
                return string.Empty;


        }

        public static string AddSingleQuotes(this string source)
        {
            string[] strArray;
            StringBuilder objStrBldr;


            strArray = source.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length > 0)
            {
                objStrBldr = new StringBuilder();

                foreach (string strTemp in strArray)
                {
                    objStrBldr.Append("'" + strTemp.Trim() + "',");
                }

                return objStrBldr.Remove(objStrBldr.Length - 1, 1).ToString();
            }
            else
                return string.Empty;


        }
    }
}