using AutoSalonPro.Services;

namespace AutoSalonPro.Models;

// Строки для отображения в таблицах DataGridView
public class CarRow
{
    public int Id { get; }
    public string Brand { get; }
    public string Model { get; }
    public int Year { get; }
    public decimal Price { get; }
    public string Body { get; }
    public string Transmission { get; }
    public string Fuel { get; }
    public int Mileage { get; }
    public string Stock { get; }

    public CarRow(Car c)
    {
        Id = c.Id; Brand = c.Brand; Model = c.Model; Year = c.Year; Price = c.Price;
        Body = c.Body; Transmission = c.Transmission; Fuel = c.Fuel; Mileage = c.Mileage;
        Stock = c.InStock ? "В наличии" : "Продан";
    }
}

public class OrderRow
{
    public int Id { get; }
    public string Car { get; }
    public string Client { get; }
    public string Created { get; }
    public string Payment { get; }
    public decimal Price { get; }
    public string Status { get; }

    public OrderRow(Order o)
    {
        var car = Storage.Db.Cars.FirstOrDefault(c => c.Id == o.CarId);
        var client = Storage.Db.Users.FirstOrDefault(u => u.Id == o.ClientId);
        Id = o.Id;
        Car = car?.Title ?? "—";
        Client = client?.FullName ?? "—";
        Created = o.Created.ToString("dd.MM.yyyy HH:mm");
        Payment = o.PaymentType;
        Price = o.FinalPrice > 0 ? o.FinalPrice : (car?.Price ?? 0);
        Status = o.StatusText;
    }
}

public class DriveRow
{
    public int Id { get; }
    public string Car { get; }
    public string Client { get; }
    public string Date { get; }
    public string Phone { get; }
    public string Status { get; }

    public DriveRow(TestDrive t)
    {
        var car = Storage.Db.Cars.FirstOrDefault(c => c.Id == t.CarId);
        var client = Storage.Db.Users.FirstOrDefault(u => u.Id == t.ClientId);
        Id = t.Id;
        Car = car?.Title ?? "—";
        Client = client?.FullName ?? "—";
        Date = t.Date.ToString("dd.MM.yyyy HH:mm");
        Phone = t.Phone;
        Status = t.StatusText;
    }
}

public class BrandReportRow
{
    public string Brand { get; set; }
    public int Count { get; set; }
    public decimal Sum { get; set; }
}
