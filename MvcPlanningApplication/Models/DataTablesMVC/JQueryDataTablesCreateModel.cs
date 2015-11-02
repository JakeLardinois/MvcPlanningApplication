using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcPlanningApplication.Models.DataTablesMVC
{
    public class JQueryDataTablesCreateModel
    {
        public string action { get; set; }
        public Dictionary<string, string> data { get; set; }
    }
}
