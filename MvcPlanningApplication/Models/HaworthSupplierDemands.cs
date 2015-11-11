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


            foreach (var objRow in objCurrentDataTable.AsEnumerable())
                try
                {
                    this.Add(new HaworthSupplierDemand
                    {
                        CHGInd = objRow.Field<string>("CHG Ind"),
                        MAD = DateTime.TryParse(objRow["MAD"] + string.Empty, out dtmTemp) ? dtmTemp : SharedVariables.MINDATE,
                        SOCrDte = DateTime.TryParse(objRow["SO Cr. Dte"] + string.Empty, out dtmTemp) ? dtmTemp : SharedVariables.MINDATE,
                        SONo = objRow.Field<string>("SO No."),
                        Item1 = objRow.Field<string>("Item1"),
                        SOQty = int.TryParse(objRow["SO Qty"] + string.Empty, out intTemp) ? intTemp : 0,
                        PONumber1 = objRow.Field<string>("PO Number"),
                        Item2 = objRow.Field<string>("Item2"),
                        POQty = int.TryParse(objRow["PO Qty"] + string.Empty, out intTemp) ? intTemp : 0,
                        MatNo = objRow.Field<string>("Mat. No."),
                        Description = objRow.Field<string>("Description"),
                        CatalogPartNumber = objRow.Field<string>("Catalog Part Number"),
                        CatalogPartDescription = objRow.Field<string>("Catalog Part Description"),
                        PlnText = objRow.Field<string>("Pln. Text"),
                        Customer = objRow.Field<string>("Customer"),
                        Name = objRow.Field<string>("Name"),
                        Street = objRow.Field<string>("Street"),
                        ShipToCity = objRow.Field<string>("Ship To City"),
                        Region = objRow.Field<string>("Region"),
                        ShToPos = objRow.Field<string>("Sh. To Pos"),
                        Country = objRow.Field<string>("Country"),
                        ShipPnt = int.TryParse(objRow["Ship Pnt."] + string.Empty, out intTemp) ? intTemp : 0,
                        SNNo = objRow.Field<string>("SN No."),
                        OrigSO = objRow.Field<string>("Orig. SO"),
                        PONumber2 = objRow.Field<string>("PO number"),
                        Usage = objRow.Field<string>("Usage"),
                        ReasonRej = objRow.Field<string>("Reason Rej"),
                        DelDate = DateTime.TryParse(objRow["Del. Date"] + string.Empty, out dtmTemp) ? dtmTemp : SharedVariables.MINDATE,
                        DelGroup = int.TryParse(objRow["Del. Group"] + string.Empty, out intTemp) ? intTemp : 0,
                        ShipIns = objRow.Field<string>("Ship Ins."),
                        SOTag1 = objRow.Field<string>("SO Tag1"),
                        SOTag2 = objRow.Field<string>("SO Tag 2"),
                        POItemConfigurationText = objRow.Field<string>("PO Item Configuration Text"),
                        DelAppt = objRow.Field<string>("Del Appt"),
                        DelSite = objRow.Field<string>("Del Site"),
                        Route = int.TryParse(objRow["Route"] + string.Empty, out intTemp) ? intTemp : 0,
                        NetPrice = double.TryParse(objRow["Net price"] + string.Empty, out dblTemp) ? dblTemp : 0,
                        NetValue = double.TryParse(objRow["Net Value"] + string.Empty, out dblTemp) ? dblTemp : 0,
                        POTotal = double.TryParse(objRow["PO Total"] + string.Empty, out dblTemp) ? dblTemp : 0,
                        SrcLoc = objRow.Field<string>("Src. Loc.")
                    });
                }
                catch (Exception objEx)
                {
                    Logger.Error("Haworth Supplier Demand Exception Thrown in Populate()", objEx);
                }
            
        }
    }
}