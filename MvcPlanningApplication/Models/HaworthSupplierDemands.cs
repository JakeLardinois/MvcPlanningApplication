using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;


namespace MvcPlanningApplication.Models
{
    public class HaworthSupplierDemands : List<HaworthSupplierDemand>
    {
        public DataTable CurrentDataTable { get; set; }


        public HaworthSupplierDemands(string ExcelFileNameAndLocation, string ExcelRangeName)
            :base()
        {
            //ExcelInfo objExcelInfo = new ExcelInfo(SelectedFile);
            //objExcelInfo.GetInformation();
            DataSet objDataSet = ExcelInfo.GetDataFromExcel(ExcelFileNameAndLocation, ExcelRangeName);
            CurrentDataTable = objDataSet.Tables[0];

            Populate();
        }

        private void Populate()
        {
            foreach (var objRow in CurrentDataTable.AsEnumerable())
            {
                this.Add(new HaworthSupplierDemand { 
                    CHGInd = objRow.Field<string>("CHG Ind") 
                });
            }
        }
    }
}