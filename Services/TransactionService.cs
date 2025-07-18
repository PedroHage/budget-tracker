using System.Text.Json;
using BudgetTracker.Models;
namespace BudgetTracker.Services;

class TransactionService
{
    private readonly string _filePath;

    public TransactionService(string filePath = "transactions.json")
    {
        _filePath = filePath;
    }

    private static readonly JsonSerializerOptions s_writeOptions = new() { WriteIndented = true };

    public List<Transaction> LoadTransactions()
    {
        try
        {
            if (!File.Exists(_filePath))
                return new List<Transaction>();

            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<Transaction>>(json) ?? new List<Transaction>();
        }
        catch (JsonException ex)
        {
            Console.WriteLine("Invalid JSON format:" + ex.Message);
            return new List<Transaction>();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to load transactions: " + ex.Message);
            return new List<Transaction>();
        }
    }

    public void SaveTransactions(List<Transaction> transactions)
    {
        try
        {
            var json = JsonSerializer.Serialize(transactions, s_writeOptions);
            File.WriteAllText(_filePath, json);
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine("Error: Access to the file denied.");
            Console.WriteLine("Transactions NOT saved");
        }
        catch (IOException ex)
        {
            Console.WriteLine("IO error:" + ex.Message);
            Console.WriteLine("Transactions NOT saved");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unexpected error saving transactions: " + ex.Message);
            Console.WriteLine("Transactions NOT saved");
        }
    }

    public void AddTransaction(List<Transaction> transactions, Transaction transaction)
    {
        transactions.Add(transaction);
        SaveTransactions(transactions);
    }

    public void RemoveTransaction(List<Transaction> transactions, Transaction transaction)
    {
        transactions.Remove(transaction);
        SaveTransactions(transactions);
    }
}