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
            var objStrBldr = new StringBuilder();


            objStrBldr.Append(objQueryDefinitions.GetQuery("SelectCOItemByCustNumListAndStatus", 
                new string[] { "3417".AddSingleQuotesAndPadLeft(7), "O" }));

            this.AddRange(db.Database
                .SqlQuery<coitem>(objStrBldr.ToString())
                .Select(g => new HaworthDispatchJob { 
                    Job = g.ref_num,
                    JobSuffix = g.ref_line_suf.HasValue ? (short)g.ref_line_suf : (short)0,
                    OrderNumber = g.co_num,
                    OrderLine = g.co_line,
                    QuantityOrdered = g.qty_ordered,
                    ItemNumber = g.item,
                    DockDate = g.due_date.HasValue? (DateTime)g.due_date:SharedVariables.MINDATE,
                    ShipByDate = g.promise_date.HasValue ? (DateTime)g.promise_date : SharedVariables.MINDATE
                }));

            objStrBldr.Clear();
            foreach (var objJob in this)
            {
                if (!string.IsNullOrEmpty(objJob.Job))
                    objStrBldr.Append(objJob.Job + ",");
            }
            var StrJobList = objStrBldr
                .Remove(objStrBldr.Length - 1, 1) //removes the last comma
                .ToString()
                .AddSingleQuotes();

            objStrBldr.Clear();
            objStrBldr.Append(objQueryDefinitions //because I am only lookup up jobmatl by job, I will grab other materials from the Indented BOM...
                .GetQuery("SelectJobMatlByJobList", new string[] {StrJobList}));

            var JobMaterials = db.Database
                .SqlQuery<jobmatl>(objStrBldr.ToString())
                .ToList();

            foreach (var objDispatchJob in this)
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
                    .ToList();
            }
        }

    }
}