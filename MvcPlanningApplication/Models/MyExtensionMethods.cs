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

        public static IList<HaworthOrder> RemainingOrders(this IList<HaworthOrder> source)
        {
            var objQueryDefinitions = new QueryDefinitions();
            StringBuilder objStrBldr = new StringBuilder();

            //then filter out for remaining orders search (below) as well...
            foreach (var objOrder in source)
                objStrBldr.Append("'" + objOrder.OrderNumber + "',");//get my list of PO's from the Haworth orders


            if (objStrBldr.Length > 0) //if there are haworth orders left to search in Syteline...
            {
                using (var SytelineDb = new SytelineDbEntities())
                {
                    //Here I get all the orders from Syteline that have the Haworth Order number in the PO field. Notice that I remove the last comma before passing the list of POs. 
                    var strSQL = objQueryDefinitions.GetQuery("SelectCustomerOrdersByPO", new string[] { objStrBldr.ToString(0, objStrBldr.Length - 1) });
                    var objCOItems = SytelineDb.Database.SqlQuery<COItem>(strSQL);
                    foreach (var objCOItem in objCOItems)//loop through the Syteline Orders and add the data they contain to my Haworth list
                    {
                        var objOrder = source  //get the haworth Order that has a matching WTF Order
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
            }



            return source //filters out the orders that are correctly entered in our system
                .Where(o => string.IsNullOrEmpty(o.WTFOrderNumber) ||
                    !o.WTFItemNumber.Trim().TrimStart('0').TrimEnd('0').Equals(o.ItemNumber.Trim().TrimStart('0').TrimEnd('0')) || //TrimStart('0').TrimEnd('0') remove leading & trailing zeros
                    o.WTFOrderQuantity != o.RequiredQty ||
                    o.WTFOrderDueDate.Date != o.DockDate)
                .ToList();
        }
    }
}