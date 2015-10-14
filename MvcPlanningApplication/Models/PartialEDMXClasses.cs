using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;


namespace MvcPlanningApplication.Models
{
    [MetadataType(typeof(coitemMetadata))]
    public partial class coitem
    {
        public string cust_po { get; set; }
    }

    public class coitemMetadata
    {
        //[Display(Name = "Order No.")]
        //public string co_num { get; set; }
    }
}