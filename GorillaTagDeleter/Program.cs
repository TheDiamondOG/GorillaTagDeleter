using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using Microsoft.Win32;

class Program
{
    static void Main(string[] args)
    {
        Console.BackgroundColor = ConsoleColor.DarkRed;
        Console.ForegroundColor = ConsoleColor.White;

        Console.Clear();

        

        if (!IsRunningAsAdmin())
        {
            Console.WriteLine("You need to run the app as admin.");
            Console.ResetColor();
            return;
        }

        Console.WriteLine("Starting to delete Gorilla Tag.");
        DeleteSteamGame(1533390);

        string defaultGorillaTagFolder = @"C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag";
        string appDataFolder = @"C:\Users\xande\AppData\LocalLow\Another Axiom";
        string registryKeyPath = @"Software\Another Axiom";

        Console.WriteLine("Enter the path to the Gorilla Tag folder (C:\\Program Files (x86)\\Steam\\steamapps\\common\\Gorilla Tag): ");
        string userGorillaTagFolder = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(userGorillaTagFolder))
        {
            userGorillaTagFolder = defaultGorillaTagFolder;
        }

        DeleteFolder(userGorillaTagFolder);

        DeleteFolder(appDataFolder);

        DeleteRegistryKey(registryKeyPath);

        Console.WriteLine("Do you want to reinstall Gorilla Tag (Y/n): ");
        string reinstallGame = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(reinstallGame) || reinstallGame.ToLower().Replace(" ", "") == "y")
        {
            DownloadSteamGame(1533390);
        }
        Console.ResetColor();
    }

    static bool IsRunningAsAdmin()
    {
        using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
        {
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }

    static void DeleteFolder(string folderPath)
    {
        try
        {
            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true);
                Console.WriteLine($"Successfully deleted folder: {folderPath}");
            }
            else
            {
                Console.WriteLine($"Folder not found: {folderPath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting folder: {folderPath}\n{ex.Message}");
        }
    }

    static void DeleteRegistryKey(string keyPath)
    {
        try
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyPath, true))
            {
                if (key != null)
                {
                    Registry.CurrentUser.DeleteSubKeyTree(keyPath);
                    Console.WriteLine($"Successfully deleted registry key: HKEY_CURRENT_USER\\{keyPath}");
                }
                else
                {
                    Console.WriteLine($"Registry key not found: HKEY_CURRENT_USER\\{keyPath}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting registry key: HKEY_CURRENT_USER\\{keyPath}\n{ex.Message}");
        }
    }

    static void DeleteSteamGame(int steamId)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = $"steam://uninstall/{steamId}",
                UseShellExecute = true
            });
            Console.WriteLine($"Opened Steam to uninstall game with AppID: {steamId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error launching Steam uninstall URL: steam://uninstall/{steamId}\n{ex.Message}");
        }
    }

    static void DownloadSteamGame(int steamId)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = $"steam://install/{steamId}",
                UseShellExecute = true
            });
            Console.WriteLine($"Opened Steam to install game with AppID: {steamId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error launching Steam install URL: steam://install/{steamId}\n{ex.Message}");
        }
    }
}
