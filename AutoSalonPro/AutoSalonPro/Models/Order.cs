using System.Text.Json.Serialization;

namespace AutoSalonPro.Models;

public enum OrderStatus { New, Approved, Rejected, Completed, Cancelled }

public enum TestDriveStatus { Planned, Confirmed, Done, Cancelled }

public class Order
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public int ClientId { get; set; }
    public int ManagerId { get; set; }
    public DateTime Created { get; set; }
    public DateTime? ClosedAt { get; set; }
    public OrderStatus Status { get; set; }
    public string PaymentType { get; set; } = "Наличные";
    public decimal Downpayment { get; set; }
    public int TermMonths { get; set; }
    public decimal Rate { get; set; }
    public decimal FinalPrice { get; set; }
    public string Comment { get; set; } = "";

    [JsonIgnore]
    public string StatusText => Status switch
    {
        OrderStatus.New => "Новая",
        OrderStatus.Approved => "Одобрена",
        OrderStatus.Rejected => "Отклонена",
        OrderStatus.Completed => "Оформлена",
        _ => "Отменена"
    };
}

public class TestDrive
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public int ClientId { get; set; }
    public DateTime Date { get; set; }
    public string Phone { get; set; } = "";
    public TestDriveStatus Status { get; set; }

    [JsonIgnore]
    public string StatusText => Status switch
    {
        TestDriveStatus.Planned => "Ожидает",
        TestDriveStatus.Confirmed => "Подтверждён",
        TestDriveStatus.Done => "Проведён",
        _ => "Отменён"
    };
}

public class Database
{
    public List<Car> Cars { get; set; } = new();
    public List<User> Users { get; set; } = new();
    public List<Order> Orders { get; set; } = new();
    public List<TestDrive> TestDrives { get; set; } = new();
}
