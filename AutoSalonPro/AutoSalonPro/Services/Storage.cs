using System.Text.Json;
using AutoSalonPro.Models;

namespace AutoSalonPro.Services;

public static class Storage
{
    private static readonly string Dir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AutoSalonPro");
    private static readonly string FilePath = Path.Combine(Dir, "data.json");
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public static Database Db { get; private set; } = new();

    public static void Load()
    {
        try
        {
            if (File.Exists(FilePath))
                Db = JsonSerializer.Deserialize<Database>(File.ReadAllText(FilePath), Options) ?? new Database();
        }
        catch
        {
            Db = new Database();
        }
        if (Db.Users.Count == 0)
        {
            Seed();
            Save();
        }
    }

    public static void Save()
    {
        Directory.CreateDirectory(Dir);
        File.WriteAllText(FilePath, JsonSerializer.Serialize(Db, Options));
    }

    public static int NextId(IEnumerable<int> ids) => ids.Any() ? ids.Max() + 1 : 1;

    private static Car AddCar(string brand, string model, int year, decimal price, string body,
        string gear, string fuel, double volume, int power, int km, bool isNew, string color,
        string description, bool inStock = true)
    {
        var car = new Car
        {
            Id = NextId(Db.Cars.Select(c => c.Id)), Brand = brand, Model = model, Year = year,
            Price = price, Body = body, Transmission = gear, Fuel = fuel, EngineVolume = volume,
            Power = power, Mileage = km, IsNew = isNew, Color = color, Description = description,
            InStock = inStock
        };
        Db.Cars.Add(car);
        return car;
    }

    private static void Seed()
    {
        Db.Users.Add(AuthService.CreateUser("manager", "manager123", "Петрова Мария Сергеевна",
            "+79180000001", "manager@autosalon.ru", Role.Manager));
        Db.Users.Add(AuthService.CreateUser("client", "client123", "Иванов Иван Иванович",
            "+79180000002", "ivanov@mail.ru", Role.Client));

        AddCar("Lada", "Vesta", 2024, 1489000, "Седан", "Механика", "Бензин", 1.6, 106, 0, true, "Серый", "Новый автомобиль в комплектации Comfort.");
        AddCar("Kia", "Rio", 2022, 1450000, "Седан", "Автомат", "Бензин", 1.6, 123, 34500, false, "Белый", "Один владелец, сервисная книжка.");
        AddCar("Hyundai", "Solaris", 2021, 1190000, "Седан", "Автомат", "Бензин", 1.6, 123, 52000, false, "Чёрный", "Обслуживался у официального дилера.");
        AddCar("Toyota", "Camry", 2023, 3790000, "Седан", "Автомат", "Бензин", 2.5, 200, 12000, false, "Белый", "Комплектация Prestige, кожаный салон.");
        AddCar("Skoda", "Octavia", 2022, 2350000, "Хэтчбек", "Робот", "Бензин", 1.4, 150, 41000, false, "Синий", "Адаптивный круиз-контроль, климат-контроль.");
        AddCar("Haval", "Jolion", 2024, 2099000, "Кроссовер", "Робот", "Бензин", 1.5, 143, 0, true, "Красный", "Новый автомобиль, гарантия 5 лет.");
        AddCar("Geely", "Coolray", 2024, 2450000, "Кроссовер", "Робот", "Бензин", 1.5, 150, 0, true, "Серый", "Комплектация Flagship, панорамная крыша.");
        AddCar("Volkswagen", "Tiguan", 2021, 2890000, "Кроссовер", "Автомат", "Бензин", 2.0, 180, 63000, false, "Чёрный", "Полный привод, камера заднего вида.");
        AddCar("Toyota", "Land Cruiser Prado", 2020, 5200000, "Внедорожник", "Автомат", "Дизель", 2.8, 177, 88000, false, "Белый", "Полный привод, три ряда сидений.");
        AddCar("Kia", "Sportage", 2023, 3200000, "Кроссовер", "Автомат", "Бензин", 2.0, 150, 21000, false, "Серебристый", "Подогрев руля и сидений.");
        var sold1 = AddCar("Hyundai", "Creta", 2022, 2100000, "Кроссовер", "Автомат", "Бензин", 2.0, 149, 30000, false, "Серый", "Продан.", false);
        var sold2 = AddCar("Toyota", "RAV4", 2021, 3300000, "Кроссовер", "Вариатор", "Бензин", 2.0, 149, 45000, false, "Белый", "Продан.", false);

        Db.Orders.Add(new Order { Id = 1, CarId = sold1.Id, ClientId = 2, ManagerId = 1, Created = DateTime.Today.AddDays(-9), ClosedAt = DateTime.Today.AddDays(-5), Status = OrderStatus.Completed, PaymentType = "Наличные", Downpayment = sold1.Price, FinalPrice = sold1.Price });
        Db.Orders.Add(new Order { Id = 2, CarId = sold2.Id, ClientId = 2, ManagerId = 1, Created = DateTime.Today.AddDays(-16), ClosedAt = DateTime.Today.AddDays(-12), Status = OrderStatus.Completed, PaymentType = "Кредит", Downpayment = 700000, TermMonths = 60, Rate = 16, FinalPrice = sold2.Price });
    }
}
