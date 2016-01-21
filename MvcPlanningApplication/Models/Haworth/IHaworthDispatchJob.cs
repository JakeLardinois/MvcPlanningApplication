using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcPlanningApplication.Models.Haworth
{
    public interface IHaworthDispatchJob
    {
        string JobOrder { get; set; }
        string CustomerOrder { get; set; }
        string PurchaseOrder { get; set; }
        string SalesOrder { get; set; }
        decimal QuantityOrdered { get; set; }
        decimal QuantityRemaining { get; set; }
        string ItemNumber { get; set; }
        DateTime ShipByDate { get; set; }
        DateTime DockDate { get; set; }

        string Shell { get; set; }
        string Frame { get; set; }
        string Fabric { get; set; }
        string ArmCaps { get; set; }
    }
}