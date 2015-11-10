using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.OleDb;
using System.Data;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;


namespace MvcPlanningApplication.Models
{
    public class ExcelOpenXMLInfo
    {
        private string[] Extensions = new string[] { ".xls", ".xlsx" };
        private string mstrFileName;
        public string FileName
        {
            get { return mstrFileName; }
            set
            {
                if (!Extensions.Contains(System.IO.Path.GetExtension(value.ToLower())))
                    throw new InvalidDataException("Invalid file type");
                mstrFileName = value;
            }
        }

        private List<string> mNameRanges = new List<string>();
        public List<string> NameRanges { get { return mNameRanges; } }

        private List<string> mSheets = new List<string>();
        public List<string> Sheets { get { return mSheets; } }

        public ExcelOpenXMLInfo() { }
        public ExcelOpenXMLInfo(string strFileName)
        {
            FileName = strFileName;
        }
        public bool GetInformation()
        {
            if (!System.IO.File.Exists(FileName))
                throw (new FileNotFoundException("Failed to locate '" + FileName + "'"));

            mSheets.Clear();
            mNameRanges.Clear();


            return true;
        }

        public static DataSet GetDataFromExcel(string strFilePathAndName, string strRangeName = "")
        {
            /*When I used the below connection string then jet would automatically cast my data to the most common type and return nulls wherever data didn't match.
            String sConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;" +
                    "Data Source=" + strFilePathAndName + ";" +
                    "Extended Properties=Excel 8.0;";*/
            /*When I used the below connection string my data would be right but I would have to leave the value HDR=NO because if I told it to use headers with HDR=YES, then it would then cast all my data
             * to strings, and I would then have to implement a method such as:
            private static DataTable BuildHeadersFromFirstRowThenRemoveFirstRow(DataTable dt)
            {
                DataRow firstRow = dt.Rows[0];

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    dt.Columns[i].ColumnName = firstRow[i].ToString().Trim();
                }

                dt.Rows.RemoveAt(0);

                return dt;
            }
             *In order to rename the columns to the names of the first row and then delete the first row.
            String sConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;" +
                    "Data Source=" + strFilePathAndName + ";" +
                    "Extended Properties='Excel 8.0;IMEX=1;HDR=NO;TypeGuessRows=0;ImportMixedTypes=Text;'";*/
            /*Luckily by using ACE.OLEDB.12.0, the Extended Properties attribute functioned as it should; data of like type is converted appropriately and unlike types are converted as text. 
             * However, I had a problem where columns with only 1 or 2 string attributes were still being converted as numbers and so nulls were populating the table.  I found an explanation here
             * http://support.microsoft.com/kb/194124 But basically this is the explanation:
             *              Setting IMEX=1 tells the driver to use Import mode. In this state, the registry setting ImportMixedTypes=Text will be noticed. This forces mixed data to be converted to text. 
             *              For this to work reliably, you may also have to modify the registry setting, TypeGuessRows=8. The ISAM driver by default looks at the first eight rows and from that sampling determines the datatype. 
             *              If this eight row sampling is all numeric, then setting IMEX=1 will not convert the default datatype to Text; it will remain numeric. 
             * However, I could not find the registry key TypeGuessRows=8 in the path referenced in the article; I finally did find it here
             *      x64 -> HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Office\12.0\Access Connectivity Engine\Engines\Excel
             *      x32 -> HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Office\12.0\Access Connectivity Engine\Engines\Excel
             * But alas, I could not get it to work, so I had to set HDR=NO and then remove the use the sub mentioned above.
             */
            String sConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;" +
                        "Data Source=" + strFilePathAndName + ";" +
                        "Extended Properties='Excel 12.0;IMEX=1;HDR=NO;ImportMixedTypes=Text;TypeGuessRows=0;'";

            OleDbConnection objConn = new OleDbConnection(sConnectionString);
            objConn.Open();

            // Create new OleDbCommand to return data from worksheet.
            OleDbCommand objCmdSelect = new OleDbCommand("SELECT * FROM " + strRangeName, objConn);

            // Create new OleDbDataAdapter that is used to build a DataSet
            // based on the preceding SQL SELECT statement.
            OleDbDataAdapter objAdapter1 = new OleDbDataAdapter();

            // Pass the Select command to the adapter.
            objAdapter1.SelectCommand = objCmdSelect;

            // Create new DataSet to hold information from the worksheet.
            DataSet objDataset1 = new DataSet();

            // Fill the DataSet with the information from the worksheet.
            objAdapter1.Fill(objDataset1, "XLData");

            // Clean up objects.
            objConn.Close();

            BuildHeadersFromFirstRowThenRemoveFirstRow(objDataset1.Tables[0]);

            return objDataset1;
        }

        private static System.Data.DataTable BuildHeadersFromFirstRowThenRemoveFirstRow(System.Data.DataTable dt)
        {
            DataRow firstRow = dt.Rows[0];

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                dt.Columns[i].ColumnName = firstRow[i].ToString().Trim();
            }

            dt.Rows.RemoveAt(0);

            return dt;
        }

    }
}