using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Windows;

namespace IronConvert;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        EnsureDependenciesInstalled();
        InitializeComponent();
    }

    private static void EnsureDependenciesInstalled()
    {
        (bool pandoc, bool ffmpeg) = (ProgramExists("pandoc"), ProgramExists("ffmpeg"));
        if (pandoc && ffmpeg) return;

        StringBuilder message = new("You need to install extra program(s) to use IronConvert:\n\n");
        if (!pandoc) message.Append("* Pandoc - Document formatting tools\n");
        if (!ffmpeg) message.Append("* FFmpeg - Multimedia codec collection\n\n");
        message.Append(
            """
            Do you want to install the required programs now?

            Click YES to install the programs by winget automatically.
            Click NO to do nothing and close this program immediately.
            """);

        if (MessageBox.Show(message.ToString(), "Additional Programs Required.", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
        {
            if (!pandoc) _ = WingetInstall("--source winget --exact --id JohnMacFarlane.Pandoc");
            if (!ffmpeg) _ = WingetInstall("--id Gyan.FFmpeg");
        }
        else { Environment.Exit(-1); }
    }




    private static bool ProgramExists(string programName)
    {
        try
        {
            ProcessStartInfo psi = new(programName)
            {
                CreateNoWindow = true,
                Arguments = "--version"
            };

            using Process? program = Process.Start(psi);
            program?.WaitForExit();
            return true;
        }
        catch { return false; }
    }

    private static async Task WingetInstall(string programName)
    {
        using Process program = Process.Start("winget", "install " + programName);
        await program.WaitForExitAsync();
    }
}

