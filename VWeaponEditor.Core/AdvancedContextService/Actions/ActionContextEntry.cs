using System.Collections.Generic;
using VWeaponEditor.Core.Actions;
using VWeaponEditor.Core.AdvancedContextService.Base;

namespace VWeaponEditor.Core.AdvancedContextService.Actions {
    /// <summary>
    /// The default implementation for a context entry (aka menu item), which also supports modifying the header,
    /// input gesture text, command and command parameter to reflect the UI menu item
    /// <para>
    /// Setting <see cref="ContextEntry.InputGestureText"/> will not do anything, as the UI will automatically search for the action ID shortcut
    /// </para>
    /// </summary>
    public class ActionContextEntry : ContextEntry, IContextEntry {
        private string actionId;
        public string ActionId {
            get => this.actionId;
            set => this.RaisePropertyChanged(ref this.actionId, value);
        }

        private ActionManager manager = ActionManager.Instance;
        public ActionManager Manager {
            get => this.manager;
            set => this.RaisePropertyChanged(ref this.manager, value ?? ActionManager.Instance);
        }

        public ActionContextEntry(IEnumerable<IContextEntry> children = null) : base(children) {

        }

        public ActionContextEntry(string actionId, IEnumerable<IContextEntry> children = null) : base(children) {
            this.actionId = actionId;
        }

        public ActionContextEntry(object dataContext, string actionId, IEnumerable<IContextEntry> children = null) : base(dataContext, children) {
            this.actionId = actionId;
        }
    }
}