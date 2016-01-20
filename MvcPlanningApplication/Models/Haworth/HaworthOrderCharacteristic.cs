using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations.Schema;

namespace MvcPlanningApplication.Models.Haworth
{
    public class HaworthOrderCharacteristic
    {
        public virtual int ID { get; set; }

        public virtual HaworthOrder HaworthOrder { get; set; }//sets up an Entity Framework foreign key

        public string OrderNumber { get; set; }
        public string Characteristic { get; set; }
        public string Value { get; set; }
        public string CharacteristicKey { get; set; }
    }
}