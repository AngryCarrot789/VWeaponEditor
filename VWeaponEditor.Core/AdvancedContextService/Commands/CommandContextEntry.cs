using System.Collections.Generic;
using System.Windows.Input;
using SharpPadV2.Core.AdvancedContextService.Base;

namespace SharpPadV2.Core.AdvancedContextService.Commands {
    /// <summary>
    /// The default implementation for a context entry (aka menu item), which also supports modifying the header,
    /// input gesture text, command and command parameter to reflect the UI menu item
    /// </summary>
    public class CommandContextEntry : ContextEntry, IContextEntry {
        private string header;
        private string inputGestureText;
        private string toolTip;
        private ICommand command;
        private object commandParameter;

        /// <summary>
        /// The menu item's header, aka text
        /// </summary>
        public string Header {
            get => this.header;
            set => this.RaisePropertyChanged(ref this.header, value);
        }

        /// <summary>
        /// The preview input gesture text, which is typically on the right side of a menu item (used for shortcuts)
        /// </summary>
        public string InputGestureText {
            get => this.inputGestureText;
            set => this.RaisePropertyChanged(ref this.inputGestureText, value);
        }

        /// <summary>
        /// A mouse over tooltip for this entry
        /// </summary>
        public string ToolTip {
            get => this.toolTip;
            set => this.RaisePropertyChanged(ref this.toolTip, value);
        }

        public ICommand Command {
            get => this.command;
            set => this.RaisePropertyChanged(ref this.command, value);
        }

        public object CommandParameter {
            get => this.commandParameter;
            set => this.RaisePropertyChanged(ref this.commandParameter, value);
        }

        public CommandContextEntry(IEnumerable<IContextEntry> children = null) : base(children) {

        }

        public CommandContextEntry(ICommand command, object commandParameter, IEnumerable<IContextEntry> children = null) : base(children) {
            this.command = command;
            this.commandParameter = commandParameter;
        }

        public CommandContextEntry(string header, ICommand command, IEnumerable<IContextEntry> children = null) : base(null, children) {
            this.Header = header;
            this.command = command;
        }

        public CommandContextEntry(string header, ICommand command, object commandParameter, IEnumerable<IContextEntry> children = null) : base(null, children) {
            this.Header = header;
            this.command = command;
            this.commandParameter = commandParameter;
        }

        public CommandContextEntry(string header, ICommand command, object commandParameter, string inputGestureText, IEnumerable<IContextEntry> children = null) : base(null, children) {
            this.Header = header;
            this.InputGestureText = inputGestureText;
            this.command = command;
            this.commandParameter = commandParameter;
        }

        public CommandContextEntry(string header, ICommand command, object commandParameter, string inputGestureText, string toolTip, IEnumerable<IContextEntry> children = null) : base(null, children) {
            this.Header = header;
            this.InputGestureText = inputGestureText;
            this.ToolTip = toolTip;
            this.command = command;
            this.commandParameter = commandParameter;
        }
    }
}