using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Text;


namespace MvcPlanningApplication.Models
{

    public class QueryDefinitions
    {
        static System.Resources.ResourceManager objResourceManager = new System.Resources.ResourceManager("MvcPlanningApplication.Models.QueryDefs", System.Reflection.Assembly.GetExecutingAssembly());
        private StringBuilder strSQL = new StringBuilder();


        public string GetQuery(string strQueryName)
        {
            return objResourceManager.GetString(strQueryName);
        }

        public string GetQuery(string strQueryName, string[] strParams)
        {
            strSQL.Clear();
            strSQL.Append(objResourceManager.GetString(strQueryName));

            for (int intCounter = strParams.Length - 1; intCounter > -1; intCounter--)
            {
                string strTemp = "~p" + intCounter;
                strSQL.Replace(strTemp, strParams[intCounter]);
            }

            return strSQL.ToString();
        }
    }

    public static class SharedVariables
    {
        public static DateTime MINDATE = new DateTime(1900, 1, 1);
        public static DateTime MAXDATE = new DateTime(2999, 1, 1);
    }

    public static class Settings
    {
        public static string HaworthArchiveLocation { get { return System.Configuration.ConfigurationManager.AppSettings["HaworthArchiveLocation"]; } }
        public static string HaworthFTPURI { get { return System.Configuration.ConfigurationManager.AppSettings["HaworthFTPURI"]; } }
        public static string HaworthFTPUsername { get { return System.Configuration.ConfigurationManager.AppSettings["HaworthFTPUsername"]; } }
        public static string HaworthFTPPassword { get { return System.Configuration.ConfigurationManager.AppSettings["HaworthFTPPassword"]; } }
    }
}