using GICAssignment.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GICAssignment
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BankService bankService = new BankService();

            while (true)
            {
                Console.WriteLine("Welcome to AwesomeGIC Bank! What would you like to do?");
                Console.WriteLine("[T] Input transactions");
                Console.WriteLine("[I] Define interest rules");
                Console.WriteLine("[P] Print statement");
                Console.WriteLine("[Q] Quit");
                Console.Write("> ");
                string choice = Console.ReadLine();
                if (choice == null) continue;
                choice = choice.Trim().ToUpper();

                switch (choice)
                {
                    case "T":
                        bankService.InputTransactions();
                        break;
                    case "I":
                        bankService.DefineInterestRules();
                        break;
                    case "P":
                        bankService.PrintStatement();
                        break;
                    case "Q":
                        Console.WriteLine("Thank you for banking with AwesomeGIC Bank.\nHave a nice day!");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }
            }
        }
    }
}
