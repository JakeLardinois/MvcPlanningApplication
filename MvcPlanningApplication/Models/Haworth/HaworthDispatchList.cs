using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Text;


namespace MvcPlanningApplication.Models.Haworth
{
    public class HaworthDispatchList : List<HaworthDispatchJob>
    {
        public HaworthDispatchList()
            : base()
        {
            Populate();
        }

        public void Populate()
        {
            var db = new SytelineDbEntities();
            var objQueryDefinitions = new QueryDefinitions();
            var objStrBldrSQL = new StringBuilder();

            objStrBldrSQL.Append(objQueryDefinitions.GetQuery("SelectCOItemByCustNumListAndStatus", 
                new string[] { "3417".AddSingleQuotesAndPadLeft(7), "O" }));

            this.AddRange(db.Database
                .SqlQuery<coitem>(objStrBldrSQL.ToString())
                .Select(g => new HaworthDispatchJob { 
                    Job = g.ref_num,
                    JobSuffix = g.ref_line_suf.HasValue ? (short)g.ref_line_suf : (short)0,
                    OrderNumber = g.co_num,
                    OrderLine = g.co_line,
                    QuantityOrdered = g.qty_ordered,
                    ItemNumber = g.item,
                    DockDate = g.due_date.HasValue? (DateTime)g.due_date:SharedVariables.MINDATE,
                    ShipByDate = g.promise_date.HasValue ? (DateTime)g.promise_date : SharedVariables.MINDATE
                }
                ));

            foreach (var objDispatchJob in this)
            {
                objStrBldrSQL.Clear();
                objStrBldrSQL.Append(objQueryDefinitions.GetQuery("SpJobPickListByJobAndSuffix", 
                    new string[] { objDispatchJob.Job, objDispatchJob.JobSuffix.ToString() }));

                objDispatchJob.DispatchJobMaterials = db.Database
                    .SqlQuery<SpJobPickListMaterial>(objStrBldrSQL.ToString())
                    .Select(g => new HaworthDispatchJobMaterial
                    {
                        JobMaterial = string.IsNullOrEmpty(g.det_JobMatlItem) ? " " : g.det_JobMatlItem,
                        JobMaterialDescription = g.det_JobMatlDesciption,
                        UnitOfMeasure = g.det_JobMatlU_M,
                        QtyRequired = g.det_TotalRequired ?? 0,
                        QtyIssued = g.det_JobMatlQtyIssued ?? 0,
                        QtyAvailable = g.det_QtyAvailable ?? 0
                    }).ToList();
            }
        }

    }
}