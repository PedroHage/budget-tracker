using BudgetTracker.Utilities;
using BudgetTracker.Models;
using BudgetTracker.Services;

namespace BudgetTracker.Menu;

class MenuController
{
    private string[] categories = ["Food", "Rent", "Transport", "Utilities", "Debt", "Health", "Shopping", "Entertainment", "Education", "Work"];
    private TransactionService service = new();
    private List<Transaction> transactions = [];

    public void Show()
    {
        transactions = service.LoadTransactions();
        while (true)
        {
            Console.WriteLine("<===============Budget Tracker===============>");
            Console.WriteLine("[1] Add Transaction");
            Console.WriteLine("[2] View Transactions");
            Console.WriteLine("[3] View Balance");
            Console.WriteLine("[4] View Income");
            Console.WriteLine("[5] View Expenses");
            Console.WriteLine("[6] Filter by Category");
            Console.WriteLine("[7] Edit Transaction");
            Console.WriteLine("[8] Delete Transaction");
            Console.WriteLine("[9] Exit");
            Console.Write("> ");

            var userInput = Console.ReadLine();
            Console.WriteLine("\n");

            switch (userInput)
            {
                case "1": AddTransaction(); break;
                case "2": ViewTransactions(transactions); break;
                case "3": Console.WriteLine(GetBalance(transactions)); break;
                case "4": Console.WriteLine(GetIncome(transactions)); break; // transactions.Where(t => t.Amount > 0).Sum(t => t.Amount)
                case "5": Console.WriteLine(GetExpenses(transactions)); break;
                case "6": FilterByCategory(); break;
                case "7": EditTransaction(); break;
                case "8": DeleteTransaction(); break;
                case "9": Environment.Exit(0); break;
                default: Console.WriteLine("Invalid option."); break;
            }
        }
    }

    private void AddTransaction()
    {
        var (amount, description, categoryIndex) = GetTransactionInfo();
        service.AddTransaction(transactions, new Transaction(amount, categories[categoryIndex], description));
        Console.WriteLine("Transaction registered with success.");
    }

    private (decimal amount, string description, int categoryIndex) GetTransactionInfo()
    {
        decimal amount;
        string description;
        int category;
        while (true)
        {
            Console.Write("Amount: ");
            var inputAmount = Console.ReadLine();
            if (!decimal.TryParse(inputAmount, out decimal d))
            {
                Console.WriteLine("Invalid amount. Try again\n");
                continue;
            }
            amount = d;
            break;
        }
        while (true)
        {
            Console.Write("\nDescription: ");
            var inputDescription = Console.ReadLine();
            if (string.IsNullOrEmpty(inputDescription))
            {
                Console.WriteLine("Invalid description. Try again\n");
                continue;
            }
            description = inputDescription;
            break;
        }

        while (true)
        {
            Console.WriteLine("\nCategory: \n");
            for (int i = 0; i < categories.Length; i++)
            {
                Console.WriteLine($"[{i}] {categories[i]}");
            }
            var inputCategoryIndex = Console.ReadLine();
            if (!int.TryParse(inputCategoryIndex, out int categoryIndex) || categories.ElementAtOrDefault(categoryIndex) == null)
            {
                Console.WriteLine("Invalid Category. Try again");
                continue;
            }
            category = categoryIndex;
            break;
        }
        return (amount, description, category);
    }

    private void ViewTransactions(List<Transaction> transactions)
    {
        Console.WriteLine("Transactions:");
        for (int i = 0; i < transactions.Count; i++)
        {
            Console.WriteLine($"\n{i + 1}. Amount: {CurrencyFormatter.FormatCurrency(transactions[i].Amount, "en-us")}\nCategory: {transactions[i].Category}\nDescription: {transactions[i].Description}\nDate created: {transactions[i].Date}");
        }
    }

    private string GetBalance(List<Transaction> transactions)
    {
        decimal balance = transactions.Sum(t => t.Amount);
        return "Balance: " + CurrencyFormatter.FormatCurrency(balance, "en-us");
    }

    private string GetIncome(List<Transaction> transactions)
    {
        return "Income: " + CurrencyFormatter.FormatCurrency(transactions.Where(t => t.Amount > 0).Sum(t => t.Amount), "en-us");
    }

    private string GetExpenses(List<Transaction> transactions)
    {
        return "Expenses: " + CurrencyFormatter.FormatCurrency(transactions.Where(t => t.Amount < 0).Sum(t => t.Amount), "en-us");
    }

    private void FilterByCategory()
    {
        var groupedTransactions = transactions.GroupBy(t => t.Category);
        foreach (var group in groupedTransactions)
        {
            Console.WriteLine($"\nCategory: {group.Key}");
            Console.WriteLine(GetIncome(group.ToList()));
            Console.WriteLine(GetExpenses(group.ToList()));
            Console.WriteLine(GetBalance(group.ToList()));
            Console.WriteLine(new string('-', 30));
        }
    }

    private void EditTransaction()
    {
        ViewTransactions(transactions);
        Transaction chosenTransaction;
        while (true)
        {
            Console.Write("Choose the transaction to edit:");
            var transactionChoice = Console.ReadLine();
            if (!int.TryParse(transactionChoice, out int transactionIndex) || transactions.ElementAtOrDefault(transactionIndex - 1) == null)
            {
                Console.WriteLine("\nNo such transaction.");
                continue;
            }
            chosenTransaction = transactions[transactionIndex - 1];
            break;
        }

        var (amount, description, categoryIndex) = GetTransactionInfo();
        string userConfirmation;
        while (true)
        {
            Console.WriteLine("Do you want to confirm the changes (y/n)?");
            userConfirmation = (Console.ReadLine() ?? "").Trim().ToLower();
            if (userConfirmation != "y" && userConfirmation != "n")
            {
                Console.WriteLine("Invalid option");
                continue;
            }
            break;
        }
        if (userConfirmation == "n")
        {
            Console.WriteLine("Changes canceled.");
            return;
        }
        chosenTransaction.Amount = amount;
        chosenTransaction.Description = description;
        chosenTransaction.Category = categories[categoryIndex];
        service.SaveTransactions(transactions);
        Console.WriteLine("Changes successfully saved.");
    }

    private void DeleteTransaction()
    {
        ViewTransactions(transactions);
        Transaction chosenTransaction;
        while (true)
        {
            Console.Write("Choose the transaction to Delete:");
            var transactionChoice = Console.ReadLine();
            if (!int.TryParse(transactionChoice, out int transactionIndex) || transactions.ElementAtOrDefault(transactionIndex - 1) == null)
            {
                Console.WriteLine("\nNo such transaction.");
                continue;
            }
            chosenTransaction = transactions[transactionIndex - 1];
            break;
        }
        string userConfirmation;
        while (true)
        {
            Console.WriteLine("Do you want to confirm the deletion (y/n)?");
            userConfirmation = (Console.ReadLine() ?? "").Trim().ToLower();
            if (userConfirmation != "y" && userConfirmation != "n")
            {
                Console.WriteLine("Invalid option");
                continue;
            }
            break;
        }
        if (userConfirmation == "n")
        {
            Console.WriteLine("Deletion canceled.");
            return;
        }
        service.RemoveTransaction(transactions, chosenTransaction);
        Console.WriteLine("Transaction deleted.");
    }
}