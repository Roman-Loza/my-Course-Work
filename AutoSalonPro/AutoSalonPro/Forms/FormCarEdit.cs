using AutoSalonPro.Models;
using AutoSalonPro.Services;

namespace AutoSalonPro.Forms;

public class FormCarEdit : Form
{
    private readonly Car _car;
    private readonly bool _isNewCar;
    private readonly TextBox _brand, _model, _color, _desc;
    private readonly NumericUpDown _year, _price, _volume, _power, _km;
    private readonly ComboBox _body, _gear, _fuel;
    private readonly CheckBox _isNew, _inStock;

    public FormCarEdit(Car car)
    {
        _isNewCar = car == null;
        _car = car ?? new Car { Year = DateTime.Now.Year };
        Text = _isNewCar ? "Добавление автомобиля" : "Редактирование автомобиля";
        ClientSize = new Size(560, 600);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        Ui.AddLabel(this, Text, 20, 10, 520, true);
        Ui.AddLabel(this, "Марка", 30, 55, 200);
        _brand = Ui.AddText(this, 30, 78, 230);
        Ui.AddLabel(this, "Модель", 290, 55, 200);
        _model = Ui.AddText(this, 290, 78, 230);
        Ui.AddLabel(this, "Год выпуска", 30, 115, 200);
        _year = Ui.AddNum(this, 30, 138, 230, 1990, DateTime.Now.Year + 1, DateTime.Now.Year);
        _year.ThousandsSeparator = false;
        Ui.AddLabel(this, "Цена, ₽", 290, 115, 200);
        _price = Ui.AddNum(this, 290, 138, 230, 0, 100000000, 1000000, 0, 10000);
        Ui.AddLabel(this, "Кузов", 30, 175, 200);
        _body = Ui.AddCombo(this, 30, 198, 230, Dictionaries.Bodies);
        Ui.AddLabel(this, "Коробка передач", 290, 175, 200);
        _gear = Ui.AddCombo(this, 290, 198, 230, Dictionaries.Transmissions);
        Ui.AddLabel(this, "Топливо", 30, 235, 200);
        _fuel = Ui.AddCombo(this, 30, 258, 230, Dictionaries.Fuels);
        Ui.AddLabel(this, "Цвет", 290, 235, 200);
        _color = Ui.AddText(this, 290, 258, 230);
        Ui.AddLabel(this, "Объём двигателя, л", 30, 295, 200);
        _volume = Ui.AddNum(this, 30, 318, 230, 0, 10, 1.6m, 1, 0.1m);
        Ui.AddLabel(this, "Мощность, л.с.", 290, 295, 200);
        _power = Ui.AddNum(this, 290, 318, 230, 0, 2000, 100);
        Ui.AddLabel(this, "Пробег, км", 30, 355, 200);
        _km = Ui.AddNum(this, 30, 378, 230, 0, 1000000, 0, 0, 1000);
        _isNew = new CheckBox { Text = "Новый автомобиль", Left = 290, Top = 380, Width = 230, Font = Ui.Normal };
        Controls.Add(_isNew);
        _inStock = new CheckBox { Text = "В наличии", Left = 290, Top = 410, Width = 230, Font = Ui.Normal, Checked = true };
        Controls.Add(_inStock);
        Ui.AddLabel(this, "Описание", 30, 415, 200);
        _desc = new TextBox { Left = 30, Top = 438, Width = 490, Height = 70, Multiline = true, Font = Ui.Normal };
        Controls.Add(_desc);
        Ui.AddButton(this, "Сохранить", 190, 535, 180, 40, OnSave);

        if (!_isNewCar) Fill();
    }

    private void Fill()
    {
        _brand.Text = _car.Brand;
        _model.Text = _car.Model;
        _year.Value = _car.Year;
        _price.Value = _car.Price;
        _body.SelectedItem = _car.Body;
        _gear.SelectedItem = _car.Transmission;
        _fuel.SelectedItem = _car.Fuel;
        _color.Text = _car.Color;
        _volume.Value = (decimal)_car.EngineVolume;
        _power.Value = _car.Power;
        _km.Value = _car.Mileage;
        _isNew.Checked = _car.IsNew;
        _inStock.Checked = _car.InStock;
        _desc.Text = _car.Description;
    }

    private void OnSave(object sender, EventArgs e)
    {
        if (_brand.Text.Trim().Length == 0 || _model.Text.Trim().Length == 0)
        {
            Ui.Error("Укажите марку и модель автомобиля");
            return;
        }
        if (_price.Value <= 0)
        {
            Ui.Error("Цена должна быть больше нуля");
            return;
        }
        _car.Brand = _brand.Text.Trim();
        _car.Model = _model.Text.Trim();
        _car.Year = (int)_year.Value;
        _car.Price = _price.Value;
        _car.Body = (string)_body.SelectedItem;
        _car.Transmission = (string)_gear.SelectedItem;
        _car.Fuel = (string)_fuel.SelectedItem;
        _car.Color = _color.Text.Trim();
        _car.EngineVolume = (double)_volume.Value;
        _car.Power = (int)_power.Value;
        _car.Mileage = (int)_km.Value;
        _car.IsNew = _isNew.Checked;
        _car.InStock = _inStock.Checked;
        _car.Description = _desc.Text.Trim();

        if (_isNewCar)
        {
            _car.Id = Storage.NextId(Storage.Db.Cars.Select(c => c.Id));
            Storage.Db.Cars.Add(_car);
        }
        Storage.Save();
        DialogResult = DialogResult.OK;
    }
}
