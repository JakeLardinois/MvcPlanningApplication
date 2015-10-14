using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcPlanningApplication.Models
{
    public class SpJobPickListMaterial
    {
        public string det_JobMatlItem { get; set; }
        public string det_JobMatlDesciption { get; set; }
        public string det_JobMatlU_M { get; set; }
        public decimal? det_TotalRequired { get; set; }
        public decimal? det_JobMatlQtyIssued { get; set; }
        public decimal? det_QtyAvailable { get; set; }
    }
}