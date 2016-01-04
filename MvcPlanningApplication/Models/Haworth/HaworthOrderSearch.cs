using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcPlanningApplication.Models.Haworth
{
    public class HaworthOrderSearch
    {
        public string[] Characteristics { get; set; }
        public DateTime ChangeDateGT { get; set; }
        public DateTime ChangeDateLT { get; set; }
        public string OrderNumber { get; set; }
        public string ItemNumber { get; set; }
        public string Description { get; set; }
        public string Description2 { get; set; }
        public string ColorCode { get; set; }
        public string ColorPattern { get; set; }
        public string ColorDescription { get; set; }
        public string[] StatusCode { get; set; }
        public DateTime DockDateGT { get; set; }
        public DateTime DockDateLT { get; set; }
        public DateTime ImportDateTimeGT { get; set; }
        public DateTime ImportDateTimeLT { get; set; }
    }
}