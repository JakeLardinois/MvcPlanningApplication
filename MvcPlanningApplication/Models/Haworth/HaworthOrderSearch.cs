using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcPlanningApplication.Models.Haworth
{
    public class HaworthOrderSearch
    {
        public string Characteristics { get; set; }
        public string ChangeDate { get; set; }
        public string OrderNumber { get; set; }
        public string ItemNumber { get; set; }
        public string Description { get; set; }
        public string Description2 { get; set; }
        public string ColorCode { get; set; }
        public string ColorPattern { get; set; }
        public string ColorDescription { get; set; }
        public string StatusCodes { get; set; }
        public string DockDate { get; set; }
        public string ImportDateTime { get; set; }
    }
}