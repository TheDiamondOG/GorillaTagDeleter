using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using Microsoft.Win32;
using System.Net.Http;
using System.IO.Compression;
using System.Threading.Tasks;

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

        Console.WriteLine("Checking if Gorilla Tag is open");

        KillGorillaTag();

        Console.WriteLine("Starting to delete Gorilla Tag.");
        DeleteSteamGame(1533390);

        string user = Environment.GetEnvironmentVariable("USERNAME");

        string defaultGorillaTagFolder = @"C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag";
        string appDataFolder = $@"C:\Users\{user}\AppData\LocalLow\Another Axiom";
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

            Console.WriteLine("Do you want to install Bepinex (Y/n): ");
            string installBepinex = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(installBepinex) || installBepinex.ToLower().Replace(" ", "") == "y")
            {
                ReinstallBepinex(userGorillaTagFolder);

                Console.WriteLine("Hit enter after it says Installed Bepinex");
                Console.ReadLine();

                Console.WriteLine("Do you want to install Unity Explorer (Y/n): ");
                string installUnityExplorer = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(installUnityExplorer) || installUnityExplorer.ToLower().Replace(" ", "") == "y")
                {
                    InstallUnityExplorer(userGorillaTagFolder);

                    Console.WriteLine("Hit enter after it says Installed Unity Explorer");
                    Console.ReadLine();
                }
                
            }
        }

        Console.ResetColor();
    }

    public static void KillGorillaTag()
    {
        try
        {
            Process[] processes = Process.GetProcessesByName("Gorilla Tag");

            if (processes.Length > 0)
            {
                foreach (var process in processes)
                {
                    Console.WriteLine($"Gorilla Tag is still opening, killing the process.");
                    process.Kill();
                    Console.WriteLine($"Killed Gorilla Tag");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public async static Task ReinstallBepinex(string path)
    {
        string zipUrl = "https://github.com/BepInEx/BepInEx/releases/download/v5.4.23.2/BepInEx_win_x64_5.4.23.2.zip";
        string tempZipFile = Path.Combine(Path.GetTempPath(), "BepInEx.zip");

        try
        {
            using (HttpClient client = new HttpClient())
            {
                Console.WriteLine("Grabbing Bepinex...");
                byte[] zipData = await client.GetByteArrayAsync(zipUrl);
                await File.WriteAllBytesAsync(tempZipFile, zipData);
                Console.WriteLine("Grabbed Bepinex");
            }

            Console.WriteLine("Installing Bepinex");
            ZipFile.ExtractToDirectory(tempZipFile, path);
            Console.WriteLine($"Installed Bepinex");

            File.Delete(tempZipFile);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
    public async static Task InstallUnityExplorer(string path)
    {
        string zipUrl = "https://github.com/sinai-dev/UnityExplorer/releases/download/4.9.0/UnityExplorer.BepInEx5.Mono.zip";
        string tempZipFile = Path.Combine(Path.GetTempPath(), "UnityExplorer.zip");

        try
        {
            using (HttpClient client = new HttpClient())
            {
                Console.WriteLine("Grabbing Unity Explorer...");
                byte[] zipData = await client.GetByteArrayAsync(zipUrl);
                await File.WriteAllBytesAsync(tempZipFile, zipData);
                Console.WriteLine("Grabbed Unity Explorer");
            }

            Console.WriteLine("Installing Unity Explorer");
            ZipFile.ExtractToDirectory(tempZipFile, path+"/Bepinex/");
            Console.WriteLine($"Installed Unity Explorer");

            File.Delete(tempZipFile);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
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
