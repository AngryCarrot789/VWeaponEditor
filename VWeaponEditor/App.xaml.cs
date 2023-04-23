using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VWeaponEditor.Core.Actions;
using VWeaponEditor.Core.Shortcuts.Managing;
using VWeaponEditor.Core.Shortcuts.ViewModels;
using VWeaponEditor.Core;
using VWeaponEditor.Services;
using VWeaponEditor.Shortcuts.Dialogs;
using VWeaponEditor.Shortcuts.Views;
using VWeaponEditor.Shortcuts;
using VWeaponEditor.Views.Dialogs.FilePicking;
using VWeaponEditor.Views.Dialogs.Message;
using VWeaponEditor.Views.Dialogs.UserInputs;
using VWeaponEditor.Shortcuts.Converters;
using System.Windows.Threading;
using VWeaponEditor.Core.Services;
using VWeaponEditor.Processes;

namespace VWeaponEditor {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        private void Application_Startup(object sender, StartupEventArgs e) {
            ActionManager.SearchAndRegisterActions(IoC.ActionManager);
            IoC.MessageDialogs = new MessageDialogService();
            IoC.Dispatcher = new DispatcherDelegate(this);
            IoC.Clipboard = new ClipboardService();
            IoC.FilePicker = new FilePickDialogService();
            IoC.UserInput = new UserInputDialogService();
            IoC.ExplorerService = new WinExplorerService();
            InputStrokeViewModel.KeyToReadableString = KeyStrokeStringConverter.ToStringFunction;
            InputStrokeViewModel.MouseToReadableString = MouseStrokeStringConverter.ToStringFunction;
            IoC.KeyboardDialogs = new KeyboardDialogService();
            IoC.MouseDialogs = new MouseDialogService();
            IoC.ShortcutManager = WPFShortcutManager.Instance;
            IoC.ShortcutManagerDialog = new ShortcutManagerDialogService();
            IoC.OnShortcutModified = (x) => {
                if (!string.IsNullOrWhiteSpace(x)) {
                    WPFShortcutManager.Instance.InvalidateShortcutCache();
                    GlobalUpdateShortcutGestureConverter.BroadcastChange();
                    // UpdatePath(this.Resources, x);
                }
            };

            string filePath = @"F:\VSProjsV2\SharpPadV2\SharpPadV2\Keymap.xml";
            if (File.Exists(filePath)) {
                using (FileStream stream = File.OpenRead(filePath)) {
                    ShortcutGroup group = WPFKeyMapDeserialiser.Instance.Deserialise(stream);
                    WPFShortcutManager.Instance.SetRoot(group);
                }
            }
            else {
                MessageBox.Show("Keymap file does not exist: " + filePath);
            }

            MainWindow window = new MainWindow();
            this.MainWindow = window;
            window.Show();
        }

        private class DispatcherDelegate : IDispatcher {
            private readonly App app;

            public DispatcherDelegate(App app) {
                this.app = app;
            }

            public void InvokeLater(Action action) {
                this.app.Dispatcher.Invoke(action, DispatcherPriority.Normal);
            }

            public void Invoke(Action action) {
                this.app.Dispatcher.Invoke(action);
            }

            public T Invoke<T>(Func<T> function) {
                return this.app.Dispatcher.Invoke(function);
            }

            public async Task InvokeAsync(Action action) {
                await this.app.Dispatcher.InvokeAsync(action);
            }

            public async Task<T> InvokeAsync<T>(Func<T> function) {
                return await this.app.Dispatcher.InvokeAsync(function);
            }
        }
    }
}
