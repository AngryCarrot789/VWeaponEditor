using System;
using VWeaponEditor.Core.Actions;
using VWeaponEditor.Core.Services;
using VWeaponEditor.Core.Shortcuts.Dialogs;
using VWeaponEditor.Core.Shortcuts.Managing;
using VWeaponEditor.Core.Views.Dialogs.FilePicking;
using VWeaponEditor.Core.Views.Dialogs.Message;
using VWeaponEditor.Core.Views.Dialogs.UserInputs;

namespace VWeaponEditor.Core {
    public static class IoC {
        public static SimpleIoC Instance { get; } = new SimpleIoC();

        public static ActionManager ActionManager { get; } = new ActionManager();
        public static Action<string> OnShortcutModified { get; set; }
        public static Action<string> BroadcastShortcutActivity { get; set; }

        public static IDispatcher Dispatcher { get; set; }
        public static IClipboardService Clipboard { get; set; }
        public static IMessageDialogService MessageDialogs { get; set; }
        public static IFilePickDialogService FilePicker { get; set; }
        public static IUserInputDialogService UserInput { get; set; }
        public static IExplorerService ExplorerService { get; set; }

        public static IKeyboardDialogService KeyboardDialogs { get; set; }

        public static IMouseDialogService MouseDialogs { get; set; }

        public static ShortcutManager ShortcutManager { get; set; }
        public static IShortcutManagerDialogService ShortcutManagerDialog { get; set; }
    }
}