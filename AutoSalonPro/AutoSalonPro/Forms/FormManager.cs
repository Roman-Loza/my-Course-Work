using System.Text;
using AutoSalonPro.Models;
using AutoSalonPro.Services;

namespace AutoSalonPro.Forms;

public class FormManager : Form
{
    private DataGridView _gridCars, _gridOrders, _gridDrives, _gridReport;
    private ComboBox _statusFilter;
    private DateTimePicker _from, _to;
    private Label _summary;
    private List<BrandReportRow> _report = new();

    public FormManager()
    {
        Text = $"Управление автосалоном — {AuthService.Current.FullName}";
        ClientSize = new Size(1050, 640);
        MinimumSize = new Size(1050, 640);
        StartPosition = FormStartPosition.CenterScreen;

        var tabs = new TabControl { Dock = DockStyle.Fill, Font = Ui.Normal };
        tabs.TabPages.Add(BuildCarsTab());
        tabs.TabPages.Add(BuildOrdersTab());
        tabs.TabPages.Add(BuildDrivesTab());
        tabs.TabPages.Add(BuildReportTab());
        Controls.Add(tabs);

        RefreshCars();
        RefreshOrders();
        RefreshDrives();
    }

    private static Button Bottom(Control parent, string text, int x, EventHandler click)
    {
        var b = Ui.AddButton(parent, text, x, 535, 170, 36, click);
        b.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        return b;
    }

    // ---------- Автомобили ----------
    private TabPage BuildCarsTab()
    {
        var tab = new TabPage("Автомобили");
        _gridCars = Ui.AddGrid(tab, 10, 10, 1010, 510);
        Bottom(tab, "Добавить", 10, (s, e) => EditCar(false));
        Bottom(tab, "Изменить", 190, (s, e) => EditCar(true));
        Bottom(tab, "Удалить", 370, (s, e) => DeleteCar());
        return tab;
    }

    private void RefreshCars() =>
        CarGrid.Bind(_gridCars, Storage.Db.Cars.OrderBy(c => c.Brand).ThenBy(c => c.Model));

    private void EditCar(bool existing)
    {
        Car car = null;
        if (existing)
        {
            car = CarGrid.Selected(_gridCars);
            if (car == null) return;
        }
        using var form = new FormCarEdit(car);
        if (form.ShowDialog(this) == DialogResult.OK) RefreshCars();
    }

    private void DeleteCar()
    {
        var car = CarGrid.Selected(_gridCars);
        if (car == null) return;
        if (Storage.Db.Orders.Any(o => o.CarId == car.Id))
        {
            Ui.Error("Нельзя удалить автомобиль, по которому есть заявки. Снимите его с продажи.");
            return;
        }
        if (!Ui.Ask($"Удалить автомобиль {car.Title}?")) return;
        Storage.Db.Cars.Remove(car);
        Storage.Db.TestDrives.RemoveAll(t => t.CarId == car.Id);
        foreach (var u in Storage.Db.Users) u.Favorites.Remove(car.Id);
        Storage.Save();
        RefreshCars();
    }

    // ---------- Заявки ----------
    private TabPage BuildOrdersTab()
    {
        var tab = new TabPage("Заявки");
        Ui.AddLabel(tab, "Статус:", 10, 12, 60);
        _statusFilter = Ui.AddCombo(tab, 75, 10, 160,
            new[] { "Все", "Новая", "Одобрена", "Отклонена", "Оформлена", "Отменена" });
        _statusFilter.SelectedIndexChanged += (s, e) => RefreshOrders();
        _gridOrders = Ui.AddGrid(tab, 10, 45, 1010, 475);
        Bottom(tab, "Одобрить", 10, (s, e) => Move(OrderStatus.New, OrderStatus.Approved));
        Bottom(tab, "Отклонить", 190, (s, e) => Move(OrderStatus.New, OrderStatus.Rejected));
        Bottom(tab, "Оформить продажу", 370, (s, e) => CompleteSale());
        return tab;
    }

    private void RefreshOrders()
    {
        var orders = Storage.Db.Orders.AsEnumerable();
        if (_statusFilter.SelectedIndex > 0)
            orders = orders.Where(o => o.StatusText == (string)_statusFilter.SelectedItem);
        _gridOrders.DataSource = orders.OrderByDescending(o => o.Created).Select(o => new OrderRow(o)).ToList();
        Ui.Header(_gridOrders, "Id", "№");
        Ui.Header(_gridOrders, "Car", "Автомобиль");
        Ui.Header(_gridOrders, "Client", "Клиент");
        Ui.Header(_gridOrders, "Created", "Дата заявки");
        Ui.Header(_gridOrders, "Payment", "Оплата");
        Ui.Header(_gridOrders, "Price", "Сумма, ₽");
        Ui.Header(_gridOrders, "Status", "Статус");
        _gridOrders.Columns["Price"].DefaultCellStyle.Format = "N0";
    }

    private Order SelectedOrder() =>
        _gridOrders.CurrentRow?.DataBoundItem is OrderRow row
            ? Storage.Db.Orders.FirstOrDefault(o => o.Id == row.Id)
            : null;

    private void Move(OrderStatus from, OrderStatus to)
    {
        var order = SelectedOrder();
        if (order == null) return;
        if (order.Status != from)
        {
            Ui.Error("Действие недоступно для заявки с текущим статусом");
            return;
        }
        order.Status = to;
        order.ManagerId = AuthService.Current.Id;
        Storage.Save();
        RefreshOrders();
    }

