using AutoSalonPro.Models;
using AutoSalonPro.Services;

namespace AutoSalonPro.Forms;

public class FormMyOrders : Form
{
    private readonly DataGridView _orders, _drives;

    public FormMyOrders()
    {
        Text = "Мои заявки и записи";
        ClientSize = new Size(760, 460);
        StartPosition = FormStartPosition.CenterParent;

        var tabs = new TabControl { Left = 10, Top = 10, Width = 740, Height = 440, Font = Ui.Normal };
        var tabOrders = new TabPage("Заявки на покупку");
        var tabDrives = new TabPage("Тест-драйвы");
        tabs.TabPages.Add(tabOrders);
        tabs.TabPages.Add(tabDrives);
        Controls.Add(tabs);

        _orders = Ui.AddGrid(tabOrders, 10, 10, 710, 330);
        Ui.AddButton(tabOrders, "Отменить заявку", 10, 350, 160, 36, (s, e) => CancelOrder());
        _drives = Ui.AddGrid(tabDrives, 10, 10, 710, 330);
        Ui.AddButton(tabDrives, "Отменить запись", 10, 350, 160, 36, (s, e) => CancelDrive());
        Reload();
    }

    private void Reload()
    {
        int me = AuthService.Current.Id;
        _orders.DataSource = Storage.Db.Orders.Where(o => o.ClientId == me)
            .OrderByDescending(o => o.Created).Select(o => new OrderRow(o)).ToList();
        Ui.Header(_orders, "Id", null);
        Ui.Header(_orders, "Client", null);
        Ui.Header(_orders, "Car", "Автомобиль");
        Ui.Header(_orders, "Created", "Дата заявки");
        Ui.Header(_orders, "Payment", "Оплата");
        Ui.Header(_orders, "Price", "Сумма, ₽");
        Ui.Header(_orders, "Status", "Статус");
        _orders.Columns["Price"].DefaultCellStyle.Format = "N0";

        _drives.DataSource = Storage.Db.TestDrives.Where(t => t.ClientId == me)
            .OrderByDescending(t => t.Date).Select(t => new DriveRow(t)).ToList();
        Ui.Header(_drives, "Id", null);
        Ui.Header(_drives, "Client", null);
        Ui.Header(_drives, "Phone", null);
        Ui.Header(_drives, "Car", "Автомобиль");
        Ui.Header(_drives, "Date", "Дата и время");
        Ui.Header(_drives, "Status", "Статус");
    }

    private void CancelOrder()
    {
        if (_orders.CurrentRow?.DataBoundItem is not OrderRow row) return;
        var order = Storage.Db.Orders.First(o => o.Id == row.Id);
        if (order.Status != OrderStatus.New)
        {
            Ui.Error("Отменить можно только новую заявку");
            return;
        }
        if (!Ui.Ask("Отменить выбранную заявку?")) return;
        order.Status = OrderStatus.Cancelled;
        Storage.Save();
        Reload();
    }

    private void CancelDrive()
    {
        if (_drives.CurrentRow?.DataBoundItem is not DriveRow row) return;
        var drive = Storage.Db.TestDrives.First(t => t.Id == row.Id);
        if (drive.Status != TestDriveStatus.Planned && drive.Status != TestDriveStatus.Confirmed)
        {
            Ui.Error("Эту запись отменить нельзя");
            return;
        }
        if (!Ui.Ask("Отменить запись на тест-драйв?")) return;
        drive.Status = TestDriveStatus.Cancelled;
        Storage.Save();
        Reload();
    }
}
