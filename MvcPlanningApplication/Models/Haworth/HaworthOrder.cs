using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;


namespace MvcPlanningApplication.Models.Haworth
{
    public class HaworthOrder
    {

        public HaworthOrder()
        {
            WTFOrderRequestDate = SharedVariables.MINDATE;
            WTFOrderDueDate = SharedVariables.MINDATE;
            ChangeDate = SharedVariables.MINDATE;
            DockDate = SharedVariables.MINDATE;
            MaintenanceDateTime = SharedVariables.MINDATE;
            ImportDateTime = SharedVariables.MINDATE;
        }


        [Key]
        public virtual int ID { get; set; }

        private double mUnitPrice { get; set; }

        public string FileName { get; set; }
        public string File { get; set; }

        public string Type { get; set; }
        public string OrderNumber { get; set; }
        public string WTFOrderNumber { get; set; }

        public string WTFItemNumber { get; set; }
        public double WTFOrderQuantity { get; set; }
        public DateTime WTFOrderRequestDate { get; set; }
        public DateTime WTFOrderDueDate { get; set; }//This is the DockDate

        public string VariantName { get; set; }
        public string OrgCode { get; set; }
        public string OrgName { get; set; }
        public string StatusCode { get; set; }
        public DateTime ChangeDate { get; set; }
        public string BuyerID { get; set; }
        public string PlannerID { get; set; }
        public DateTime DockDate { get; set; }
        public string SupplierNumber { get; set; }
        public string SupplierName { get; set; }
        public HaworthPartInformation PartInformation { get; set; }
        public string ItemNumber { get; set; }
        public double RequiredQty { get; set; }
        public double ReceivedQty { get; set; }
        public string UnitOfMeasure { get; set; }
        public int ModifiedUnitOfMeasure
        {
            get
            {
                int intTemp;

                return string.IsNullOrEmpty(UnitOfMeasure) ? -1 : 
                    int.TryParse(UnitOfMeasure.Replace("PER ", string.Empty).Replace(" EA", string.Empty), out intTemp) ? intTemp : -1;
            }
        }
        public double UnitPrice
        {
            get { return ModifiedUnitOfMeasure == -1 ? mUnitPrice : mUnitPrice / ModifiedUnitOfMeasure; }
            set { mUnitPrice = value; }
        }
        public double PartWeight { get; set; }
        public double CartonWeight { get; set; }
        public double CartonLength { get; set; }
        public double CartonWidth { get; set; }
        public double CartonHeight { get; set; }
        public HaworthDeliveryInformation DeliveryInformation { get; set; }
        public double LeadTime { get; set; }
        public string ShippingInstructions { get; set; }
        public string PurchText { get; set; }
        public string PurchText2 { get; set; }
        public string TransType { get; set; }
        public DateTime MaintenanceDateTime { get; set; }
        public DateTime ImportDateTime { get; set; }
    }


}