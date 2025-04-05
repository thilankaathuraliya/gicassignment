using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GICAssignment.Models
{
    public class Account
    {
        public string AccountId { get; set; }
        public List<Transaction> Transactions { get; set; }

        public Account()
        {
            AccountId = string.Empty;
            Transactions = new List<Transaction>();
        }

        public decimal GetBalanceAt(DateTime date)
        {
            return Transactions
                .Where(t => t.Date <= date)
                .Sum(t => t.Type.ToUpper() == "D" ? t.Amount : t.Type.ToUpper() == "W" ? -t.Amount : t.Amount);
        }

        public decimal CurrentBalance
        {
            get { return GetBalanceAt(DateTime.MaxValue); }
        }
    }
}
