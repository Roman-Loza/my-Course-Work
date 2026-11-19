using AutoSalonPro.Models;
using AutoSalonPro.Services;

namespace AutoSalonPro.Forms;

public class FormTestDrive : Form
{
    private readonly Car _car;
    private readonly DateTimePicker _date;
    private readonly ComboBox _time;
    private readonly TextBox _phone;

    public FormTestDrive(Car car)
    {
        _car = car;
        Text = "Запись на тест-драйв";
        ClientSize = new Size(420, 360);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        Ui.AddLabel(this, "Тест-драйв", 20, 15, 380, true);
        Ui.AddLabel(this, car.Title, 30, 60, 360);
        Ui.AddLabel(this, "Дата", 30, 95, 150);
        _date = new DateTimePicker
        {
            Left = 30, Top = 118, Width = 160, Font = Ui.Normal, Format = DateTimePickerFormat.Short,
            MinDate = DateTime.Today, MaxDate = DateTime.Today.AddDays(30)
        };
        Controls.Add(_date);
        Ui.AddLabel(this, "Время", 220, 95, 150);
        _time = Ui.AddCombo(this, 220, 118, 150, new[] { "10:00", "11:00", "12:00", "13:00", "14:00", "15:00", "16:00", "17:00", "18:00" });
        Ui.AddLabel(this, "Контактный телефон", 30, 165, 300);
        _phone = Ui.AddText(this, 30, 188, 340);
        _phone.Text = AuthService.Current.Phone;
        Ui.AddButton(this, "Записаться", 120, 260, 180, 40, OnSave);
    }

    private void OnSave(object sender, EventArgs e)
    {
        var parts = ((string)_time.SelectedItem).Split(':');
        var when = _date.Value.Date.AddHours(int.Parse(parts[0]));
        if (when <= DateTime.Now)
        {
            Ui.Error("Выберите время в будущем");
            return;
        }
        if (_phone.Text.Count(char.IsDigit) < 10)
        {
            Ui.Error("Введите корректный номер телефона");
            return;
        }
        bool busy = Storage.Db.TestDrives.Any(t => t.CarId == _car.Id && t.Date == when
            && t.Status != TestDriveStatus.Cancelled);
        if (busy)
        {
            Ui.Error("Это время уже занято, выберите другое");
            return;
        }
        Storage.Db.TestDrives.Add(new TestDrive
        {
            Id = Storage.NextId(Storage.Db.TestDrives.Select(t => t.Id)),
            CarId = _car.Id, ClientId = AuthService.Current.Id, Date = when,
            Phone = _phone.Text.Trim(), Status = TestDriveStatus.Planned
        });
        Storage.Save();
        Ui.Info($"Вы записаны на тест-драйв: {when:dd.MM.yyyy HH:mm}");
        DialogResult = DialogResult.OK;
    }
}
