﻿using Avalonia;
using System;
using System.IO;

namespace VWeaponEditor.Avalonia;

class Program {
    [STAThread]
    public static void Main(string[] args) {
        try {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e) {
            string? filePath = args.Length > 0 ? args[0] : null;
            if (string.IsNullOrEmpty(filePath)) {
                string[] trueArgs = Environment.GetCommandLineArgs();
                if (trueArgs.Length > 0)
                    filePath = trueArgs[0];
            }

            string? dirPath = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dirPath) && Directory.Exists(dirPath)) {
                try {
                    File.WriteAllText(Path.Combine(dirPath, "VWeaponEditor_LastCrashError.txt"), e.ToString());
                }
                catch { /* ignored */ }
            }
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>().
                      UsePlatformDetect().
                      WithInterFont().
                      With(new Win32PlatformOptions() { CompositionMode = [Win32CompositionMode.LowLatencyDxgiSwapChain], RenderingMode = [Win32RenderingMode.AngleEgl] }).
                      LogToTrace();
}