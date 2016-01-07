using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcPlanningApplication.Models.Haworth
{
    public class HaworthOrderCharacteristic
    {
        [Key]
        public virtual int ID { get; set; }

        public string OrderNumber { get; set; }
        public string Characteristic { get; set; }
        public string Value { get; set; }
    }
}