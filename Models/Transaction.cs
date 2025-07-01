namespace BudgetTracker.Models;
public class Transaction
{
    public decimal Amount { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }

    public Transaction(decimal amount, string category, string description)
    {
        Amount = amount;
        Category = category;
        Description = description;
        Date = DateTime.Now;
    }

}

