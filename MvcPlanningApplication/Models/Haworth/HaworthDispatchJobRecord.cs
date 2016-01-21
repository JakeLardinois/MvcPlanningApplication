using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcPlanningApplication.Models.Haworth
{
    public class HaworthDispatchJobRecord : IHaworthDispatchJob
    {
        public virtual int ID { get; set; }

        public string JobOrder { get; set; }
        public string CustomerOrder { get; set; }
        public string PurchaseOrder { get; set; }
        public string SalesOrder { get; set; }
        public decimal QuantityOrdered { get; set; }
        public decimal QuantityRemaining { get; set; }
        public string ItemNumber { get; set; }
        public DateTime ShipByDate { get; set; }
        public DateTime DockDate { get; set; }

        public string Shell { get; set; }
        public string Frame { get; set; }
        public string Fabric { get; set; }
        public string ArmCaps { get; set; }
    }
}