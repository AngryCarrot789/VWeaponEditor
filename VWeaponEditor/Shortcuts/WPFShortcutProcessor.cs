using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VWeaponEditor.Core;
using VWeaponEditor.Core.Actions.Contexts;
using VWeaponEditor.Core.Shortcuts.Inputs;
using VWeaponEditor.Core.Shortcuts.Managing;
using VWeaponEditor.Core.Shortcuts.Usage;
using VWeaponEditor.Core.Utils;

namespace VWeaponEditor.Shortcuts {
    public class WPFShortcutProcessor : ShortcutProcessor {
        public new WPFShortcutManager Manager => (WPFShortcutManager) base.Manager;

        public string CurrentInputBindingUsageID { get; set; } = WPFShortcutManager.DEFAULT_USAGE_ID;

        public WPFShortcutProcessor(ShortcutManager manager) : base(manager) {

        }

        public static WPFShortcutProcessor GetProcessorForControl(object sender) {
            return sender is Window window ? UIFocusGroup.GetShortcutProcessor(window) : null;
        }

        public static bool CanProcessEvent(DependencyObject obj, bool isPreviewEvent) {
            return UIFocusGroup.GetUsePreviewEvents(obj) == isPreviewEvent;
        }

        // Using async void here could possibly be dangerous if the awaited processor method (e.g. OnMouseStroke) halts
        // for a while due to a dialog for example. However... the methods should only really be callable when the window
        // is actually focused. But if the "root" event source is not a window then it could possibly be a problem
        // Could maybe implement a bool flag to state if it's current being processed or not?

        public async void OnWindowMouseDown(object sender, MouseButtonEventArgs e, bool isPreviewEvent) {
            if (e.OriginalSource is DependencyObject focused && CanProcessEvent(focused, isPreviewEvent)) {
                UIFocusGroup.ProcessFocusGroupChange(focused);

                try {
                    this.CurrentInputBindingUsageID = UIFocusGroup.GetUsageID(focused) ?? WPFShortcutManager.DEFAULT_USAGE_ID;
                    this.SetupDataContext(sender, focused);
                    MouseStroke stroke = new MouseStroke((int) e.ChangedButton, (int) Keyboard.Modifiers, e.ClickCount);
                    if (await this.OnMouseStroke(UIFocusGroup.FocusedGroupPath, stroke)) {
                        e.Handled = true;
                    }
                }
                finally {
                    this.CurrentDataContext = null;
                    this.CurrentInputBindingUsageID = WPFShortcutManager.DEFAULT_USAGE_ID;
                }
            }
        }

        public async void OnWindowMouseWheel(object sender, MouseWheelEventArgs e, bool isPreviewEvent) {
            if (e.OriginalSource is DependencyObject focused && CanProcessEvent(focused, isPreviewEvent)) {
                int button;
                if (e.Delta < 0) {
                    button = WPFShortcutManager.BUTTON_WHEEL_DOWN;
                }
                else if (e.Delta > 0) {
                    button = WPFShortcutManager.BUTTON_WHEEL_UP;
                }
                else {
                    return;
                }

                try {
                    this.CurrentInputBindingUsageID = UIFocusGroup.GetUsageID(focused) ?? WPFShortcutManager.DEFAULT_USAGE_ID;
                    this.SetupDataContext(sender, focused);
                    MouseStroke stroke = new MouseStroke(button, (int) Keyboard.Modifiers, 0, e.Delta);
                    if (await this.OnMouseStroke(UIFocusGroup.FocusedGroupPath, stroke)) {
                        e.Handled = true;
                    }
                }
                finally {
                    this.CurrentDataContext = null;
                    this.CurrentInputBindingUsageID = WPFShortcutManager.DEFAULT_USAGE_ID;
                }
            }
        }

        public async void OnKeyEvent(object sender, DependencyObject focused, KeyEventArgs e, bool isRelease, bool isPreviewEvent) {
            if (e.Handled) {
                return;
            }

            if (!isPreviewEvent) {
                System.Diagnostics.Debug.WriteLine($"{(isRelease ? "UP  " : "DOWN")}: {e.Key}");
            }

            Key key = e.Key == Key.System ? e.SystemKey : e.Key;
            if (ShortcutUtils.IsModifierKey(key) || key == Key.DeadCharProcessed) {
                return;
            }

            if (!CanProcessEvent(focused, isPreviewEvent)) {
                return;
            }

            WPFShortcutProcessor processor = GetProcessorForControl(sender);
            if (processor == null) {
                return;
            }

            try {
                this.CurrentInputBindingUsageID = UIFocusGroup.GetUsageID(focused) ?? WPFShortcutManager.DEFAULT_USAGE_ID;
                this.SetupDataContext(sender, focused);
                KeyStroke stroke = new KeyStroke((int) key, (int) Keyboard.Modifiers, isRelease);
                if (await processor.OnKeyStroke(UIFocusGroup.FocusedGroupPath, stroke)) {
                    e.Handled = true;
                }
            }
            finally {
                this.CurrentDataContext = null;
                this.CurrentInputBindingUsageID = WPFShortcutManager.DEFAULT_USAGE_ID;
            }
        }

