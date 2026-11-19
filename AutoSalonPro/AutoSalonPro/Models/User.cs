namespace AutoSalonPro.Models;

public enum Role
{
    Client,
    Manager
}

public class User
{
    public int Id { get; set; }
    public string Login { get; set; } = "";
    public string Salt { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Email { get; set; } = "";
    public Role Role { get; set; }
    public List<int> Favorites { get; set; } = new();
}
