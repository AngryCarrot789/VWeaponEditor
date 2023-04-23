using VWeaponEditor.Core.Shortcuts.Inputs;

namespace VWeaponEditor.Core.Shortcuts.Dialogs {
    public interface IKeyboardDialogService {
        KeyStroke? ShowGetKeyStrokeDialog();
    }
}