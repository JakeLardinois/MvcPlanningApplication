using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Text.RegularExpressions;
using System.Text;


namespace MvcPlanningApplication.Models.Haworth
{
    public class HaworthDispatchJob : IHaworthDispatchJob
    {
        public string JobOrder {
            get { return Job + "." + JobSuffix; }
            set { }
        }
        public string Job { get; set; }
        public Int16 JobSuffix { get; set; }
        public string CustomerOrder {
            get { return co_num + "." + co_line; }
            set { }
        }
        public string co_num { get; set; }
        public short co_line { get; set; }
        public string PurchaseOrder { get; set; }
        public string cust_po { get; set; }
        public string SalesOrder { get; set; }
        public decimal QuantityOrdered { get; set; }
        public decimal QuantityRemaining { get; set; }
        public string ItemNumber { get; set; }
        public DateTime ShipByDate { get; set; }
        public DateTime DockDate { get; set; }
        public List<HaworthDispatchJobMaterial> DispatchJobMaterials { get; set; }

        //'^' denotes the begining of string and '$' denotes the end of string; I am stating I want to match 
        private static string mstrFrame5CharactersRegEx { get { return "^[0-9][0-9][0-9][0-9][0-9]$"; } }
        private static string mstrFrame8CharactersRegEx { get { return "^[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]$"; } }
        private static string mstrShellRegEx { get { return "^TR-"; } }
        private static string mstrSeatFabricRegEx { get { return "^MIS SEAT COVER "; } }
        private static string mstrBackFabricRegEx { get { return "^MIS BACK COVER "; } }
        private static string mstrArmCapRegEx { get { return "^MIS 5([7][7-9]|[8][0])"; } }

        public string Shell {
            get {
                if (DispatchJobMaterials != null)
                    return DispatchJobMaterials
                        .Where(m => Regex.Match(m.JobMaterial, mstrShellRegEx, RegexOptions.IgnoreCase).Success)
                        .DefaultIfEmpty(new HaworthDispatchJobMaterial { JobMaterial = "None" })
                        .FirstOrDefault().JobMaterial;
                else
                    return "None";
            }
            set { }
        }

        public string Frame {
            get {
                if (DispatchJobMaterials != null)
                    return DispatchJobMaterials
                        .Where(m => Regex.Match(m.JobMaterial, mstrFrame5CharactersRegEx).Success || 
                            Regex.Match(m.JobMaterial, mstrFrame8CharactersRegEx).Success)
                        .DefaultIfEmpty(new HaworthDispatchJobMaterial { JobMaterial = "None" })
                        .FirstOrDefault().JobMaterial;
                else
                    return "None";
            }
            set { }
        }

        public string Fabric {
            get
            {
                if (DispatchJobMaterials != null)
                    return DispatchJobMaterials
                        .Where(m => Regex.Match(m.JobMaterial, mstrSeatFabricRegEx, RegexOptions.IgnoreCase).Success || 
                            Regex.Match(m.JobMaterial, mstrBackFabricRegEx, RegexOptions.IgnoreCase).Success)
                        .DefaultIfEmpty(new HaworthDispatchJobMaterial { JobMaterial = "None" })
                        .Aggregate(new StringBuilder(), (a, b) => a.Append(b.JobMaterial).Append("<br/>")).ToString();
                else
                    return "None";
            }
            set { }
        }

        public string ArmCaps
        {
            get
            {
                if (DispatchJobMaterials != null)
                    return DispatchJobMaterials
                        .Where(m => Regex.Match(m.JobMaterial, mstrArmCapRegEx, RegexOptions.IgnoreCase).Success)
                        .DefaultIfEmpty(new HaworthDispatchJobMaterial { JobMaterial = "None" })
                        .Aggregate(new StringBuilder(), (a, b) => a.Append(b.JobMaterial).Append("<br/>")).ToString();
                else
                    return "None";
            }
            set { }
        }
    }
}