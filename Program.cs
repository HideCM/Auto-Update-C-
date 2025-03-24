using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Collections.Generic;

class Program
{
    private static readonly HttpClient client = new HttpClient();
    private static readonly string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
    private static readonly string versionFile = Path.Combine(currentDirectory, "version.txt");
    private static readonly string ignoreFile = Path.Combine(currentDirectory, "ignore.txt");
    private static readonly string updateZip = Path.Combine(currentDirectory, "update.zip");

    // Console colors
    private static readonly ConsoleColor[] colors = new ConsoleColor[]
    {
        ConsoleColor.Cyan,
        ConsoleColor.Green,
        ConsoleColor.Yellow,
        ConsoleColor.Magenta,
        ConsoleColor.Red,
        ConsoleColor.Blue
    };

    static async Task Main(string[] args)
    {
        try
        {
            // Show animated banner
            await ShowAnimatedBanner();

            // Show main menu
            ShowMainMenu();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            ShowError($"An error occurred: {ex.Message}");
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }

    private static async Task ShowAnimatedBanner()
    {
        string[] banner = new string[]
        {
            "╔════════════════════════════════════════════════════════════════════════════╗",
            "║                                                                             ║",
            "║   █████╗ ██╗   ██╗████████╗ ██████╗     ██╗   ██╗██████╗ ██████╗ ███████╗  ║",
            "║  ██╔══██╗██║   ██║╚══██╔══╝██╔═══██╗    ██║   ██║██╔══██╗██╔══██╗██╔════╝  ║",
            "║  ███████║██║   ██║   ██║   ██║   ██║    ██║   ██║██████╔╝██║  ██║█████╗    ║",
            "║  ██╔══██║██║   ██║   ██║   ██║   ██║    ╚██╗ ██╔╝██╔══██╗██║  ██║██╔══╝    ║",
            "║  ██║  ██║╚██████╔╝   ██║   ╚██████╔╝     ╚████╔╝ ██║  ██║██████╔╝███████╗  ║",
            "║  ╚═╝  ╚═╝ ╚═════╝    ╚═╝    ╚═════╝       ╚═══╝  ╚═╝  ╚═╝╚═════╝ ╚══════╝  ║",
            "║                                                                             ║",
            "╚════════════════════════════════════════════════════════════════════════════╝"
        };

        for (int i = 0; i < banner.Length; i++)
        {
            Console.ForegroundColor = colors[i % colors.Length];
            Console.WriteLine(banner[i]);
            await Task.Delay(100);
        }

        Console.ResetColor();
        Console.WriteLine("\n");
    }

    private static void ShowMainMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== AUTO UPDATE TOOL ===\n");
            Console.ResetColor();

            Console.WriteLine("1. Check for updates");
            Console.WriteLine("2. Show current version");
            Console.WriteLine("3. Exit");
            Console.Write("\nSelect an option (1-3): ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    CheckForUpdates().Wait();
                    break;
                case "2":
                    ShowCurrentVersion().Wait();
                    break;
                case "3":
                    return;
                default:
                    ShowError("Invalid option. Please try again.");
                    break;
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }

    private static async Task CheckForUpdates()
    {
        try
        {
            ShowInfo("Starting auto-update check...");

            if (!File.Exists(versionFile))
            {
                ShowError("version.txt not found!");
                return;
            }

            string localVersion = await File.ReadAllTextAsync(versionFile);
            ShowInfo($"Local version: {localVersion}");

            // TODO: Replace with your actual URL
            string remoteVersionUrl = "https://example.com/version.txt";
            string updateUrl = "https://example.com/update.zip";

            string remoteVersion = await GetRemoteVersion(remoteVersionUrl);
            ShowInfo($"Remote version: {remoteVersion}");

            if (IsNewerVersion(remoteVersion, localVersion))
            {
                ShowSuccess("New version available! Starting update...");
                await DownloadAndInstallUpdate(updateUrl);
            }
            else
            {
                ShowInfo("No update needed.");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Failed to check for updates: {ex.Message}");
        }
    }

    private static async Task ShowCurrentVersion()
    {
        try
        {
            if (!File.Exists(versionFile))
            {
                ShowError("version.txt not found!");
                return;
            }

            string version = await File.ReadAllTextAsync(versionFile);
            ShowSuccess($"Current version: {version}");
        }
        catch (Exception ex)
        {
            ShowError($"Failed to read version: {ex.Message}");
        }
    }

    private static void ShowInfo(string message)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"[INFO] {message}");
        Console.ResetColor();
    }

    private static void ShowSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[SUCCESS] {message}");
        Console.ResetColor();
    }

    private static void ShowError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[ERROR] {message}");
        Console.ResetColor();
    }

    private static async Task<string> GetRemoteVersion(string url)
    {
        try
        {
            return await client.GetStringAsync(url);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get remote version: {ex.Message}");
        }
    }

    private static bool IsNewerVersion(string remote, string local)
    {
        // Simple version comparison (can be enhanced with proper semantic versioning)
        return string.Compare(remote, local, StringComparison.Ordinal) > 0;
    }

    private static async Task DownloadAndInstallUpdate(string updateUrl)
    {
        try
        {
            // Download update.zip
            ShowInfo("Downloading update...");
            byte[] updateBytes = await client.GetByteArrayAsync(updateUrl);
            await File.WriteAllBytesAsync(updateZip, updateBytes);

            // Read ignore list
            string[] ignoreList = File.Exists(ignoreFile) 
                ? await File.ReadAllLinesAsync(ignoreFile) 
                : Array.Empty<string>();

            // Extract update
            ShowInfo("Installing update...");
            using (ZipArchive archive = ZipFile.OpenRead(updateZip))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    string entryPath = Path.Combine(currentDirectory, entry.FullName);
                    
                    // Skip if file/directory is in ignore list
                    if (ignoreList.Any(ignore => entryPath.Contains(ignore)))
                    {
                        ShowInfo($"Skipping ignored file: {entry.FullName}");
                        continue;
                    }

                    // Create directory if needed
                    Directory.CreateDirectory(Path.GetDirectoryName(entryPath));

                    // Extract file
                    entry.ExtractToFile(entryPath, true);
                    ShowInfo($"Updated: {entry.FullName}");
                }
            }

            // Cleanup
            File.Delete(updateZip);
            ShowSuccess("Update completed successfully!");
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to install update: {ex.Message}");
        }
    }
} 