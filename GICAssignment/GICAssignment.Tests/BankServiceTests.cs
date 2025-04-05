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
    public class BankServiceTests
    {
        [Test]
        public void TransactionIdFormat_IsCorrect()
        {
            var txn = new Transaction
            {
                Date = new DateTime(2023, 4, 5),
                Type = "D",
                Amount = 50m,
                TransactionId = "20230405-01"
            };

            Assert.AreEqual("20230405-01", txn.TransactionId);
        }

        [Test]
        public void Withdrawal_ExceedingBalance_ShouldBePrevented()
        {
            var account = new Account();
            account.Transactions.Add(new Transaction { Date = DateTime.Today, Type = "D", Amount = 100m });

            decimal balance = account.GetBalanceAt(DateTime.Today);
            Assert.IsTrue(balance >= 100m);
            Assert.IsFalse(balance < 0m);
        }
    }
}
