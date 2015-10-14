using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Text;


namespace MvcPlanningApplication.Models
{
    public class HaworthDispatchList : List<DispatchJob>
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

            objStrBldrSQL.Append(objQueryDefinitions.GetQuery("SelectHaworthAssemblyDispatch"));
            this.AddRange(db.Database.SqlQuery<DispatchJob>(objStrBldrSQL.ToString()));

            foreach (var objDispatchJob in this)
            {
                objStrBldrSQL.Clear();
                objStrBldrSQL.Append(objQueryDefinitions.GetQuery("SpHaworthAssemblyDispatchMaterials", 
                    new string[] { objDispatchJob.Job, objDispatchJob.JobSuffix.ToString(), objDispatchJob.ItemNumber }));

                objDispatchJob.DispatchJobMaterials = db.Database
                    .SqlQuery<SpJobPickListMaterial>(objStrBldrSQL.ToString())
                    .Select(g => new DispatchJobMaterial
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