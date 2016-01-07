﻿using System;
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

        //'^' denotes the begining of string and '$' denotes the end of string; I am stating I want to match 
        private static string mstrFrame5CharactersRegEx { get { return "^[0-9][0-9][0-9][0-9][0-9]$"; } }
        private static string mstrFrame8CharactersRegEx { get { return "^[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]$"; } }
        private static string mstrShellRegEx { get { return "^TR-"; } }
        private static string mstrSeatFabricRegEx { get { return "^MIS SEAT COVER "; } }
        private static string mstrBackFabricRegEx { get { return "^MIS BACK COVER "; } }

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
        }

        public string ArmCaps
        {
            get
            {
                return string.Empty;
            }
        }
    }
}