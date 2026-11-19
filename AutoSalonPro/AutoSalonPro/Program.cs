using AutoSalonPro.Forms;
using AutoSalonPro.Services;

namespace AutoSalonPro;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Storage.Load();
        Application.Run(new FormStart());
    }
}
