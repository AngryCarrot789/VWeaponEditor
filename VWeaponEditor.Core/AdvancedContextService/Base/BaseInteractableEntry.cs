using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SharpPadV2.Core.Actions.Contexts;

namespace SharpPadV2.Core.AdvancedContextService.Base {
    public class BaseInteractableEntry : BaseViewModel, IContextEntry {
        private readonly DataContext context;

        public IDataContext Context => this.context;

        protected BaseInteractableEntry(object dataContext) {
            this.context = new DataContext();
            if (dataContext != null) {
                this.context.AddContext(dataContext);
            }
        }

        protected void SetContextKey(string key, object value) {
            this.context.Set(key, value);
        }
    }
}