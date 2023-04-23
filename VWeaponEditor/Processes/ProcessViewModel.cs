using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using VWeaponEditor.Core;
using VWeaponEditor.Highlighting;

namespace VWeaponEditor.Processes {
    public class ProcessViewModel : BaseViewModel, IDisposable {
        public Process Process { get; }

        private bool isAlive;
        public bool IsAlive {
            get => this.isAlive;
            private set => this.RaisePropertyChanged(ref this.isAlive, value);
        }

        private string processName;
        private int processId;
        private string mainWindowTitle;
        private bool isResponding;
        private int sessionId;

        public string ProcessName {
            get => this.processName;
            private set => this.RaisePropertyChanged(ref this.processName, value);
        }

        public int ProcessId {
            get => this.processId;
            private set => this.RaisePropertyChanged(ref this.processId, value);
        }

        public string MainWindowTitle {
            get => this.mainWindowTitle;
            private set => this.RaisePropertyChanged(ref this.mainWindowTitle, value);
        }

        public bool IsResponding {
            get => this.isResponding;
            private set => this.RaisePropertyChanged(ref this.isResponding, value);
        }

        public int SessionId {
            get => this.sessionId;
            private set => this.RaisePropertyChanged(ref this.sessionId, value);
        }

        private IEnumerable<TextRange> processNameHighlight;
        public IEnumerable<TextRange> ProcessNameHighlight {
            get => this.processNameHighlight;
            set => this.RaisePropertyChanged(ref this.processNameHighlight, value);
        }

        public ProcessViewModel(Process process) {
            this.Process = process;
            this.isAlive = true;
        }

        public async Task Refresh(bool refreshProcess, bool raisePropertiesChanged = true) {
            if (refreshProcess) {
                this.Process.Refresh();
            }

            this.isAlive = true;
            this.isAlive =         this.TryGet(x => !x.HasExited, false);
            this.processName =     this.TryGet(x => x.ProcessName, default);
            this.processId =       this.TryGet(x => x.Id, default);
            this.mainWindowTitle = this.TryGet(x => x.MainWindowTitle, default);
            this.isResponding =    this.TryGet(x => x.Responding, false);
            this.sessionId =       this.TryGet(x => x.SessionId, default);

            if (!raisePropertiesChanged)
                return;
            await IoC.Dispatcher.InvokeAsync(() => {
                this.RaisePropertyChanged(nameof(this.ProcessName));
                this.RaisePropertyChanged(nameof(this.ProcessId));
                this.RaisePropertyChanged(nameof(this.MainWindowTitle));
                this.RaisePropertyChanged(nameof(this.IsResponding));
                this.RaisePropertyChanged(nameof(this.SessionId));
            });
        }

        public void Dispose() {
            this.Process.Dispose();
        }

        public T TryGet<T>(Func<Process, T> getter, T def) {
            if (!this.isAlive)
                return def;

            try {
                return getter(this.Process);
            }
            catch {
                return def;
            }
        }
    }
}