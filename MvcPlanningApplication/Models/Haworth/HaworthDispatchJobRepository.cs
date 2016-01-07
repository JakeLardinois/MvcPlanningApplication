using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Collections.ObjectModel;
using System.Text;
using MvcPlanningApplication.Models.DataTablesMVC;


namespace MvcPlanningApplication.Models.Haworth
{
    public class HaworthDispatchJobRepository
    {
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
                objStrBldr.Clear();
                objStrBldr.Append(objQueryDefinitions.GetQuery("SelectCOItemByCustNumListAndStatus",
                new string[] { "3417".AddSingleQuotesAndPadLeft(7), "O" }));

                //Do your filtering here using the above objHaworthDispatchJobSearch object...
                orders = db.Database
                    .SqlQuery<coitem>(objStrBldr.ToString())
                    .Select(g => new HaworthDispatchJob
                    {
                        Job = g.ref_num,
                        JobSuffix = g.ref_line_suf.HasValue ? (short)g.ref_line_suf : (short)0,
                        OrderNumber = g.co_num,
                        OrderLine = g.co_line,
                        QuantityOrdered = g.qty_ordered,
                        ItemNumber = g.item,
                        DockDate = g.due_date.HasValue ? (DateTime)g.due_date : SharedVariables.MINDATE,
                        ShipByDate = g.promise_date.HasValue ? (DateTime)g.promise_date : SharedVariables.MINDATE
                    })
                    .ToList(); //must call toList() or else your .Select doesn't make your objects

                //Create a comma separated list of jobs
                objStrBldr.Clear();
                foreach (var objJob in orders)
                {
                    if (!string.IsNullOrEmpty(objJob.Job))
                        objStrBldr.Append(objJob.Job + ",");
                }

                if (objStrBldr.Length > 0)
                {
                    var StrJobList = objStrBldr
                    .Remove(objStrBldr.Length - 1, 1) //removes the last comma
                    .ToString()
                    .AddSingleQuotes();

                    //Use the above job list to create your SQL
                    objStrBldr.Clear();
                    objStrBldr.Append(objQueryDefinitions //because I am only lookup up jobmatl by job, I will grab other materials from the Indented BOM...
                        .GetQuery("SelectJobMatlByJobList", new string[] { StrJobList }));

                    //Get the complete list of job materials for the above created job list
                    var JobMaterials = db.Database
                        .SqlQuery<jobmatl>(objStrBldr.ToString())
                        .ToList();

                    //iterate through the job collection and add the matching job materials for the respective job
                    foreach (var objDispatchJob in orders)
                    {
                        objDispatchJob.DispatchJobMaterials = JobMaterials
                            .Where(j => j.job.Equals(objDispatchJob.Job) && j.matl_type.Equals("M"))
                            .Select(g => new HaworthDispatchJobMaterial
                            {
                                JobMaterial = g.item,
                                JobMaterialDescription = g.description,
                                UnitOfMeasure = g.u_m
                                //QtyRequired = g.matl_qty * objDispatchJob.QuantityOrdered,
                                //QtyIssued = g.qty_issued * objDispatchJob.QuantityOrdered,
                                //QtyAvailable = g.det_QtyAvailable ?? 0
                            })
                            .ToList();// since .Select is lazy, call the ToList()
                    }
                }
                




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