        public void SetupDataContext(object sender, DependencyObject obj) {
            DataContext context = new DataContext();
            if (obj is IDataContext) {
                context.Merge((IDataContext) obj);
            }

            if (obj is FrameworkElement element) {
                object elementContext = element.DataContext;
                if (elementContext is IDataContext dc) {
                    context.Merge(dc);
                }
                else if (elementContext != null) {
                    context.AddContext(elementContext);
                }

                context.AddContext(element);
            }

            context.AddContext(sender);
            this.CurrentDataContext = context;
        }

        public override async Task<bool> OnShortcutActivated(GroupedShortcut shortcut) {
            bool finalResult = false;
            if (WPFShortcutManager.InputBindingCallbackMap.TryGetValue(shortcut.Path, out Dictionary<string, List<ActivationHandlerReference>> usageMap)) {
                if (shortcut.IsGlobal || shortcut.Group.IsGlobal) {
                    if (usageMap.TryGetValue(WPFShortcutManager.DEFAULT_USAGE_ID, out List<ActivationHandlerReference> list) && list.Count > 0) {
                        finalResult = await this.ActivateShortcut(shortcut, list);
                    }
                }

                if (!finalResult) {
                    if (usageMap.TryGetValue(this.CurrentInputBindingUsageID, out List<ActivationHandlerReference> list) && list.Count > 0) {
                        finalResult = await this.ActivateShortcut(shortcut, list);
                    }
                }
            }

            return finalResult || await base.OnShortcutActivated(shortcut);
        }

        private async Task<bool> ActivateShortcut(GroupedShortcut shortcut, List<ActivationHandlerReference> callbacks) {
            bool result = false;
            IoC.BroadcastShortcutActivity($"Activated global shortcut: {shortcut}. Calling {callbacks.Count} callbacks...");
            foreach (ActivationHandlerReference reference in callbacks) {
                ShortcutActivateHandler callback = reference.Value;
                if (callback != null && (result = await callback(this, shortcut))) {
                    break;
                }
            }

            IoC.BroadcastShortcutActivity($"Activated global shortcut: {shortcut}. Calling {callbacks.Count} callbacks... Complete!");
            return result;
        }

        public override bool OnNoSuchShortcutForKeyStroke(string @group, in KeyStroke stroke) {
            if (stroke.IsKeyDown) {
                IoC.BroadcastShortcutActivity("No such shortcut for key stroke: " + stroke + " in group: " + group);
            }

            return base.OnNoSuchShortcutForKeyStroke(@group, in stroke);
        }

        public override bool OnNoSuchShortcutForMouseStroke(string @group, in MouseStroke stroke) {
            IoC.BroadcastShortcutActivity("No such shortcut for mouse stroke: " + stroke + " in group: " + group);
            return base.OnNoSuchShortcutForMouseStroke(@group, in stroke);
        }

        public override bool OnCancelUsageForNoSuchNextKeyStroke(IShortcutUsage usage, GroupedShortcut shortcut, in KeyStroke stroke) {
            IoC.BroadcastShortcutActivity("No such shortcut for next key stroke: " + stroke);
            return base.OnCancelUsageForNoSuchNextKeyStroke(usage, shortcut, in stroke);
        }

        public override bool OnCancelUsageForNoSuchNextMouseStroke(IShortcutUsage usage, GroupedShortcut shortcut, in MouseStroke stroke) {
            IoC.BroadcastShortcutActivity("No such shortcut for next mouse stroke: " + stroke);
            return base.OnCancelUsageForNoSuchNextMouseStroke(usage, shortcut, in stroke);
        }

        public override bool OnShortcutUsagesCreated() {
            StringJoiner joiner = new StringJoiner(new StringBuilder(), ", ");
            foreach (KeyValuePair<IShortcutUsage, GroupedShortcut> pair in this.ActiveUsages) {
                joiner.Append(pair.Key.CurrentStroke.ToString());
            }

            IoC.BroadcastShortcutActivity("Waiting for next input: " + joiner);
            return base.OnShortcutUsagesCreated();
        }

        public override bool OnSecondShortcutUsagesProgressed() {
            StringJoiner joiner = new StringJoiner(new StringBuilder(), ", ");
            foreach (KeyValuePair<IShortcutUsage, GroupedShortcut> pair in this.ActiveUsages) {
                joiner.Append(pair.Key.CurrentStroke.ToString());
            }

            IoC.BroadcastShortcutActivity("Waiting for next input: " + joiner);
            return base.OnSecondShortcutUsagesProgressed();
        }
    }
}