using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcPlanningApplication.Models
{
    public class DispatchJobMaterial
    {
        public string JobMaterial { get; set; }
        public string JobMaterialDescription { get; set; }
        public string UnitOfMeasure { get; set; }
        public decimal QtyRequired { get; set; }
        public decimal QtyIssued { get; set; }
        public decimal QtyAvailable { get; set; }
    }

}