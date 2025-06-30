string[] categories = ["Food", "Rent", "Transport", "Utilities", "Debt", "Health", "Shopping", "Entertainment", "Education", "Work"];
TransactionService service= new();
List<Transaction> transactions = service.LoadTransactions();

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

void AddTransaction()
{
    Console.Write("Amount: ");
    var userInput = Console.ReadLine();
    if (!decimal.TryParse(userInput, out decimal amount))
    {
        Console.WriteLine("Invalid Amount.");
        return;
    }

    Console.Write("\nDescription: ");
    string description = Console.ReadLine() ?? "";

    Console.WriteLine("\nCategory: \n");
    for (int i = 0; i < categories.Length; i++)
    {
        Console.WriteLine($"[{i}] {categories[i]}");
    }
    
    userInput = Console.ReadLine();
    if (!int.TryParse(userInput, out int categoryIndex) || categoryIndex < 0 || categoryIndex > categories.Length )
    {
        Console.WriteLine("\nInvalid category.");
        return;
    }
    service.AddTransaction(transactions, new Transaction(amount, categories[categoryIndex], description));
    Console.WriteLine("Transaction registered with success.");
}

void ViewTransactions(List<Transaction> transactions)
{
    Console.WriteLine("Transactions:");
    for (int i = 0; i < transactions.Count; i++)
    {
        Console.WriteLine($"\n{i+1}. Amount: ${transactions[i].Amount}\nCategory: {transactions[i].Category}\nDescription: {transactions[i].Description}\nDate: {transactions[i].Date}");
    }
}

string GetBalance(List<Transaction> transactions)
{
    decimal balance = transactions.Sum(t => t.Amount);
    return "Balance: " + balance;
}

string GetIncome(List<Transaction> transactions)
{
    return "Income: " + transactions.Where(t => t.Amount > 0).Sum(t => t.Amount);
}

string GetExpenses(List<Transaction> transactions)
{
    return "Expenses: " + transactions.Where(t => t.Amount < 0).Sum(t => t.Amount);
}

void FilterByCategory()
{
    foreach (string category in categories)
    {
        List<Transaction> filteredTransactions = transactions.Where(t => t.Category.Equals(category)).ToList();
        if (filteredTransactions.Count > 0)
        {
            Console.WriteLine($"{category}: ");
            Console.WriteLine($"{GetIncome(filteredTransactions)}\n{GetExpenses(filteredTransactions)}\n{GetBalance(filteredTransactions)}\n");
        }
    }
}

void EditTransaction()
{
    ViewTransactions(transactions);
    Console.Write("Choose the transaction to edit:");
    var transactionChoice = Console.ReadLine();
    Transaction chosenTransaction;
    if (int.TryParse(transactionChoice, out int transactionIndex) && transactions.ElementAtOrDefault(transactionIndex - 1) != null)
    {
        chosenTransaction = transactions[transactionIndex - 1];
    }
    else
    {
        Console.WriteLine("\nNo such transaction.");
        return;
    }

    Console.WriteLine("Leave blank to maintain previous info.\n");
    Console.Write("Amount: ");
    var editedAmount = Console.ReadLine();
    if (decimal.TryParse(editedAmount, out decimal amount))
    {
        chosenTransaction.Amount = amount;
        service.SaveTransactions(transactions);
    }
    else if (!string.IsNullOrEmpty(editedAmount))
    {
        Console.WriteLine("Invalid Amount.");
        return;
    }
    
    Console.Write("\nDescription: ");
    var editedDescription = Console.ReadLine();
    if (!string.IsNullOrEmpty(editedDescription))
    {
        chosenTransaction.Description = editedDescription;
        service.SaveTransactions(transactions);   
    }

    Console.WriteLine("\nCategory: \n");
    for (int i = 0; i < categories.Length; i++)
    {
        Console.WriteLine($"[{i}] {categories[i]}");
    }
    
    var editedCategoryChoice = Console.ReadLine();
    if (int.TryParse(editedCategoryChoice, out int categoryIndex))
    {
        chosenTransaction.Category = categories[categoryIndex];
        service.SaveTransactions(transactions);
    }
    else if (!string.IsNullOrEmpty(editedCategoryChoice))
    {
        Console.WriteLine("\nInvalid category.");
        return;
    }       
    Console.WriteLine("Transaction edited with success.");
}