using AutoSalonPro.Models;
using AutoSalonPro.Services;

namespace AutoSalonPro.Forms;

public class FormOrder : Form
{
    private readonly Car _car;
    private readonly ComboBox _payment, _term;
    private readonly NumericUpDown _down, _rate;
    private readonly Label _monthly;
    private readonly TextBox _comment;

    public FormOrder(Car car)
    {
        _car = car;
        Text = "Заявка на покупку";
        ClientSize = new Size(480, 560);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        Ui.AddLabel(this, "Заявка на покупку", 20, 15, 440, true);
        Ui.AddLabel(this, $"{car.Title}, {car.Year} г.", 30, 60, 420);
        Ui.AddLabel(this, $"Стоимость: {car.Price:N0} ₽", 30, 85, 420);

        Ui.AddLabel(this, "Способ оплаты", 30, 125, 200);
        _payment = Ui.AddCombo(this, 30, 148, 200, new[] { "Наличные", "Кредит" });
        Ui.AddLabel(this, "Первоначальный взнос, ₽", 30, 188, 250);
        _down = Ui.AddNum(this, 30, 211, 200, 0, car.Price, car.Price * 0.2m, 0, 10000);
        Ui.AddLabel(this, "Срок кредита, мес.", 260, 188, 200);
        _term = Ui.AddCombo(this, 260, 211, 190, new[] { "12", "24", "36", "48", "60", "72", "84" });
        _term.SelectedIndex = 4;
        Ui.AddLabel(this, "Процентная ставка, % годовых", 30, 251, 250);
        _rate = Ui.AddNum(this, 30, 274, 200, 0, 40, 16, 1, 0.5m);
        _monthly = Ui.AddLabel(this, "", 30, 314, 420);
        _monthly.Font = new Font("Segoe UI", 11F, FontStyle.Bold);

        Ui.AddLabel(this, "Комментарий", 30, 355, 200);
        _comment = new TextBox { Left = 30, Top = 378, Width = 420, Height = 70, Multiline = true, Font = Ui.Normal };
        Controls.Add(_comment);

        Ui.AddButton(this, "Отправить заявку", 150, 480, 180, 40, OnSend);

        _payment.SelectedIndexChanged += (s, e) => Recalc();
        _down.ValueChanged += (s, e) => Recalc();
        _term.SelectedIndexChanged += (s, e) => Recalc();
        _rate.ValueChanged += (s, e) => Recalc();
        Recalc();
    }

    private bool IsCredit => _payment.SelectedIndex == 1;

    private void Recalc()
    {
        _down.Enabled = _term.Enabled = _rate.Enabled = IsCredit;
        if (!IsCredit)
        {
            _monthly.Text = "Оплата единовременно";
            return;
        }
        int months = int.Parse((string)_term.SelectedItem);
        var pay = CreditCalculator.Monthly(_car.Price, _down.Value, _rate.Value, months);
        _monthly.Text = $"Платёж в месяц: {pay:N0} ₽";
    }

    private void OnSend(object sender, EventArgs e)
    {
        var me = AuthService.Current;
        bool exists = Storage.Db.Orders.Any(o => o.ClientId == me.Id && o.CarId == _car.Id
            && (o.Status == OrderStatus.New || o.Status == OrderStatus.Approved));
        if (exists)
        {
            Ui.Error("У вас уже есть активная заявка на этот автомобиль");
            return;
        }
        var order = new Order
        {
            Id = Storage.NextId(Storage.Db.Orders.Select(o => o.Id)),
            CarId = _car.Id,
            ClientId = me.Id,
            Created = DateTime.Now,
            Status = OrderStatus.New,
            PaymentType = (string)_payment.SelectedItem,
            Downpayment = IsCredit ? _down.Value : _car.Price,
            TermMonths = IsCredit ? int.Parse((string)_term.SelectedItem) : 0,
            Rate = IsCredit ? _rate.Value : 0,
            Comment = _comment.Text.Trim()
        };
        Storage.Db.Orders.Add(order);
        Storage.Save();
        Ui.Info("Заявка отправлена. Менеджер свяжется с вами после рассмотрения.");
        DialogResult = DialogResult.OK;
    }
}
