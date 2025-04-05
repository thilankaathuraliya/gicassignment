using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GICAssignment.Models
{
    public class InterestRule
    {
        public DateTime EffectiveDate { get; set; }
        public string RuleId { get; set; }
        public decimal Rate { get; set; } // as percentage

        public InterestRule()
        {
            RuleId = string.Empty;
        }
    }
}
