using BudgetTracker.Menu;
namespace BudgetTracker;

class Program
{
    public static void Main(string[] args)
    {
        MenuController menu = new();
        menu.Show();
    }
}