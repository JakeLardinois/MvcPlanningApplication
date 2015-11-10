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
            DataSet objDataSet = ExcelOpenXMLInfo.GetDataFromExcel(ExcelFileNameAndLocation, ExcelRangeName);

            Populate(objDataSet.Tables[0]);
        }

        private void Populate(DataTable objCurrentDataTable)
        {
            try
            {
                foreach (var objRow in objCurrentDataTable.AsEnumerable())
                    this.Add(new HaworthSupplierDemand
                    {
                        CHGInd = objRow.Field<string>("CHG Ind"),
                        MAD = objRow.Field<DateTime>("CHG Ind"),
                        SOCrDte = objRow.Field<DateTime>("CHG Ind"),
                        SONo = objRow.Field<string>("CHG Ind"),
                        Item1 = objRow.Field<string>("CHG Ind"),
                        SOQty = objRow.Field<int>("CHG Ind"),
                        PONumber1 = objRow.Field<string>("CHG Ind"),
                        Item2 = objRow.Field<string>("CHG Ind"),
                        POQty = objRow.Field<int>("CHG Ind"),
                        MatNo = objRow.Field<string>("CHG Ind"),
                        Description = objRow.Field<string>("CHG Ind"),
                        CatalogPartNumber = objRow.Field<string>("CHG Ind"),
                        CatalogPartDescription = objRow.Field<string>("CHG Ind"),
                        PlnText = objRow.Field<string>("CHG Ind"),
                        Customer = objRow.Field<string>("CHG Ind"),
                        Name = objRow.Field<string>("CHG Ind"),
                        Street = objRow.Field<string>("CHG Ind"),
                        ShipToCity = objRow.Field<string>("CHG Ind"),
                        Region = objRow.Field<string>("CHG Ind"),
                        ShToPos = objRow.Field<string>("CHG Ind"),
                        Country = objRow.Field<string>("CHG Ind"),
                        ShipPnt = objRow.Field<int>("CHG Ind"),
                        SNNo = objRow.Field<string>("CHG Ind"),
                        OrigSO = objRow.Field<string>("CHG Ind"),
                        PONumber2 = objRow.Field<string>("CHG Ind"),
                        Usage = objRow.Field<string>("CHG Ind"),
                        ReasonRej = objRow.Field<string>("CHG Ind"),
                        DelDate = objRow.Field<DateTime>("CHG Ind"),
                        DelGroup = objRow.Field<int>("CHG Ind"),
                        ShipIns = objRow.Field<string>("CHG Ind"),
                        SOTag1 = objRow.Field<string>("CHG Ind"),
                        SOTag2 = objRow.Field<string>("CHG Ind"),
                        POItemConfigurationText = objRow.Field<string>("CHG Ind"),
                        DelAppt = objRow.Field<string>("CHG Ind"),
                        DelSite = objRow.Field<string>("CHG Ind"),
                        Route = objRow.Field<int>("CHG Ind"),
                        NetPrice = objRow.Field<double>("CHG Ind"),
                        NetValue = objRow.Field<double>("CHG Ind"),
                        POTotal = objRow.Field<double>("CHG Ind"),
                        SrcLoc = objRow.Field<string>("CHG Ind")
                    });
            }
            catch(Exception objEx)
            {
                Logger.Error("Exception Thrown", objEx);
            }
        }
    }
}