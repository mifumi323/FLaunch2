#nullable enable
using IWshRuntimeLibrary;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace FLaunch2.Services;

[SupportedOSPlatform("windows")]
public class ShortcutLink
{
    public ShortcutLink(string lnkFilePath)
    {
        // 参考：https://note.dokeep.jp/post/csharp-shortcut-file-load/
        WshShell? shell = null;
        IWshShortcut? lnk = null;
        try
        {
            shell = new WshShell();
            lnk = (IWshShortcut)shell.CreateShortcut(lnkFilePath);

            if (string.IsNullOrEmpty(lnk.TargetPath))
            {
                throw new InvalidOperationException("ショートカットを解析できませんでした。");
            }

            Arguments = lnk.Arguments;
            Description = lnk.Description;
            TargetPath = lnk.TargetPath;
            WorkingDirectory = lnk.WorkingDirectory;
        }
        finally
        {
            if (lnk != null) Marshal.ReleaseComObject(lnk);
            if (shell != null) Marshal.ReleaseComObject(shell);
        }
    }

    public string Arguments { get; internal set; }
    public string Description { get; private set; }
    public string TargetPath { get; private set; }
    public string WorkingDirectory { get; private set; }
}
