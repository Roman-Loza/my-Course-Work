using AutoSalonPro.Models;
using AutoSalonPro.Services;

namespace AutoSalonPro.Forms;

// Вспомогательные методы для создания элементов управления
public static class Ui
{
    public static readonly Font Normal = new("Segoe UI", 10F);
    public static readonly Font Title = new("Segoe UI", 14F, FontStyle.Bold);

    public static Label AddLabel(Control parent, string text, int x, int y, int w = 160, bool title = false)
    {
        var l = new Label
        {
            Text = text, Left = x, Top = y, Width = w,
            Height = title ? 32 : 22, Font = title ? Title : Normal,
            TextAlign = title ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleLeft
        };
        parent.Controls.Add(l);
        return l;
    }

    public static TextBox AddText(Control parent, int x, int y, int w = 200, bool password = false)
    {
        var t = new TextBox { Left = x, Top = y, Width = w, Font = Normal, UseSystemPasswordChar = password };
        parent.Controls.Add(t);
        return t;
    }

    public static Button AddButton(Control parent, string text, int x, int y, int w, int h, EventHandler click)
    {
        var b = new Button { Text = text, Left = x, Top = y, Width = w, Height = h, Font = Normal };
        if (click != null) b.Click += click;
        parent.Controls.Add(b);
        return b;
    }

    public static ComboBox AddCombo(Control parent, int x, int y, int w, IEnumerable<string> items)
    {
        var c = new ComboBox { Left = x, Top = y, Width = w, DropDownStyle = ComboBoxStyle.DropDownList, Font = Normal };
        c.Items.AddRange(items.Cast<object>().ToArray());
        if (c.Items.Count > 0) c.SelectedIndex = 0;
        parent.Controls.Add(c);
        return c;
    }

    public static NumericUpDown AddNum(Control parent, int x, int y, int w, decimal min, decimal max,
        decimal value, int decimals = 0, decimal step = 1)
    {
        var n = new NumericUpDown
        {
            Left = x, Top = y, Width = w, Font = Normal, Minimum = min, Maximum = max,
            DecimalPlaces = decimals, Increment = step, ThousandsSeparator = true
        };
        n.Value = Math.Min(Math.Max(value, min), max);
        parent.Controls.Add(n);
        return n;
    }

    public static DataGridView AddGrid(Control parent, int x, int y, int w, int h)
    {
        var g = new DataGridView
        {
            Left = x, Top = y, Width = w, Height = h, Font = Normal,
            ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            RowHeadersVisible = false, BackgroundColor = Color.White,
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
        };
        parent.Controls.Add(g);
        return g;
    }

    public static void Info(string text) =>
        MessageBox.Show(text, "АвтоСалон Pro", MessageBoxButtons.OK, MessageBoxIcon.Information);

    public static void Error(string text) =>
        MessageBox.Show(text, "АвтоСалон Pro", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    public static bool Ask(string text) =>
        MessageBox.Show(text, "АвтоСалон Pro", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

    public static void Header(DataGridView grid, string column, string text)
    {
        var col = grid.Columns[column];
        if (col == null) return;
        if (text == null) col.Visible = false;
        else col.HeaderText = text;
    }
}

// Работа с таблицей автомобилей
public static class CarGrid
{
    public static void Bind(DataGridView grid, IEnumerable<Car> cars)
    {
        grid.DataSource = cars.Select(c => new CarRow(c)).ToList();
        Ui.Header(grid, "Id", null);
        Ui.Header(grid, "Brand", "Марка");
        Ui.Header(grid, "Model", "Модель");
        Ui.Header(grid, "Year", "Год");
        Ui.Header(grid, "Price", "Цена, ₽");
        Ui.Header(grid, "Body", "Кузов");
        Ui.Header(grid, "Transmission", "КПП");
        Ui.Header(grid, "Fuel", "Топливо");
        Ui.Header(grid, "Mileage", "Пробег, км");
        Ui.Header(grid, "Stock", "Статус");
        grid.Columns["Price"].DefaultCellStyle.Format = "N0";
        grid.Columns["Mileage"].DefaultCellStyle.Format = "N0";
    }

    public static Car Selected(DataGridView grid)
    {
        if (grid.CurrentRow?.DataBoundItem is CarRow row)
            return Storage.Db.Cars.FirstOrDefault(c => c.Id == row.Id);
        return null;
    }
}
