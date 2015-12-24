using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;


namespace MvcPlanningApplication.Models.Haworth
{
    public class HaworthSupplierDemand
    {
        [Key]
        public virtual int ID { get; set; }
        public string OrderNumber {
            get {

                if (!string.IsNullOrEmpty(PONumber) && !string.IsNullOrEmpty(POLine))
                    return PONumber + "." + POLine.PadLeft(5, '0') + ".0001";
                return string.Empty;
            }
            protected set { } //EF requires a property to have both a getter and setter to store value in the database...
        }


        public string PONumber { get; set; }
        public string POLine { get; set; }
        public string POItemConfigurationText { get; set; }
    }
}