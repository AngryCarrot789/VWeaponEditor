using System;
using System.IO;
using Avalonia;
using Avalonia.Markup.Xaml;
using PFXToolKitUI;
using PFXToolKitUI.Avalonia;

namespace VWeaponEditor.Avalonia;

public partial class App : Application {
    public override void Initialize() {
        AvaloniaXamlLoader.Load(this);
        AvUtils.OnApplicationInitialised();

        ApplicationPFX.InitializeInstance(new VVWeaponEditorApplication(this));
    }

    public override async void OnFrameworkInitializationCompleted() {
        base.OnFrameworkInitializationCompleted();
        AvUtils.OnFrameworkInitialised();

        EmptyApplicationStartupProgress progress = new EmptyApplicationStartupProgress();
        string[] envArgs = Environment.GetCommandLineArgs();
        if (envArgs.Length > 0 && Path.GetDirectoryName(envArgs[0]) is string dir && dir.Length > 0) {
            Directory.SetCurrentDirectory(dir);
        }
        
        await ApplicationPFX.InitializeApplication(progress, envArgs);
    }
}