using System.Text.Json.Serialization;

namespace AutoSalonPro.Models;

public class Car
{
    public int Id { get; set; }
    public string Brand { get; set; } = "";
    public string Model { get; set; } = "";
    public int Year { get; set; }
    public decimal Price { get; set; }
    public string Body { get; set; } = "Седан";
    public string Transmission { get; set; } = "Автомат";
    public string Fuel { get; set; } = "Бензин";
    public string Color { get; set; } = "";
    public double EngineVolume { get; set; }
    public int Power { get; set; }
    public int Mileage { get; set; }
    public bool IsNew { get; set; }
    public bool InStock { get; set; } = true;
    public string Description { get; set; } = "";

    [JsonIgnore]
    public string Title => $"{Brand} {Model}";
}

public static class Dictionaries
{
    public static readonly string[] Bodies =
        { "Седан", "Хэтчбек", "Универсал", "Кроссовер", "Внедорожник", "Купе", "Минивэн" };
    public static readonly string[] Transmissions = { "Механика", "Автомат", "Робот", "Вариатор" };
    public static readonly string[] Fuels = { "Бензин", "Дизель", "Гибрид", "Электро" };
}
