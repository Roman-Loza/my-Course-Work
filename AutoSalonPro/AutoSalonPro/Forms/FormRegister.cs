using AutoSalonPro.Services;

namespace AutoSalonPro.Forms;

public class FormRegister : Form
{
    private readonly TextBox _name, _phone, _email, _login, _password, _repeat;

    public FormRegister()
    {
        Text = "Регистрация покупателя";
        ClientSize = new Size(400, 560);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        Ui.AddLabel(this, "Регистрация", 20, 15, 360, true);
        Ui.AddLabel(this, "ФИО", 40, 60, 300);
        _name = Ui.AddText(this, 40, 84, 320);
        Ui.AddLabel(this, "Телефон", 40, 120, 300);
        _phone = Ui.AddText(this, 40, 144, 320);
        Ui.AddLabel(this, "Электронная почта", 40, 180, 300);
        _email = Ui.AddText(this, 40, 204, 320);
        Ui.AddLabel(this, "Создайте логин", 40, 240, 300);
        _login = Ui.AddText(this, 40, 264, 320);
        Ui.AddLabel(this, "Создайте пароль", 40, 300, 300);
        _password = Ui.AddText(this, 40, 324, 320, true);
        Ui.AddLabel(this, "Повторите пароль", 40, 360, 300);
        _repeat = Ui.AddText(this, 40, 384, 320, true);
        AcceptButton = Ui.AddButton(this, "Продолжить", 120, 450, 160, 38, OnRegister);
    }

    private void OnRegister(object sender, EventArgs e)
    {
        if (_password.Text != _repeat.Text)
        {
            Ui.Error("Пароли не совпадают");
            return;
        }
        var error = AuthService.Register(_login.Text.Trim(), _password.Text, _name.Text.Trim(),
            _phone.Text.Trim(), _email.Text.Trim());
        if (error != null)
        {
            Ui.Error(error);
            return;
        }
        Ui.Info("Регистрация выполнена. Теперь вы можете войти в систему.");
        DialogResult = DialogResult.OK;
    }
}
