using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Collections.ObjectModel;
using System.Text;
using MvcPlanningApplication.Models.DataTablesMVC;


namespace MvcPlanningApplication.Models.Haworth
{
    public class HaworthOrdersRepository
    {
        public IList<HaworthOrder> GetOrders(out int searchRecordCount, JQueryDataTablesModel DataTablesModel, bool isDownloadReport = false)
        {
            ReadOnlyCollection<SortedColumn> sortedColumns = DataTablesModel.GetSortedColumns();
            IEnumerable<HaworthOrder> orders;
            DateTime dtmTemp;
            int intTemp;
            string[] objResults;
            string strEmptyString = "EMPTY";
            StringBuilder objStrBldr = new StringBuilder();
            var objQueryDefinitions = new QueryDefinitions();


            var objHaworthOrderSearch = new HaworthOrderSearch();
            for (int intCounter = 0; intCounter < DataTablesModel.iColumns; intCounter++)
            {

                if (DataTablesModel.bSearchable_[intCounter] == true && !string.IsNullOrEmpty(DataTablesModel.sSearch_[intCounter]))
                {
                    /*For some reason when I implemented resizable movable columns and would then move the columns in the application, the application would send tilde's in the 'checkbox' column types sSearch field which was wierd
                     since the checkbox column types are delimited by the pipe | character and the 'range' column types are delimited by the tilde...  The resolution that I came up with was to check if the only value passed in sSearch
                     was a tilde and if it was then skip the loop so that the respective VendorRequestSearch field was left null.*/
                    if (DataTablesModel.sSearch_[intCounter].Equals("~"))
                        continue;

                    /*Notice that i had to use mDataProp2_ due to datatables multi-column filtering not placing sSearch into proper array position when columns are reordered; See VendorRequestsController.cs Search method for details...*/
                    switch (DataTablesModel.mDataProp2_[intCounter])
                    {
                        default:
                            break;
                    }
                }
            }


            using (var db = new PlanningApplicationDb())
            {
                //Do your searching here based on the objHaworthOrderSearch object...
                orders = db.HaworthOrders;


                //then filter out for remaining orders search (below) as well...
                foreach (var objOrder in orders)
                    objStrBldr.Append("'" + objOrder.OrderNumber + "',");//get my list of PO's from the Haworth orders

                //Here I get all the orders from Syteline that have the Haworth Order number in the PO field. Notice that I remove the last comma before passing the list of POs. 
                var strSQL = objQueryDefinitions.GetQuery("SelectCustomerOrdersByPO", new string[] { objStrBldr.ToString(0, objStrBldr.Length - 1) });

                using (var SytelineDb = new SytelineDbEntities())
                {
                    var objCOItems = SytelineDb.Database.SqlQuery<COItem>(strSQL);
                    foreach (var objCOItem in objCOItems)//loop through the Syteline Orders and add the data they contain to my Haworth list
                    {
                        var objOrder = orders  //get the haworth Order that has a matching WTF Order
                            .Where(o => o.OrderNumber.Trim().ToUpper().Equals(objCOItem.cust_po.Trim().ToUpper()))
                            .FirstOrDefault();

                        //populate the haworth order with WTF Order Number, Item number on the WTF Order
                        objOrder.WTFOrderNumber = objCOItem.co_num;
                        objOrder.WTFItemNumber = objCOItem.item;
                        objOrder.WTFOrderQuantity = (double)objCOItem.qty_ordered;
                        objOrder.WTFOrderDueDate = objCOItem.due_date ?? DateTime.MinValue;
                        objOrder.WTFOrderRequestDate = objCOItem.promise_date ?? DateTime.MinValue;
                    }
                }
                

                orders = orders //filters out the orders that are correctly entered in our system
                    .Where(o => string.IsNullOrEmpty(o.WTFOrderNumber) ||
                        !o.WTFItemNumber.Trim().TrimStart('0').TrimEnd('0').Equals(o.ItemNumber.Trim().TrimStart('0').TrimEnd('0')) || //TrimStart('0').TrimEnd('0') remove leading & trailing zeros
                        o.WTFOrderQuantity != o.RequiredQty ||
                        o.WTFOrderDueDate.Date != o.DockDate)
                    .ToList();


                //needed this to get the proper pagination values. by adding it here, i was hoping to optomize performance and still leverage deferred execution with the above queries
                // and the take values below...
                searchRecordCount = orders.Count();


                IEnumerable<HaworthOrder> obj;
                if (isDownloadReport)
                    obj = orders
                        .ToList();
                else
                    obj = orders
                        .Skip(DataTablesModel.iDisplayStart)
                        .Take(DataTablesModel.iDisplayLength)
                        .ToList();


                return obj.ToList();
                    
            }
        }

    }
}