    private void CompleteSale()
    {
        var order = SelectedOrder();
        if (order == null) return;
        if (order.Status != OrderStatus.Approved)
        {
            Ui.Error("Оформить продажу можно только по одобренной заявке");
            return;
        }
        var car = Storage.Db.Cars.FirstOrDefault(c => c.Id == order.CarId);
        if (car == null || !car.InStock)
        {
            Ui.Error("Автомобиль отсутствует в наличии");
            return;
        }
        order.Status = OrderStatus.Completed;
        order.FinalPrice = car.Price;
        order.ClosedAt = DateTime.Now;
        order.ManagerId = AuthService.Current.Id;
        car.InStock = false;
        foreach (var other in Storage.Db.Orders.Where(o => o.CarId == car.Id && o.Id != order.Id
                     && (o.Status == OrderStatus.New || o.Status == OrderStatus.Approved)))
            other.Status = OrderStatus.Rejected;

        Storage.Save();
        RefreshOrders();
        RefreshCars();
        Ui.Info("Продажа оформлена");
    }

    // ---------- Тест-драйвы ----------
    private TabPage BuildDrivesTab()
    {
        var tab = new TabPage("Тест-драйвы");
        _gridDrives = Ui.AddGrid(tab, 10, 10, 1010, 510);
        Bottom(tab, "Подтвердить", 10, (s, e) => SetDrive(TestDriveStatus.Planned, TestDriveStatus.Confirmed));
        Bottom(tab, "Проведён", 190, (s, e) => SetDrive(TestDriveStatus.Confirmed, TestDriveStatus.Done));
        Bottom(tab, "Отменить", 370, (s, e) => SetDrive(null, TestDriveStatus.Cancelled));
        return tab;
    }

    private void RefreshDrives()
    {
        _gridDrives.DataSource = Storage.Db.TestDrives.OrderBy(t => t.Date)
            .Select(t => new DriveRow(t)).ToList();
        Ui.Header(_gridDrives, "Id", "№");
        Ui.Header(_gridDrives, "Car", "Автомобиль");
        Ui.Header(_gridDrives, "Client", "Клиент");
        Ui.Header(_gridDrives, "Date", "Дата и время");
        Ui.Header(_gridDrives, "Phone", "Телефон");
        Ui.Header(_gridDrives, "Status", "Статус");
    }

    private void SetDrive(TestDriveStatus? from, TestDriveStatus to)
    {
        if (_gridDrives.CurrentRow?.DataBoundItem is not DriveRow row) return;
        var drive = Storage.Db.TestDrives.First(t => t.Id == row.Id);
        if (from.HasValue && drive.Status != from.Value)
        {
            Ui.Error("Действие недоступно для записи с текущим статусом");
            return;
        }
        if (drive.Status == TestDriveStatus.Done || drive.Status == TestDriveStatus.Cancelled)
        {
            Ui.Error("Запись уже закрыта");
            return;
        }
        drive.Status = to;
        Storage.Save();
        RefreshDrives();
    }

    // ---------- Отчёт ----------
    private TabPage BuildReportTab()
    {
        var tab = new TabPage("Отчёт по продажам");
        Ui.AddLabel(tab, "Период с", 10, 14, 70);
        _from = new DateTimePicker
        {
            Left = 85, Top = 10, Width = 130, Font = Ui.Normal, Format = DateTimePickerFormat.Short,
            Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)
        };
        Ui.AddLabel(tab, "по", 225, 14, 30);
        _to = new DateTimePicker
        {
            Left = 260, Top = 10, Width = 130, Font = Ui.Normal, Format = DateTimePickerFormat.Short,
            Value = DateTime.Today
        };
        tab.Controls.Add(_from);
        tab.Controls.Add(_to);
        Ui.AddButton(tab, "Сформировать", 410, 8, 150, 32, (s, e) => BuildReport());
        Ui.AddButton(tab, "Экспорт в CSV", 575, 8, 150, 32, (s, e) => ExportCsv());
        _summary = Ui.AddLabel(tab, "", 10, 55, 1000);
        _gridReport = Ui.AddGrid(tab, 10, 90, 1010, 430);
        return tab;
    }

    private void BuildReport()
    {
        var sales = ReportService.Sales(_from.Value, _to.Value);
        _report = ReportService.ByBrand(sales);
        var sum = sales.Sum(o => o.FinalPrice);
        var avg = sales.Count > 0 ? sum / sales.Count : 0;
        _summary.Text = $"Продано автомобилей: {sales.Count}; выручка: {sum:N0} ₽; средний чек: {avg:N0} ₽";
        _gridReport.DataSource = _report;
        Ui.Header(_gridReport, "Brand", "Марка");
        Ui.Header(_gridReport, "Count", "Продано, шт.");
        Ui.Header(_gridReport, "Sum", "Выручка, ₽");
        _gridReport.Columns["Sum"].DefaultCellStyle.Format = "N0";
    }

    private void ExportCsv()
    {
        if (_report.Count == 0)
        {
            Ui.Error("Сначала сформируйте отчёт");
            return;
        }
        using var dialog = new SaveFileDialog { Filter = "CSV (*.csv)|*.csv", FileName = "report.csv" };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        var lines = new List<string> { "Марка;Продано;Выручка" };
        lines.AddRange(_report.Select(r => $"{r.Brand};{r.Count};{r.Sum:0}"));
        File.WriteAllLines(dialog.FileName, lines, new UTF8Encoding(true));
        Ui.Info("Отчёт сохранён");
    }
}
