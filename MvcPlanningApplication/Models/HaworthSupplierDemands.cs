using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;
using log4net;


namespace MvcPlanningApplication.Models
{
    public class HaworthSupplierDemands : List<HaworthSupplierDemand>
    {
        private static readonly ILog Logger = LogHelper.GetLogger();


        public HaworthSupplierDemands(string ExcelFileNameAndLocation, string ExcelRangeName)
            :base()
        {
            //ExcelOpenXMLInfo objExcelInfo = new ExcelOpenXMLInfo(SelectedFile);
            //objExcelInfo.GetInformation();
            var objDataTable = ExcelOpenXMLInfo.GetDataFromExcelRange(ExcelFileNameAndLocation, ExcelRangeName);

            Populate(objDataTable);
        }

        private void Populate(DataTable objCurrentDataTable)
        {
            DateTime dtmTemp;
            int intTemp;
            double dblTemp;
            var ErrorList = new List<Exception>();


            foreach (var objRow in objCurrentDataTable.AsEnumerable())
                try
                {
                    this.Add(new HaworthSupplierDemand
                    {
                        PONumber = objRow.Field<string>("PO Number"),
                        POLine = objRow.Field<string>("PO Item"),
                        POItemConfigurationText = objRow.Field<string>("PO Item Configuration Text")
                    });
                }
                catch (Exception objEx)
                {
                    ErrorList.Add(objEx);
                    Logger.Error("Haworth Supplier Demand Exception Thrown in Populate()", objEx);
                }

            if (ErrorList.Count > 0)
                throw ErrorList.First();
        }
    }
}