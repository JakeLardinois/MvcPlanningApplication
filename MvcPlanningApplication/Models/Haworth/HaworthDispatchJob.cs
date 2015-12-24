using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Text.RegularExpressions;
using System.Text;


namespace MvcPlanningApplication.Models.Haworth
{
    public class HaworthDispatchJob
    {
        public string Job { get; set; }
        public Int16 JobSuffix { get; set; }
        public string OrderNumber { get; set; }
        public Int16 OrderLine { get; set; }
        public decimal QuantityOrdered { get; set; }
        public string ItemNumber { get; set; }
        public DateTime ShipByDate { get; set; }
        public DateTime DockDate { get; set; }
        public List<HaworthDispatchJobMaterial> DispatchJobMaterials { get; set; }

        public string Shell {
            get {
                if (DispatchJobMaterials != null)
                    return DispatchJobMaterials
                        .Where(m => m.JobMaterial.ToUpper().Contains("MIS 544-"))
                        .DefaultIfEmpty(new HaworthDispatchJobMaterial { JobMaterial = "None" })
                        .FirstOrDefault().JobMaterial;
                else
                    return "None";
            } 
        }

        public string Frame {
            get {
                if (DispatchJobMaterials != null)
                    return DispatchJobMaterials //'^' denotes the begining of string and '$' denotes the end of string; I am stating I want to match
                        .Where(m => Regex.Match(m.JobMaterial, "^[0-9][0-9][0-9][0-9][0-9]$").Success) // a 5 character number
                        .DefaultIfEmpty(new HaworthDispatchJobMaterial { JobMaterial = "None" })
                        .FirstOrDefault().JobMaterial;
                else
                    return "None";
            } 
        }

        public string Fabric {
            get
            {
                if (DispatchJobMaterials != null)
                    return DispatchJobMaterials
                        .Where(m => m.JobMaterial.ToUpper().Contains("MIS S") || m.JobMaterial.ToUpper().Contains("MIS B"))
                        .DefaultIfEmpty(new HaworthDispatchJobMaterial { JobMaterial = "None" })
                        .Aggregate(new StringBuilder(), (a, b) => a.Append(b.JobMaterial).Append("  ")).ToString();
                else
                    return "None";
            }
        }
    }
}