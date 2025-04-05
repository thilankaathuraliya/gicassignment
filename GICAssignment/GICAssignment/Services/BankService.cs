using GICAssignment.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GICAssignment.Services
{
    public class BankService
    {
        private Dictionary<string, Account> _accounts = new Dictionary<string, Account>();
        private List<InterestRule> _rules = new List<InterestRule>();

        public void InputTransactions()
        {
            Console.WriteLine("Please enter transaction details in <Date> <Account> <Type> <Amount> format \n(or enter blank to go back to main menu):");
            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) break;

                string[] parts = input.Split();
                DateTime date;
                decimal amount;

                if (parts.Length != 4
                    || !DateTime.TryParseExact(parts[0], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date)
                    || string.IsNullOrWhiteSpace(parts[1])
                    || !(parts[2].ToUpper() == "D" || parts[2].ToUpper() == "W")
                    || !decimal.TryParse(parts[3], out amount) || amount <= 0)
                {
                    Console.WriteLine("Invalid input. Please try again.");
                    continue;
                }

                string accountId = parts[1];
                string type = parts[2].ToUpper();

                if (!_accounts.ContainsKey(accountId))
                {
                    _accounts[accountId] = new Account();
                    _accounts[accountId].AccountId = accountId;
                }

                Account account = _accounts[accountId];

                if (type == "W" && account.GetBalanceAt(date) < amount)
                {
                    Console.WriteLine("Insufficient balance. Cannot withdraw.");
                    continue;
                }

                int txnCount = account.Transactions.FindAll(t => t.Date == date).Count + 1;
                string txnId = date.ToString("yyyyMMdd") + "-" + txnCount.ToString("D2");
                Transaction txn = new Transaction();
                txn.Date = date;
                txn.TransactionId = txnId;
                txn.Type = type;
                txn.Amount = amount;
                account.Transactions.Add(txn);

                PrintAccountTransactions(account);
            }
        }

        public void DefineInterestRules()
        {
            Console.WriteLine("Please enter interest rules details in <Date> <RuleId> <Rate in %> format\n(or enter blank to go back to main menu):");
            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) break;

                string[] parts = input.Split();
                DateTime date;
                decimal rate;

                if (parts.Length != 3
                    || !DateTime.TryParseExact(parts[0], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date)
                    || string.IsNullOrWhiteSpace(parts[1])
                    || !decimal.TryParse(parts[2], out rate) || rate <= 0 || rate >= 100)
                {
                    Console.WriteLine("Invalid rule input. Please try again.");
                    continue;
                }

                _rules.RemoveAll(delegate (InterestRule r) { return r.EffectiveDate == date; });
                InterestRule newRule = new InterestRule();
                newRule.EffectiveDate = date;
                newRule.RuleId = parts[1];
                newRule.Rate = rate;
                _rules.Add(newRule);

                Console.WriteLine("Interest rules:");
                List<InterestRule> sortedRules = new List<InterestRule>(_rules);
                sortedRules.Sort((x, y) => x.EffectiveDate.CompareTo(y.EffectiveDate));

                foreach (InterestRule rule in sortedRules)
                {
                    Console.WriteLine("| {0:yyyyMMdd} | {1} | {2,8:N2} |", rule.EffectiveDate, rule.RuleId, rule.Rate);
                }
            }
        }

        public void PrintStatement()
        {
            Console.WriteLine("Please enter account and month to generate the statement <Account> <Year><Month>\n(or enter blank to go back to main menu):");
            Console.Write("> ");
            string input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) return;

            string[] parts = input.Split();
            DateTime monthStart;

            if (parts.Length != 2 || !_accounts.ContainsKey(parts[0]) || !DateTime.TryParseExact(parts[1] + "01", "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out monthStart))
            {
                Console.WriteLine("Invalid input.");
                return;
            }

            DateTime monthEnd = new DateTime(monthStart.Year, monthStart.Month, DateTime.DaysInMonth(monthStart.Year, monthStart.Month));
            Account account = _accounts[parts[0]];
            List<Transaction> monthlyTxns = account.Transactions.FindAll(delegate (Transaction t) { return t.Date >= monthStart && t.Date <= monthEnd; });
            monthlyTxns.Sort((a, b) => a.Date.CompareTo(b.Date));

            InterestCalculator calculator = new InterestCalculator();
            decimal interest = calculator.Calculate(account, _rules, monthStart);
            if (interest > 0)
            {
                Transaction interestTxn = new Transaction();
                interestTxn.Date = monthEnd;
                interestTxn.Type = "I";
                interestTxn.Amount = Math.Round(interest, 2);
                interestTxn.TransactionId = string.Empty;
                monthlyTxns.Add(interestTxn);
            }

            if (monthlyTxns.Count > 0)
            {
                decimal balance = 0;
                Console.WriteLine("Account: {0}", account.AccountId);
                Console.WriteLine("| Date     | Txn Id      | Type | Amount | Balance |");
                monthlyTxns.Sort((a, b) => a.Date != b.Date ? a.Date.CompareTo(b.Date) : string.Compare(a.TransactionId, b.TransactionId));
                foreach (Transaction txn in monthlyTxns)
                {
                    if (txn.Type == "D" || txn.Type == "d")
                        balance += txn.Amount;
                    else if (txn.Type == "W" || txn.Type == "w")
                        balance -= txn.Amount;
                    else if (txn.Type == "I") // interest
                        balance += txn.Amount;

                    Console.WriteLine("| {0} | {1,-11} | {2,4} | {3,6:F2} | {4,7:F2} |",
                        txn.Date.ToString("yyyyMMdd"),
                        txn.TransactionId ?? "",
                        txn.Type,
                        txn.Amount,
                        balance);
                }
            }
            else
            {
                Console.WriteLine("No transactions found for the selected period.");
            }
        }

        private void PrintAccountTransactions(Account account)
        {
            Console.WriteLine("Account: {0}", account.AccountId);
            Console.WriteLine("| Date     | Txn Id      | Type | Amount |");
            List<Transaction> sorted = new List<Transaction>(account.Transactions);
            sorted.Sort((a, b) => a.Date != b.Date ? a.Date.CompareTo(b.Date) : string.Compare(a.TransactionId, b.TransactionId));
            foreach (Transaction txn in sorted)
            {
                Console.WriteLine("| {0:yyyyMMdd} | {1,-11} | {2}    | {3,6:N2} |",
                    txn.Date, txn.TransactionId, txn.Type, txn.Amount);
            }
        }
    }
}
