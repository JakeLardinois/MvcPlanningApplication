using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcPlanningApplication.Models.Haworth
{
    public class HaworthDispatchJobSearch
    {
        public string Job { get; set; }
        public string OrderNumber { get; set; }
        public string ItemNumber { get; set; }
        public string Shell { get; set; }
        public string Frame { get; set; }
        public string Fabric { get; set; }
        public string ShipByDate { get; set; }
        public string DockDate { get; set; }
    }
}