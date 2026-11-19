using AutoSalonPro.Models;
using AutoSalonPro.Services;

namespace AutoSalonPro.Forms;

public class FormCatalog : Form
{
    private readonly ComboBox _brand, _body, _gear;
    private readonly NumericUpDown _from, _to;
    private readonly TextBox _search;
    private readonly CheckBox _onlyFav;
    private readonly DataGridView _grid;
    private readonly Label _count;

    public FormCatalog()
    {
        Text = $"Каталог автомобилей — {AuthService.Current.FullName}";
        ClientSize = new Size(1000, 620);
        MinimumSize = new Size(1000, 620);
        StartPosition = FormStartPosition.CenterScreen;

        var brands = new[] { "Все" }.Concat(Storage.Db.Cars.Select(c => c.Brand).Distinct().OrderBy(b => b));
        Ui.AddLabel(this, "Марка", 15, 10, 120);
        _brand = Ui.AddCombo(this, 15, 33, 140, brands);
        Ui.AddLabel(this, "Кузов", 170, 10, 120);
        _body = Ui.AddCombo(this, 170, 33, 140, new[] { "Все" }.Concat(Dictionaries.Bodies));
        Ui.AddLabel(this, "КПП", 325, 10, 120);
        _gear = Ui.AddCombo(this, 325, 33, 130, new[] { "Все" }.Concat(Dictionaries.Transmissions));
        Ui.AddLabel(this, "Цена от, ₽", 470, 10, 120);
        _from = Ui.AddNum(this, 470, 33, 120, 0, 100000000, 0, 0, 100000);
        Ui.AddLabel(this, "Цена до, ₽", 605, 10, 120);
        _to = Ui.AddNum(this, 605, 33, 120, 0, 100000000, 100000000, 0, 100000);
        Ui.AddLabel(this, "Поиск", 740, 10, 120);
        _search = Ui.AddText(this, 740, 33, 130);
        _onlyFav = new CheckBox { Text = "Избранное", Left = 885, Top = 34, Width = 100, Font = Ui.Normal };
        Controls.Add(_onlyFav);

        Ui.AddButton(this, "Найти", 15, 72, 100, 30, (s, e) => ApplyFilter());
        Ui.AddButton(this, "Сбросить", 125, 72, 100, 30, (s, e) => ResetFilter());
        _count = Ui.AddLabel(this, "", 240, 76, 300);

        _grid = Ui.AddGrid(this, 15, 112, 970, 440);
        _grid.CellDoubleClick += (s, e) => ShowCard();

        int y = 565;
        var anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        Ui.AddButton(this, "Подробнее", 15, y, 120, 36, (s, e) => ShowCard()).Anchor = anchor;
        Ui.AddButton(this, "В избранное", 145, y, 120, 36, (s, e) => ToggleFavorite()).Anchor = anchor;
        Ui.AddButton(this, "Мои заявки", 275, y, 120, 36, (s, e) => { using var f = new FormMyOrders(); f.ShowDialog(this); }).Anchor = anchor;
        Ui.AddButton(this, "Калькулятор", 405, y, 120, 36, (s, e) => { using var f = new FormCredit(); f.ShowDialog(this); }).Anchor = anchor;
        Ui.AddButton(this, "Выйти", 865, y, 120, 36, (s, e) => Close()).Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var cars = Storage.Db.Cars.Where(c => c.InStock);
        if (_brand.SelectedIndex > 0) cars = cars.Where(c => c.Brand == (string)_brand.SelectedItem);
        if (_body.SelectedIndex > 0) cars = cars.Where(c => c.Body == (string)_body.SelectedItem);
        if (_gear.SelectedIndex > 0) cars = cars.Where(c => c.Transmission == (string)_gear.SelectedItem);
        cars = cars.Where(c => c.Price >= _from.Value && c.Price <= _to.Value);
        var text = _search.Text.Trim();
        if (text.Length > 0)
            cars = cars.Where(c => c.Title.Contains(text, StringComparison.OrdinalIgnoreCase));
        if (_onlyFav.Checked)
            cars = cars.Where(c => AuthService.Current.Favorites.Contains(c.Id));

        var list = cars.OrderBy(c => c.Price).ToList();
        CarGrid.Bind(_grid, list);
        _count.Text = $"Найдено автомобилей: {list.Count}";
    }

    private void ResetFilter()
    {
        _brand.SelectedIndex = 0;
        _body.SelectedIndex = 0;
        _gear.SelectedIndex = 0;
        _from.Value = 0;
        _to.Value = _to.Maximum;
        _search.Text = "";
        _onlyFav.Checked = false;
        ApplyFilter();
    }

    private void ShowCard()
    {
        var car = CarGrid.Selected(_grid);
        if (car == null) return;
        using var card = new FormCarCard(car);
        card.ShowDialog(this);
        ApplyFilter();
    }

    private void ToggleFavorite()
    {
        var car = CarGrid.Selected(_grid);
        if (car == null) return;
        var fav = AuthService.Current.Favorites;
        if (!fav.Remove(car.Id)) fav.Add(car.Id);
        Storage.Save();
        ApplyFilter();
    }
}
