using GICAssignment.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GICAssignment.Tests
{
    [TestFixture]
    public class AccountTests
    {
        [Test]
        public void GetBalanceAt_WithDepositsAndWithdrawals_ReturnsCorrectBalance()
        {
            var account = new Account { AccountId = "A001" };
            account.Transactions.Add(new Transaction { Date = new DateTime(2023, 1, 1), Type = "D", Amount = 100m });
            account.Transactions.Add(new Transaction { Date = new DateTime(2023, 1, 2), Type = "W", Amount = 40m });
            account.Transactions.Add(new Transaction { Date = new DateTime(2023, 1, 3), Type = "D", Amount = 60m });

            var balance = account.GetBalanceAt(new DateTime(2023, 1, 3));
            Assert.AreEqual(120m, balance);
        }

        [Test]
        public void CurrentBalance_ReturnsTotalOfAllTransactions()
        {
            var account = new Account();
            account.Transactions.Add(new Transaction { Date = DateTime.Today, Type = "D", Amount = 100m });
            account.Transactions.Add(new Transaction { Date = DateTime.Today, Type = "W", Amount = 50m });
            account.Transactions.Add(new Transaction { Date = DateTime.Today, Type = "I", Amount = 5m });

            Assert.AreEqual(55m, account.CurrentBalance);
        }
    }
}
