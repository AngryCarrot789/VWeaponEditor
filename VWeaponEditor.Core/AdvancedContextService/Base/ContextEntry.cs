using System.Collections.Generic;

namespace SharpPadV2.Core.AdvancedContextService.Base {
    public class ContextEntry : BaseInteractableEntry {
        public IEnumerable<IContextEntry> Children { get; }

        public ContextEntry(IEnumerable<IContextEntry> children = null) : base(null) {
            this.Children = children;
        }

        public ContextEntry(object dataContext, IEnumerable<IContextEntry> children = null) : base(dataContext) {
            this.Children = children;
        }
    }
}