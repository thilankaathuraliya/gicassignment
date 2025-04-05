using GICAssignment.Models;
using GICAssignment.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GICAssignment.Tests
{
    [TestFixture]
    public class InterestCalculatorTests
    {
        [Test]
        public void Calculate_WithSingleRule_AppliesCorrectRate()
        {
            var account = new Account();
            account.Transactions.Add(new Transaction
            {
                Date = new DateTime(2023, 6, 1),
                Type = "D",
                Amount = 1000m
            });

            var rules = new List<InterestRule>
            {
                new InterestRule { EffectiveDate = new DateTime(2023, 1, 1), RuleId = "R1", Rate = 2.0m }
            };

            var calculator = new InterestCalculator();
            var result = calculator.Calculate(account, rules, new DateTime(2023, 6, 1));
            decimal expected = (1000m * 2.0m * 30) / 365;
            Assert.AreEqual((double)expected, (double)result, 0.01);
        }

        [Test]
        public void Calculate_WithNoRules_ReturnsZero()
        {
            var account = new Account();
            account.Transactions.Add(new Transaction
            {
                Date = new DateTime(2023, 6, 1),
                Type = "D",
                Amount = 1000m
            });

            var calculator = new InterestCalculator();
            var result = calculator.Calculate(account, new List<InterestRule>(), new DateTime(2023, 6, 1));
            Assert.AreEqual(0m, result);
        }

        [Test]
        public void Calculate_WithMultipleRules_UsesEffectiveRate()
        {
            var account = new Account();
            account.Transactions.Add(new Transaction
            {
                Date = new DateTime(2023, 6, 1),
                Type = "D",
                Amount = 500m
            });

            var rules = new List<InterestRule>
            {
                new InterestRule { EffectiveDate = new DateTime(2023, 6, 1), RuleId = "R1", Rate = 2.0m },
                new InterestRule { EffectiveDate = new DateTime(2023, 6, 15), RuleId = "R2", Rate = 3.0m }
            };

            var calculator = new InterestCalculator();
            var result = calculator.Calculate(account, rules, new DateTime(2023, 6, 1));

            decimal part1 = 500m * 2.0m * 14;
            decimal part2 = 500m * 3.0m * 16;
            decimal expected = (part1 + part2) / 365;

            Assert.AreEqual((double)expected, (double)result, 0.01);
        }
    }
}
