using System.Globalization;
namespace BudgetTracker.Utilities;

public static class CurrencyFormatter
{
    public static string FormatCurrency(decimal value, string cultureCode)
    {
        var culture = new CultureInfo(cultureCode);
        var numberFormatInfo = (NumberFormatInfo)culture.NumberFormat.Clone();
        numberFormatInfo.CurrencyNegativePattern = 1;
        return value.ToString("C", numberFormatInfo);
    }
}