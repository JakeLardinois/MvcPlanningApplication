using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;


namespace MvcPlanningApplication.Models
{
    public class HaworthSupplierDemand
    {
        [Key]
        public virtual int ID { get; set; }

        public string CHGInd { get; set; }
        public DateTime MAD { get; set; }
        public DateTime SOCrDte{ get; set; }
        public string SONo { get; set; }
        public string Item1 { get; set; }
        public int SOQty { get; set; }
        public string PONumber1 { get; set; }
        public string Item2 { get; set; }
        public int POQty { get; set; }
        public string MatNo { get; set; }
        public string Description { get; set; }
        public string CatalogPartNumber { get; set; }
        public string CatalogPartDescription { get; set; }
        public string PlnText { get; set; }
        public string Customer { get; set; }
        public string Name { get; set; }
        public string Street { get; set; }
        public string ShipToCity { get; set; }
        public string Region { get; set; }
        public string ShToPos { get; set; }
        public string Country { get; set; }
        public int ShipPnt { get; set; }
        public string SNNo { get; set; }
        public string OrigSO { get; set; }
        public string PONumber2 { get; set; }
        public string Usage { get; set; }
        public string ReasonRej { get; set; }
        public DateTime DelDate { get; set; }
        public int DelGroup { get; set; }
        public string ShipIns { get; set; }
        public string SOTag1 { get; set; }
        public string SOTag2 { get; set; }
        public string POItemConfigurationText { get; set; }
        public string DelAppt { get; set; }
        public string DelSite { get; set; }
        public int Route { get; set; }
        public double NetPrice { get; set; }
        public double NetValue { get; set; }
        public double POTotal { get; set; }
        public string SrcLoc { get; set; }
    }
}