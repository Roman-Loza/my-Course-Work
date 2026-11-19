using System.Security.Cryptography;
using System.Text;
using AutoSalonPro.Models;

namespace AutoSalonPro.Services;

public static class AuthService
{
    public static User Current { get; private set; }

    public static string Hash(string password, string salt)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(salt + password));
        return Convert.ToHexString(bytes);
    }

    public static User CreateUser(string login, string password, string fullName,
        string phone, string email, Role role)
    {
        var salt = Guid.NewGuid().ToString("N");
        return new User
        {
            Id = Storage.NextId(Storage.Db.Users.Select(u => u.Id)),
            Login = login, Salt = salt, PasswordHash = Hash(password, salt),
            FullName = fullName, Phone = phone, Email = email, Role = role
        };
    }

    public static bool Login(string login, string password, Role role)
    {
        var user = Storage.Db.Users.FirstOrDefault(u =>
            u.Login.Equals(login, StringComparison.OrdinalIgnoreCase) && u.Role == role);
        if (user == null || user.PasswordHash != Hash(password, user.Salt))
            return false;
        Current = user;
        return true;
    }

    // Возвращает текст ошибки или null, если регистрация прошла успешно
    public static string Register(string login, string password, string fullName, string phone, string email)
    {
        if (login.Length < 3) return "Логин должен содержать не менее 3 символов";
        if (password.Length < 6) return "Пароль должен содержать не менее 6 символов";
        if (fullName.Length < 5) return "Введите фамилию, имя и отчество";
        if (!email.Contains('@') || !email.Contains('.')) return "Введите корректный адрес электронной почты";
        if (phone.Count(char.IsDigit) < 10) return "Введите корректный номер телефона";
        if (Storage.Db.Users.Any(u => u.Login.Equals(login, StringComparison.OrdinalIgnoreCase)))
            return "Пользователь с таким логином уже существует";

        Storage.Db.Users.Add(CreateUser(login, password, fullName, phone, email, Role.Client));
        Storage.Save();
        return null;
    }

    public static void Logout() => Current = null;
}
