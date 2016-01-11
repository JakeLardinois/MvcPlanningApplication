﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Collections.ObjectModel;
using System.Text;
using MvcPlanningApplication.Models.DataTablesMVC;
using log4net;


namespace MvcPlanningApplication.Models.Haworth
{
    public class HaworthDispatchJobRepository
    {
        private static readonly ILog Logger = LogHelper.GetLogger();


        public IList<HaworthDispatchJob> GetOrders(out int searchRecordCount, JQueryDataTablesModel DataTablesModel, bool isDownloadReport = false)
        {
            ReadOnlyCollection<SortedColumn> sortedColumns = DataTablesModel.GetSortedColumns();
            IEnumerable<HaworthDispatchJob> orders;
            DateTime dtmTemp;
            int intTemp;
            string[] objResults;
            string strEmptyString = "EMPTY";
            StringBuilder objStrBldr = new StringBuilder();
            var objQueryDefinitions = new QueryDefinitions();

            Logger.Debug("Fired 1");
            var objHaworthDispatchJobSearch = new HaworthDispatchJobSearch();
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

            using (var db = new SytelineDbEntities())
            {
                using (var db2 = new PlanningApplicationDb())
                {
                    //Do your filtering here using the above objHaworthDispatchJobSearch object...
                    orders = db.Database.SqlQuery<COItem>(objQueryDefinitions.GetQuery("SelectCOItemByCustNumListAndStatus", new string[] { "3417".AddSingleQuotesAndPadLeft(7), "O" }))
                        .Select(g => new HaworthDispatchJob
                        {
                            Job = g.ref_num,
                            JobSuffix = g.ref_line_suf.HasValue ? (short)g.ref_line_suf : (short)0,
                            co_num = g.co_num,
                            co_line = g.co_line,
                            cust_po = g.cust_po,
                            QuantityOrdered = g.qty_ordered,
                            PurchaseOrder = g.PurchaseOrder,
                            SalesOrder = db2.HaworthSupplierDemands
                                .Where(s => !string.IsNullOrEmpty(s.OrderNumber) && s.OrderNumber.Equals(s.OrderNumber))
                                .FirstOrDefault()
                                .SOrderNumber,
                            ItemNumber = g.item,
                            DockDate = g.due_date.HasValue ? (DateTime)g.due_date : SharedVariables.MINDATE,
                            ShipByDate = g.promise_date.HasValue ? (DateTime)g.promise_date : SharedVariables.MINDATE,
                            DispatchJobMaterials = db.jobmatls
                                .Where(m => m.job.Equals(g.ref_num))
                                .Select(jm => new HaworthDispatchJobMaterial
                                {
                                    JobMaterial = jm.item,
                                    JobMaterialDescription = jm.description,
                                    UnitOfMeasure = jm.u_m
                                    //QtyRequired = g.matl_qty * objDispatchJob.QuantityOrdered,
                                    //QtyIssued = g.qty_issued * objDispatchJob.QuantityOrdered,
                                    //QtyAvailable = g.det_QtyAvailable ?? 0
                                })
                                .ToList()
                        });

                    //needed this to get the proper pagination values. by adding it here, i was hoping to optomize performance and still leverage deferred execution with the above queries
                    // and the take values below...
                    searchRecordCount = orders.Count();

                    
                    IEnumerable<HaworthDispatchJob> obj;
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
}