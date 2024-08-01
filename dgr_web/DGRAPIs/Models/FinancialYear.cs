using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace DGRAPIs.Models
{
    public class FinancialYear
    {
        internal string Name;

        public string financial_year { get; set; }
        public dynamic start_date { get; set; }
        public dynamic end_date { get; set; }
        
    }
}
