using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GICAssignment.Models
{
    public class Transaction
    {
        public DateTime Date { get; set; }
        public string TransactionId { get; set; }
        public string Type { get; set; } // D, W, or I
        public decimal Amount { get; set; }

        public Transaction()
        {
            TransactionId = string.Empty;
            Type = string.Empty;
        }
    }
}
