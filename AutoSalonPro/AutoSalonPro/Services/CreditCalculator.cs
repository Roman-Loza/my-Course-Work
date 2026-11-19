namespace AutoSalonPro.Services;

// Расчёт аннуитетного платежа по автокредиту
public static class CreditCalculator
{
    public static decimal Monthly(decimal price, decimal down, decimal annualRate, int months)
    {
        var body = price - down;
        if (body <= 0 || months <= 0) return 0;
        if (annualRate == 0) return Math.Round(body / months, 2);

        double r = (double)annualRate / 12.0 / 100.0;
        double k = r * Math.Pow(1 + r, months) / (Math.Pow(1 + r, months) - 1);
        return Math.Round(body * (decimal)k, 2);
    }

    public static decimal Overpayment(decimal price, decimal down, decimal annualRate, int months)
    {
        var body = price - down;
        if (body <= 0) return 0;
        return Monthly(price, down, annualRate, months) * months - body;
    }
}
