using System;
using System.Collections.Generic;
using System.Linq;

namespace BudgetTracker
{
    public class Transaction
    {
        public DateTime Date { get; set; }
        public string Store { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public string Type { get; set; } // "Income" or "Expense"
    }

    public class Budget
    {
        public decimal Balance { get; private set; }
        public List<Transaction> History { get; private set; } = new();

        public Budget(decimal startingBalance)
        {
            Balance = startingBalance;
        }

        public void AddExpense(string store, decimal amount, string category)
        {
            if (amount <= 0) throw new ArgumentException("Expense must be positive.");

            Balance -= amount;
            History.Add(new Transaction
            {
                Date = DateTime.Now,
                Store = store,
                Amount = amount,
                Category = category,
                Type = "Expense"
            });

            Console.WriteLine($"Expense recorded: -{amount:C} at {store}. Remaining balance: {Balance:C}");
        }

        public void AddIncome(decimal amount, string source)
        {
            if (amount <= 0) throw new ArgumentException("Income must be positive.");

            Balance += amount;
            History.Add(new Transaction
            {
                Date = DateTime.Now,
                Store = source,
                Amount = amount, // always positive
                Category = "Income",
                Type = "Income"
            });

            Console.WriteLine($"Income recorded: +{amount:C} from {source}. New balance: {Balance:C}");
        }

        public void WeeklySummary()
        {
            var weekAgo = DateTime.Now.AddDays(-7);
            var weeklyTransactions = History.Where(t => t.Date >= weekAgo).ToList();

            Console.WriteLine("----- Weekly Summary -----");
            if (!weeklyTransactions.Any())
            {
                Console.WriteLine("No transactions recorded this week.");
                return;
            }

            var totalIncome = weeklyTransactions.Where(t => t.Type == "Income").Sum(t => t.Amount);
            var totalExpense = weeklyTransactions.Where(t => t.Type == "Expense").Sum(t => t.Amount);

            Console.WriteLine($"Total Income:  +{totalIncome:C}");
            Console.WriteLine($"Total Expense: -{totalExpense:C}");
            Console.WriteLine($"Net Change:    {(totalIncome - totalExpense):C}");

            Console.WriteLine("\nBy Category (Expenses only):");
            foreach (var group in weeklyTransactions.Where(t => t.Type == "Expense").GroupBy(t => t.Category))
            {
                Console.WriteLine($"{group.Key}: {group.Sum(t => t.Amount):C}");
            }
        }

        public void ShowHistory()
        {
            Console.WriteLine("----- Transaction History -----");
            foreach (var t in History)
            {
                var sign = t.Type == "Expense" ? "-" : "+";
                Console.WriteLine($"{t.Date:yyyy-MM-dd} | {t.Store,-12} | {t.Category,-10} | {sign}{t.Amount:C}");
            }
        }
    }

    class Program
    {
        static void Main()
        {
            Console.Write("Enter starting balance: ");
            var startingBalance = decimal.Parse(Console.ReadLine());
            var budget = new Budget(startingBalance);

            while (true)
            {
                Console.WriteLine("\nMenu:");
                Console.WriteLine("1) Add Expense");
                Console.WriteLine("2) Add Income");
                Console.WriteLine("3) Weekly Summary");
                Console.WriteLine("4) Show History");
                Console.WriteLine("q) Quit");
                Console.Write("> ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Store: ");
                        var store = Console.ReadLine();
                        Console.Write("Amount: ");
                        var amount = decimal.Parse(Console.ReadLine());
                        Console.Write("Category: ");
                        var category = Console.ReadLine();
                        budget.AddExpense(store, amount, category);
                        break;
                    case "2":
                        Console.Write("Income Amount: ");
                        var income = decimal.Parse(Console.ReadLine());
                        Console.Write("Source: ");
                        var source = Console.ReadLine();
                        budget.AddIncome(income, source);
                        break;
                    case "3":
                        budget.WeeklySummary();
                        break;
                    case "4":
                        budget.ShowHistory();
                        break;
                    case "q":
                        return;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }
    }
}
