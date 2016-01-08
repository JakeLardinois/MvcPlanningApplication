using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;
using log4net;


namespace MvcPlanningApplication.Models.Haworth
{
    public class HaworthSupplierDemands : List<HaworthSupplierDemand>
    {
        private static readonly ILog Logger = LogHelper.GetLogger();


        public HaworthSupplierDemands(string ExcelFileNameAndLocation, string ExcelRangeName)
            :base()
        {
            //ExcelOpenXMLInfo objExcelInfo = new ExcelOpenXMLInfo(SelectedFile);
            //objExcelInfo.GetInformation();
            Logger.Debug("Getting Data from Excel...");
            var objDataTable = ExcelOpenXMLInfo.GetDataFromExcelRange(ExcelFileNameAndLocation, ExcelRangeName);

            Logger.Debug("Populating the collection with the excel data...");
            Populate(objDataTable);
        }

        private void Populate(DataTable objCurrentDataTable)
        {
            var ErrorList = new List<Exception>();


            foreach (var objRow in objCurrentDataTable.AsEnumerable())
                try
                {
                    this.Add(new HaworthSupplierDemand
                    {
                        PO = objRow.Field<string>("PO Number"),
                        POLine = objRow.Field<string>("PO Item"),
                        POItemConfigurationText = objRow.Field<string>("PO Item Configuration Text"),
                        SO = objRow.Field<string>("SO No."),
                        SOLine = objRow.Field<string>("SO Item"),
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