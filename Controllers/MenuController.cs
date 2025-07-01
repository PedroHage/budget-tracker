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
            Console.WriteLine("[8] Exit");
            Console.Write("> ");
            char userInput = Console.ReadKey().KeyChar;
            Console.WriteLine("\n");

            switch (userInput)
            {
                case '1': AddTransaction(); break;
                case '2': ViewTransactions(transactions); break;
                case '3': Console.WriteLine(GetBalance(transactions)); break;
                case '4': Console.WriteLine(GetIncome(transactions)); break; // transactions.Where(t => t.Amount > 0).Sum(t => t.Amount)
                case '5': Console.WriteLine(GetExpenses(transactions)); break;
                case '6': FilterByCategory(); break;
                case '7': EditTransaction(); break;
                case '8': Environment.Exit(0); break;
                default: Console.WriteLine("Invalid option."); break;
            }
        }
    }

    private void AddTransaction()
    {
        var transactionInfo = GetTransactionInfo();
        if (transactionInfo.amount == null)
        {
            Console.WriteLine("Invalid amount. Transaction not saved");
            return;
        }
        decimal amount = transactionInfo.amount.Value;
        if (transactionInfo.category == null)
        {
            Console.WriteLine("Invalid category. Transaction not saved");
            return;
        }
        int categoryIndex = transactionInfo.category.Value;
        string description = transactionInfo.description;

        service.AddTransaction(transactions, new Transaction(amount, categories[categoryIndex], description));
        Console.WriteLine("Transaction registered with success.");
    }

    private (decimal? amount, string description, int? category) GetTransactionInfo()
    {
        decimal? amount = null;
        string? description = null;
        int? category = null;

        Console.Write("Amount: ");
        var userInput = Console.ReadLine();
        if (decimal.TryParse(userInput, out decimal d))
            amount = d;
        Console.Write("\nDescription: ");
        description = Console.ReadLine() ?? "";
        Console.WriteLine("\nCategory: \n");
        for (int i = 0; i < categories.Length; i++)
        {
            Console.WriteLine($"[{i}] {categories[i]}");
        }
        userInput = Console.ReadLine();
        if (int.TryParse(userInput, out int categoryIndex) && categories.ElementAtOrDefault(categoryIndex) != null)
        {
            category = categoryIndex;
        }
        return (amount, description, category);
    }

    private void ViewTransactions(List<Transaction> transactions)
    {
        Console.WriteLine("Transactions:");
        for (int i = 0; i < transactions.Count; i++)
        {
            Console.WriteLine($"\n{i + 1}. Amount: {CurrencyFormatter.FormatCurrency(transactions[i].Amount, "en-us")}\nCategory: {transactions[i].Category}\nDescription: {transactions[i].Description}\nDate: {transactions[i].Date}");
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
        Console.Write("Choose the transaction to edit:");
        var transactionChoice = Console.ReadLine();

        Transaction chosenTransaction;
        if (int.TryParse(transactionChoice, out int transactionIndex) && transactions.ElementAtOrDefault(transactionIndex - 1) != null)
            chosenTransaction = transactions[transactionIndex - 1];
        else
        {
            Console.WriteLine("\nNo such transaction.");
            return;
        }

        var transactionInfo = GetTransactionInfo();

        chosenTransaction.Description = transactionInfo.description;
        if (transactionInfo.amount == null)
            Console.WriteLine("Amount value not in adequate format. Amount not changed");
        else
            chosenTransaction.Amount = transactionInfo.amount.Value;

        if (transactionInfo.category == null)
            Console.WriteLine("No such category. Category not changed");
        else
            chosenTransaction.Category = categories[transactionInfo.category.Value];
    }

}