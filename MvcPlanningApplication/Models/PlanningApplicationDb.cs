using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.Entity;
using MvcPlanningApplication.Models.Haworth;
using MvcPlanningApplication.Models.Briggs;


namespace MvcPlanningApplication.Models
{
    public class PlanningApplicationDb : DbContext
    {
        public DbSet<BriggsDemandItem> BriggsDemandItems { get; set; }
        public DbSet<HaworthOrder> HaworthOrders { get; set; }
        public DbSet<HaworthSupplierDemand> HaworthSupplierDemands { get; set; }
        public DbSet<HaworthOrderCharacteristic> HaworthOrderCharacteristics { get; set; }
        public DbSet<HaworthDispatchJobRecord> HaworthDispatchJobRecords { get; set; }
    }
}