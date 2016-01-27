using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcPlanningApplication.Models.Haworth
{
    public class HaworthDispatchJobSearch
    {
        public string JobOrder { get; set; }
        public string CustomerOrder { get; set; }
        public string PurchaseOrder { get; set; }
        public string SalesOrder { get; set; }
        public string ItemNumber { get; set; }
        public string Shell { get; set; }
        public string Frame { get; set; }
        public string Fabric { get; set; }
        public string ArmCaps { get; set; }
        public DateTime ShipByDateGT { get; set; }
        public DateTime ShipByDateLT { get; set; }
        public DateTime DockDateGT { get; set; }
        public DateTime DockDateLT { get; set; }
    }
}