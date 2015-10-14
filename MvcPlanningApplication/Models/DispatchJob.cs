using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcPlanningApplication.Models
{
    public class DispatchJob
    {
        public string Job { get; set; }
        public Int16 JobSuffix { get; set; }
        public string OrderNumber { get; set; }
        public Int16 OrderLine { get; set; }
        public decimal QuantityOrdered { get; set; }
        public string ItemNumber { get; set; }
        public DateTime ShipByDate { get; set; }
        public DateTime DockDate { get; set; }
        public List<DispatchJobMaterial> DispatchJobMaterials { get; set; }

        public string Shell { get; set; }
        public string Frame { get; set; }
        public string Fabric { get; set; }
    }
}