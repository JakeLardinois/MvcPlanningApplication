using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Office.Interop.Excel;
using Microsoft.Office;
using System.Runtime.InteropServices;//added Microsoft.Office.Interop.Excel assembly
using System.Data.OleDb;
using System.Data;
using System.Diagnostics;
using System.ComponentModel;


namespace MvcPlanningApplication.Models
{
    public class ExcelInfo
    {
        public Exception LastException { get; set; }

        private string[] Extensions = new string[] { ".xls", ".xlsx" };
        private string mstrFileName;
        public string FileName
        {
            get { return mstrFileName; }
            set
            {
                if (!Extensions.Contains(System.IO.Path.GetExtension(value.ToLower())))
                    throw new Exception("Invalid file name");
                mstrFileName = value;
            }
        }

        private List<string> mNameRanges = new List<string>();
        public List<string> NameRanges { get { return mNameRanges; } }

        private List<string> mSheets = new List<string>();
        public List<string> Sheets { get { return mSheets; } }

        public ExcelInfo() { }
        public ExcelInfo(string strFileName)
        {
            FileName = strFileName;
        }
        public bool GetInformation()
        {
            Application xlApp = null;
            Workbooks xlWorkBooks = null;
            Workbook xlWorkBook = null;
            Workbook xlActiveRanges = null;
            Names xlNames = null;
            Sheets xlWorkSheets = null;
            bool Success = true;



            if (!System.IO.File.Exists(FileName))
            {
                Exception objEx = new Exception("Failed to locate '" + FileName + "'");
                LastException = objEx;
                throw objEx;
            }

            mSheets.Clear();
            mNameRanges.Clear();

            try
            {
                xlApp = new Application();
                xlApp.DisplayAlerts = false;
                xlWorkBooks = xlApp.Workbooks;
                xlWorkBook = xlWorkBooks.Open(FileName);

                xlActiveRanges = xlApp.ActiveWorkbook;
                xlNames = xlActiveRanges.Names;

                for (int x = 1; x <= xlNames.Count; x++)
                {
                    Name xlName = xlNames.Item(x);
                    mNameRanges.Add(xlName.Name);
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(xlName);
                    xlName = null;
                }

                xlWorkSheets = xlWorkBook.Sheets;

                for (int x = 1; x <= xlWorkSheets.Count; x++)
                {
                    Worksheet Sheet1 = (Worksheet)xlWorkSheets[x];
                    mSheets.Add(Sheet1.Name);
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(Sheet1);
                    Sheet1 = null;
                }

                xlWorkBook.Close();
                xlApp.UserControl = true;
                xlApp.Quit();
                TryKillProcessByMainWindowHwnd(xlApp.Hwnd);//I kept getting excel hung up.This method forces the app to close...
            }
            catch (Exception objEx)
            {
                LastException = objEx;
                Success = false;
            }
            finally
            {
                if (xlWorkSheets != null)
                {
                    Marshal.FinalReleaseComObject(xlWorkSheets);
                    xlWorkSheets = null;
                }

                if (xlNames != null)
                {
                    Marshal.FinalReleaseComObject(xlNames);
                    xlNames = null;
                }

                if (xlActiveRanges != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(xlActiveRanges);
                    xlActiveRanges = null;
                }

                if (xlWorkBook != null)
                {
                    Marshal.FinalReleaseComObject(xlWorkBook);
                    xlWorkBook = null;
                }

                if (xlWorkBooks != null)
                {
                    Marshal.FinalReleaseComObject(xlWorkBooks);
                    xlWorkBooks = null;
                }

                if (xlApp != null)
                {
                    Marshal.FinalReleaseComObject(xlApp);
                    xlApp = null;
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            return Success;
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

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        /// <summary> Tries to find and kill process by hWnd to the main window of the process.</summary> 
        /// <param name="hWnd">Handle to the main window of the process.</param> 
        /// <returns>True if process was found and killed. False if process was not found by hWnd or if it could not be killed.</returns> 
        public static bool TryKillProcessByMainWindowHwnd(int hWnd)
        {
            uint processID;

            GetWindowThreadProcessId((IntPtr)hWnd, out processID);
            if (processID == 0)
                return false;

            try
            {
                Process.GetProcessById((int)processID).Kill();
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (Win32Exception)
            {
                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            return true;
        }  /// <summary> Finds and kills process by hWnd to the main window of the process.</summary> 
        /// <param name="hWnd">Handle to the main window of the process.</param> /// <exception cref="ArgumentException"> 
        /// Thrown when process is not found by the hWnd parameter (the process is not running).  
        /// The identifier of the process might be expired. /// </exception> 
        /// <exception cref="Win32Exception">See Process.Kill() exceptions documentation.</exception> 
        /// <exception cref="NotSupportedException">See Process.Kill() exceptions documentation.</exception> 
        /// <exception cref="InvalidOperationException">See Process.Kill() exceptions documentation.</exception> 
        public static void KillProcessByMainWindowHwnd(int hWnd)
        {
            uint processID;
            GetWindowThreadProcessId((IntPtr)hWnd, out processID);
            if (processID == 0)
                throw new ArgumentException("Process has not been found by the given main window handle.", "hWnd");
            Process.GetProcessById((int)processID).Kill();
        }
    }
}