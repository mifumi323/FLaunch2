using FLaunch2.Models;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FLaunch2.Services;

[SupportedOSPlatform("windows")]
public static class StartMenuReader
{
    // {GUID}\relative\path 形式のAppIDにマッチ
    private static readonly Regex KnownFolderPattern = new(@"^\{([0-9A-Fa-f\-]+)\}\\(.+)$");

    public static IEnumerable<Item> ReadItems()
    {
        var json = RunGetStartApps();
        if (string.IsNullOrWhiteSpace(json))
        {
            yield break;
        }

        var entries = JsonSerializer.Deserialize<StartAppEntry[]>(json);
        if (entries == null)
        {
            yield break;
        }

        foreach (var entry in entries)
        {
            var item = TryConvert(entry);
            if (item != null)
            {
                yield return item;
            }
        }
    }

    private static string RunGetStartApps()
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = "-NoProfile -Command \"Get-StartApps | ConvertTo-Json -Compress\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
            };
            using var process = Process.Start(psi);
            if (process == null)
            {
                return string.Empty;
            }
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output;
        }
        catch
        {
            return string.Empty;
        }
    }

    private static Item? TryConvert(StartAppEntry entry)
    {
        if (string.IsNullOrWhiteSpace(entry.Name) || string.IsNullOrWhiteSpace(entry.AppID))
        {
            return null;
        }

        var filePath = ResolveFilePath(entry.AppID);
        if (filePath == null)
        {
            // まあストアアプリじゃねえかな
            return new Item
            {
                DisplayName = entry.Name,
                FilePath = entry.AppID,
                ItemType = ItemType.StoreApp,
            };
        }

        return new Item
        {
            DisplayName = entry.Name,
            FilePath = filePath,
        };
    }

    private static string? ResolveFilePath(string appId)
    {
        // 絶対パス
        if (Path.IsPathRooted(appId))
        {
            return appId;
        }

        // {GUID}\相対パス 形式
        var match = KnownFolderPattern.Match(appId);
        if (match.Success && Guid.TryParse(match.Groups[1].Value, out var folderId))
        {
            var folderPath = GetKnownFolderPath(folderId);
            if (folderPath != null)
            {
                return Path.Combine(folderPath, match.Groups[2].Value);
            }
        }

        // ストアアプリなど解決不可
        return null;
    }

    private static string? GetKnownFolderPath(Guid folderId)
    {
        try
        {
            SHGetKnownFolderPath(folderId, 0, nint.Zero, out var path);
            return path;
        }
        catch
        {
            return null;
        }
    }

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    private static extern int SHGetKnownFolderPath(
        [MarshalAs(UnmanagedType.LPStruct)] Guid rfid,
        uint dwFlags,
        nint hToken,
        out string pszPath);

    private sealed class StartAppEntry
    {
        public string Name { get; set; } = string.Empty;
        public string AppID { get; set; } = string.Empty;
    }
}
