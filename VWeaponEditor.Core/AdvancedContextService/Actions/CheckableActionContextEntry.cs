using System.Collections.Generic;
using SharpPadV2.Core.Actions;
using SharpPadV2.Core.AdvancedContextService.Base;
using SharpPadV2.Core.Utils;

namespace SharpPadV2.Core.AdvancedContextService.Actions {
    public class CheckableActionContextEntry : ActionContextEntry {
        private bool isChecked;
        public bool IsChecked {
            get => this.isChecked;
            set {
                this.RaisePropertyChanged(ref this.isChecked, value);
                this.SetContextKey(ToggleAction.IsToggledKey, value.Box());
            }
        }

        public CheckableActionContextEntry(IEnumerable<IContextEntry> children = null) : base(children) {

        }

        public CheckableActionContextEntry(string actionId, IEnumerable<IContextEntry> children = null) : base(actionId, children) {

        }

        public CheckableActionContextEntry(object dataContext, string actionId, IEnumerable<IContextEntry> children = null) : base(dataContext, actionId, children) {

        }
    }
}