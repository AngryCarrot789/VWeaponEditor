using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using VWeaponEditor.Core.Actions;
using VWeaponEditor.Core.Shortcuts.Inputs;
using VWeaponEditor.Core.Shortcuts.Managing;
using VWeaponEditor.Core.Utils;
using VWeaponEditor.Shortcuts.Views;

namespace VWeaponEditor.Shortcuts.Converters {
    public class ActionIdToGestureConverter : IValueConverter {
        public static ActionIdToGestureConverter Instance { get; } = new ActionIdToGestureConverter();

        public string NoSuchActionText { get; set; } = null;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is string id) {
                return ActionIdToGesture(id, this.NoSuchActionText, out string gesture) ? gesture : DependencyProperty.UnsetValue;
            }

            throw new Exception("Value is not a string");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }

        public static bool ActionIdToGesture(string id, string fallback, out string gesture) {
            if (ActionManager.Instance.GetAction(id) == null) {
                return (gesture = fallback) != null;
            }

            IEnumerable<GroupedShortcut> shortcuts = WPFShortcutManager.Instance.GetShortcutsByAction(id);
            if (shortcuts == null) {
                return (gesture = fallback) != null;
            }

            return (gesture = shortcuts.Select(ToString).JoinString(", ", " or ", fallback)) != null;
        }

        private static string ToString(GroupedShortcut shortcut) {
            return string.Join(", ", shortcut.Shortcut.InputStrokes.Select(ToString));
        }

        private static string ToString(IInputStroke stroke) {
            if (stroke is MouseStroke ms) {
                return MouseStrokeStringConverter.ToStringFunction(ms.MouseButton, ms.Modifiers, ms.ClickCount);
            }
            else if (stroke is KeyStroke ks) {
                return KeyStrokeStringConverter.ToStringFunction(ks.KeyCode, ks.Modifiers, ks.IsKeyRelease, true, true);
            }
            else {
                return stroke.ToString();
            }
        }
    }
}