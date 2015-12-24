using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcPlanningApplication.Models.Haworth
{
    public class HaworthDeliveryInformation
    {
        public string PlantCode { get; set; }
        public string PlantName { get; set; }
        public string PlantName2 { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
    }
}