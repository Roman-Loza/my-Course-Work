using AutoSalonPro.Models;
using AutoSalonPro.Services;

namespace AutoSalonPro.Forms;

public class FormStart : Form
{
    public FormStart()
    {
        Text = "АвтоСалон Pro";
        ClientSize = new Size(480, 260);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;

        Ui.AddLabel(this, "АвтоСалон Pro", 20, 20, 440, true);
        Ui.AddLabel(this, "Кто вы?", 20, 65, 440).TextAlign = ContentAlignment.MiddleCenter;

        var client = Ui.AddButton(this, "Покупатель", 40, 110, 180, 90, (s, e) => Open(Role.Client));
        client.BackColor = Color.Gold;
        var manager = Ui.AddButton(this, "Сотрудник", 260, 110, 180, 90, (s, e) => Open(Role.Manager));
        manager.BackColor = Color.LightGreen;
    }

    private void Open(Role role)
    {
        using var login = new FormLogin(role);
        if (login.ShowDialog(this) != DialogResult.OK) return;

        Hide();
        using (var main = role == Role.Client ? (Form)new FormCatalog() : new FormManager())
        {
            main.ShowDialog();
        }
        AuthService.Logout();
        Show();
    }
}
