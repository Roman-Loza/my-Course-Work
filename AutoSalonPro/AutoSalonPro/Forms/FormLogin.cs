using AutoSalonPro.Models;
using AutoSalonPro.Services;

namespace AutoSalonPro.Forms;

public class FormLogin : Form
{
    private readonly Role _role;
    private readonly TextBox _login;
    private readonly TextBox _password;

    public FormLogin(Role role)
    {
        _role = role;
        Text = "АвтоСалон Pro — вход";
        ClientSize = new Size(380, 290);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        Ui.AddLabel(this, role == Role.Client ? "Вход для покупателей" : "Вход для сотрудников", 20, 20, 340, true);
        Ui.AddLabel(this, "Логин", 40, 70);
        _login = Ui.AddText(this, 40, 94, 300);
        Ui.AddLabel(this, "Пароль", 40, 130);
        _password = Ui.AddText(this, 40, 154, 300, true);
        AcceptButton = Ui.AddButton(this, "Войти", 120, 200, 140, 36, OnEnter);

        if (role == Role.Client)
        {
            var link = new LinkLabel { Text = "Регистрация", Left = 150, Top = 250, AutoSize = true, Font = Ui.Normal };
            link.Click += (s, e) =>
            {
                using var form = new FormRegister();
                form.ShowDialog(this);
            };
            Controls.Add(link);
        }
    }

    private void OnEnter(object sender, EventArgs e)
    {
        if (AuthService.Login(_login.Text.Trim(), _password.Text, _role))
            DialogResult = DialogResult.OK;
        else
            Ui.Error("Неверный логин или пароль");
    }
}
