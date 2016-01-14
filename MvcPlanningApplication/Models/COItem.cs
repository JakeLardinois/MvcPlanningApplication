using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcPlanningApplication.Models
{
    public class COItem
    {
        public string co_num { get; set; }
        public short co_line { get; set; }
        public short co_release { get; set; }
        public string item { get; set; }
        public decimal qty_ordered { get; set; }
        public Nullable<System.DateTime> due_date { get; set; }
        public Nullable<System.DateTime> promise_date { get; set; }
        public string ref_num { get; set; }
        public Nullable<short> ref_line_suf { get; set; }

        public string cust_po { get; set; } //field is from the co table...
        public decimal qty_complete { get; set; } //field is from the job table...

        public string PurchaseOrder
        {
            get
            {
                return cust_po + "." + co_line.ToString().PadLeft(5, '0') + ".0001";
            }
        }
    }
}