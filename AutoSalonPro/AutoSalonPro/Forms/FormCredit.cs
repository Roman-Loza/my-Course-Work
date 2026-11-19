using AutoSalonPro.Services;

namespace AutoSalonPro.Forms;

public class FormCredit : Form
{
    private readonly NumericUpDown _price, _down, _rate, _term;
    private readonly Label _monthly, _over, _total;

    public FormCredit()
    {
        Text = "Кредитный калькулятор";
        ClientSize = new Size(440, 430);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        Ui.AddLabel(this, "Кредитный калькулятор", 20, 15, 400, true);
        Ui.AddLabel(this, "Стоимость автомобиля, ₽", 30, 65, 250);
        _price = Ui.AddNum(this, 30, 88, 220, 100000, 100000000, 2000000, 0, 50000);
        Ui.AddLabel(this, "Первоначальный взнос, ₽", 30, 125, 250);
        _down = Ui.AddNum(this, 30, 148, 220, 0, 100000000, 400000, 0, 10000);
        Ui.AddLabel(this, "Ставка, % годовых", 30, 185, 250);
        _rate = Ui.AddNum(this, 30, 208, 220, 0, 40, 16, 1, 0.5m);
        Ui.AddLabel(this, "Срок, месяцев", 30, 245, 250);
        _term = Ui.AddNum(this, 30, 268, 220, 6, 120, 60, 0, 6);

        _monthly = Ui.AddLabel(this, "", 30, 315, 380);
        _over = Ui.AddLabel(this, "", 30, 345, 380);
        _total = Ui.AddLabel(this, "", 30, 375, 380);

        foreach (var n in new[] { _price, _down, _rate, _term })
            n.ValueChanged += (s, e) => Recalc();
        Recalc();
    }

    private void Recalc()
    {
        int months = (int)_term.Value;
        var monthly = CreditCalculator.Monthly(_price.Value, _down.Value, _rate.Value, months);
        var over = CreditCalculator.Overpayment(_price.Value, _down.Value, _rate.Value, months);
        _monthly.Text = $"Ежемесячный платёж: {monthly:N0} ₽";
        _over.Text = $"Переплата: {over:N0} ₽";
        _total.Text = $"Общая сумма выплат: {monthly * months:N0} ₽";
    }
}
