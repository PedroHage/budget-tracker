string[] categories = ["Food", "Rent", "Transport", "Utilities", "Debt", "Health", "Shopping", "Entertainment", "Education", "Work"];
TransactionService service= new TransactionService();
List<Transaction> transactions = service.LoadTransactions();

while (true)
{
    Console.WriteLine("[1] Add Transaction");
    Console.WriteLine("[2] View Transactions");
    Console.WriteLine("[3] View Balance");
    Console.WriteLine("[4] Exit");
    Console.Write("> ");
    char userInput = Console.ReadKey().KeyChar;
    Console.WriteLine("");

    switch (userInput)
    {
        case '1': AddTransaction(); break;
        case '2': ViewTransaction(); break;
        case '3': ViewBalance(); break;
        case '4': Environment.Exit(0); break;
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
    var description = Console.ReadLine();
    if (description == null)
    {
        description = "";
    }
    Console.WriteLine("\nCategory: \n");
    for (int i = 0; i < categories.Length; i++)
    {
        Console.WriteLine($"[{i}] {categories[i]}");
    }
    userInput = Console.ReadLine();
    if (!int.TryParse(userInput, out int categoryIndex) || categoryIndex < 0 || categoryIndex > 9 )
    {
        Console.WriteLine("\nInvalid category.");
        return;
    }
    service.AddTransaction(transactions, new Transaction(amount, categories[categoryIndex], description));
    Console.WriteLine("Transaction registered with success.");
}

void ViewTransaction()
{
    Console.WriteLine("Transactions:");
    for (int i = 0; i < transactions.Count; i++)
    {
        Console.WriteLine($"\n{i}. Amount: ${transactions[i].Amount}\nCategory: {transactions[i].Category}\nDescription: {transactions[i].Description}\nDate: {transactions[i].Date}");
    }
}

void ViewBalance()
{
    decimal balance = 0;
    foreach (Transaction transaction in transactions)
    {
        balance += transaction.Amount;
    }
    Console.WriteLine($"Balance: {balance}");
}