using System.Threading.Tasks;
using VWeaponEditor.Core.Shortcuts.Managing;

namespace VWeaponEditor.Shortcuts {
    public delegate Task<bool> ShortcutActivateHandler(ShortcutProcessor processor, GroupedShortcut shortcut);
}