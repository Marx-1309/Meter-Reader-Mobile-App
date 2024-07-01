using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleMauiMvvmApp.Models
{
    public class BillingLocation
    {
        public int BillingLocationId { get; set; }
        public string Name { get; set; }
        public string? Township { get; set; }
    }
}
