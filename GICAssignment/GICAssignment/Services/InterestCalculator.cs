using GICAssignment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GICAssignment.Services
{
    public class InterestCalculator
    {
        public decimal Calculate(Account account, List<InterestRule> rules, DateTime month)
        {
            DateTime start = new DateTime(month.Year, month.Month, 1);
            DateTime end = new DateTime(month.Year, month.Month, DateTime.DaysInMonth(month.Year, month.Month));

            List<InterestRule> ruleList = rules.FindAll(delegate (InterestRule r) { return r.EffectiveDate <= end; });
            ruleList.Sort((x, y) => x.EffectiveDate.CompareTo(y.EffectiveDate));
            if (ruleList.Count == 0) return 0;

            decimal interest = 0;
            DateTime current = start;
            while (current <= end)
            {
                InterestRule rule = ruleList[0];
                for (int i = ruleList.Count - 1; i >= 0; i--)
                {
                    if (ruleList[i].EffectiveDate <= current)
                    {
                        rule = ruleList[i];
                        break;
                    }
                }

                decimal balance = account.GetBalanceAt(current);
                interest += balance * (rule.Rate / 100);
                current = current.AddDays(1);
            }

            return interest / 365;
        }
    }
}
