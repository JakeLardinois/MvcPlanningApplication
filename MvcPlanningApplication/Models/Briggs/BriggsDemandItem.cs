using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;


namespace MvcPlanningApplication.Models.Briggs
{
    public class BriggsDemandItem
    {

        public BriggsDemandItem() : base()
        {
            ReleaseDate = SharedVariables.MINDATE;
            RcptDate1 = SharedVariables.MINDATE;
            RcptDate2 = SharedVariables.MINDATE;
            RcptDate3 = SharedVariables.MINDATE;
            RcptDate4 = SharedVariables.MINDATE;
            RcptDate5 = SharedVariables.MINDATE;
        }

        [Key]
        public virtual int ID { get; set; }

        public string Part { get; set; }
        public string Revision { get; set; }
        public string VendorPart { get; set; }
        public string Description { get; set; }
        public string PurchaseOrder { get; set; }
        public string Item { get; set; }
        public int Release { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Plant { get; set; }
        public int ShipQuantity { get; set; }
        public string UnitOfMeasure { get; set; }
        public int Week0ForecastQty { get; set; }
        public int Week1ForecastQty { get; set; }
        public int Week2ForecastQty { get; set; }
        public int Week3ForecastQty { get; set; }
        public int Week4ForecastQty { get; set; }
        public int Week5ForecastQty { get; set; }
        public int Week6ForecastQty { get; set; }
        public int Week7ForecastQty { get; set; }
        public int Week8ForecastQty { get; set; }
        public int Week9ForecastQty { get; set; }
        public int Week10ForecastQty { get; set; }
        public int Week11ForecastQty { get; set; }
        public int Week12ForecastQty { get; set; }
        public int Week13ForecastQty { get; set; }
        public int Week14ForecastQty { get; set; }
        public int Week15ForecastQty { get; set; }
        public int Week16ForecastQty { get; set; }
        public int Week17ForecastQty { get; set; }
        public int Month0ForecastQty { get; set; }
        public int Month1ForecastQty { get; set; }
        public int Quarter0ForecastQty { get; set; }
        public int Quarter1ForecastQty { get; set; }
        public int ShipWeeks { get; set; }
        public int FabWeeks { get; set; }
        public int RawWeeks { get; set; }
        public bool UseASN { get; set; }
        public int ASNsInTransit { get; set; }
        public int ASNQty { get; set; }
        public int PastDue { get; set; }
        public int CumulativeRcpt { get; set; }
        public int RcptQty1 { get; set; }
        public DateTime RcptDate1 { get; set; }
        public string RcptPackSlip1 { get; set; }
        public int RcptQty2 { get; set; }
        public DateTime RcptDate2 { get; set; }
        public string RcptPackSlip2 { get; set; }
        public int RcptQty3 { get; set; }
        public DateTime RcptDate3 { get; set; }
        public string RcptPackSlip3 { get; set; }
        public int RcptQty4 { get; set; }
        public DateTime RcptDate4 { get; set; }
        public string RcptPackSlip4 { get; set; }
        public int RcptQty5 { get; set; }
        public DateTime RcptDate5 { get; set; }
        public string RcptPackSlip5 { get; set; }
        public string CoreVendor { get; set; }
        public string CoreName { get; set; }
        public string OAVendor { get; set; }
        public string Name { get; set; }
        public string Name2 { get; set; }
        public string OAStreet { get; set; }
        public string OAAddress { get; set; }
        public string ShipName { get; set; }
        public string ShipName2 { get; set; }
        public string ShipStreet { get; set; }
        public string ShipAddress { get; set; }
        public string Company { get; set; }
        public string StoreLocation { get; set; }
        public string StoreBin { get; set; }
        public string ShipInstructions { get; set; }
        public string Buyer { get; set; }
        public string BuyerName { get; set; }
        public string BuyerTitle { get; set; }
        public string BuyerPhone { get; set; }
        public string BuyerFax { get; set; }
        public string BuyerEmail { get; set; }
        public string Controller { get; set; }
        public string PlannerName { get; set; }
        public string PlannerPhone { get; set; }
        public string Incoterm { get; set; }
        public string VendorContract { get; set; }

        public string Warehouse { get; set; }
        public string CustomerNo { get; set; }
        public decimal? WTFCurrentPrice { get; set; }
    }
}