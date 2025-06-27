using System.Text.Json;

class TransactionService
{
    private readonly string _filePath;

    public TransactionService(string filePath = "transactions.json")
    {
        _filePath = filePath;
    }

    public List<Transaction> LoadTransactions()
    {
        if (!File.Exists(_filePath))
            return new List<Transaction>();
        var json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<Transaction>>(json) ?? new List<Transaction>();
    }

    public void SaveTransactions(List<Transaction> transactions)
    {
        var json = JsonSerializer.Serialize(transactions, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }

    public void AddTransaction(List<Transaction> transactions, Transaction transaction)
    {
        transactions.Add(transaction);
        SaveTransactions(transactions);
    }
}