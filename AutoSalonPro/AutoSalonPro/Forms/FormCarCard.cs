using AutoSalonPro.Models;
using AutoSalonPro.Services;

namespace AutoSalonPro.Forms;

public class FormCarCard : Form
{
    private readonly Car _car;

    private void InitializeComponent()
    {

    }

    public FormCarCard(Car car)
    {
        _car = car;
        Text = car.Title;
        ClientSize = new Size(560, 560);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        Ui.AddLabel(this, $"{car.Title}, {car.Year}", 20, 15, 520, true);
        Ui.AddLabel(this, $"{car.Price:N0} ₽", 20, 50, 520, true);

        var info = new (string, string)[]
        {
            ("Состояние", car.IsNew ? "Новый" : "С пробегом"),
            ("Пробег", $"{car.Mileage:N0} км"),
            ("Кузов", car.Body),
            ("Коробка передач", car.Transmission),
            ("Топливо", car.Fuel),
            ("Двигатель", $"{car.EngineVolume:0.0} л, {car.Power} л.с."),
            ("Цвет", car.Color)
        };
        int y = 100;
        foreach (var (name, value) in info)
        {
            Ui.AddLabel(this, name + ":", 40, y, 180);
            Ui.AddLabel(this, value, 230, y, 300);
            y += 28;
        }

        var desc = new TextBox
        {
            Left = 40,
            Top = y + 10,
            Width = 480,
            Height = 90,
            Multiline = true,
            ReadOnly = true,
            Text = car.Description,
            Font = Ui.Normal
        };
        Controls.Add(desc);

        Ui.AddButton(this, "Оставить заявку", 40, 470, 150, 38, (s, e) =>
        {
            using var f = new FormOrder(_car);
            f.ShowDialog(this);
        });
        Ui.AddButton(this, "Тест-драйв", 205, 470, 150, 38, (s, e) =>
        {
            using var f = new FormTestDrive(_car);
            f.ShowDialog(this);
        });
        Ui.AddButton(this, "Закрыть", 370, 470, 150, 38, (s, e) => Close());
    }
}
