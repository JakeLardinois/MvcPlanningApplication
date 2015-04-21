using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.Entity;


namespace MvcPlanningApplication.Models
{
    public class PlanningApplicationDb : DbContext
    {
        public DbSet<BriggsDemandItem> BriggsDemandItems { get; set; }
    }
}