using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using VWeaponEditor.Core;
using VWeaponEditor.Core.Timing;
using VWeaponEditor.Core.Utils;
using VWeaponEditor.Core.Views.Dialogs;
using VWeaponEditor.Core.Views.ViewModels;
using VWeaponEditor.Highlighting;
using VWeaponEditor.Utils;

namespace VWeaponEditor.Processes {
    public class ProcessSelectorViewModel : BaseConfirmableDialogViewModel, IErrorInfoHandler {
        public ObservableCollection<ProcessViewModel> Processes { get; }

        private ProcessViewModel selectedProcess;
        public ProcessViewModel SelectedProcess {
            get => this.selectedProcess;
            set {
                this.RaisePropertyChanged(ref this.selectedProcess, value);
                this.ConfirmCommand.IsEnabled = value != null;
            }
        }

        private volatile bool isRefreshing;
        public bool IsRefreshing {
            get => this.isRefreshing;
            set {
                this.isRefreshing = value;
                this.RaisePropertyChanged();
            }
        }

        private string searchTerm;
        public string SearchTerm {
            get => this.searchTerm;
            set {
                this.RaisePropertyChanged(ref this.searchTerm, value);
                this.OnInputChanged();
            }
        }

        public ICommand RefreshCommand { get; }

        private Task updateDataTask;
        private volatile bool isUpdateDataTaskRunning;

        public IdleEventService IdleEventService { get; }

        public ProcessSelectorViewModel(IDialog dialog) : base(dialog) {
            this.Processes = new ObservableCollection<ProcessViewModel>();
            this.RefreshCommand = new AsyncRelayCommand(this.RefreshAction);
            this.IdleEventService = new IdleEventService();
            this.IdleEventService.MinimumTimeSinceInput = TimeSpan.FromMilliseconds(200);
            this.IdleEventService.OnIdle += this.OnTickSearch;
        }

        private void OnInputChanged() {
            if (string.IsNullOrEmpty(this.SearchTerm)) {
                this.IdleEventService.CanFireNextTick = false;
                this.OnTickSearch(); // clears highlighting
                // this.IsSearchTermEmpty = true;
                // this.IsSearchActive = false;
            }
            else {
                // this.IsSearchTermEmpty = false;
                this.IdleEventService.OnInput();
            }
        }

        private void OnTickSearch() {
            if (string.IsNullOrWhiteSpace(this.SearchTerm)) {
                this.Processes.ForEach(x => x.ProcessNameHighlight = null);
                return;
            }

            foreach (ProcessViewModel process in this.Processes) {
                string text = process.ProcessName;
                if (string.IsNullOrEmpty(text))
                    continue;

                List<TextRange> ranges = new List<TextRange>();
                if (!FindMatches(this.SearchTerm, text, RegexOptions.IgnoreCase, ranges, out bool regexFail)) {
                    if (regexFail)
                        return;
                    continue;
                }

                process.ProcessNameHighlight = ranges;
            }
        }

        private static bool FindMatches(string pattern, string value, RegexOptions options, List<TextRange> matches, out bool patternFail) {
            MatchCollection collection;
            try {
                collection = Regex.Matches(value, pattern, options);
            }
            catch (ArgumentException) {
                patternFail = true;
                return false;
            }

            if (collection.Count < 1) {
                patternFail = false;
                return false;
            }

            foreach (Match match in collection) {
                matches.Add(new TextRange(match.Index, match.Length));
            }

            patternFail = false;
            return true;
        }

        public void StartRefreshTask() {
            this.isUpdateDataTaskRunning = true;
            this.updateDataTask = Task.Run(async () => {
                while (true) {
                    if (this.isUpdateDataTaskRunning) {
                        if (!this.isRefreshing) {
                            await IoC.Dispatcher.InvokeAsync(() => this.UpdateProcesses(true));
                        }

                        await Task.Delay(10000);
                    }
                    else {
                        break;
                    }
                }
            });
        }

        public void StopRefreshTask() {
            this.isUpdateDataTaskRunning = false;
        }

        public async Task RefreshAction() {
            this.IsRefreshing = true;
            this.DisposeAllExcept(null);
            this.Processes.Clear();
            this.SelectedProcess = null;

            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

            // Task.Run allows us to load the processes on another task and add them on the main thread
            // because there can be 100s of processes, and querying properties on them can be quite expensive (20+ ms per process)
            await Task.Run(async () => {
                foreach (Process process in Process.GetProcesses()) {
                    ProcessViewModel p = new ProcessViewModel(process);
                    try {
                        await p.Refresh(false, false);
                    }
                    catch { /* ignored because screw it lol */ }

                    if (!p.IsAlive || string.IsNullOrWhiteSpace(p.MainWindowTitle))
                        continue;

                    // Background priority, so that which function won't ruin the UI performance
                    await dispatcher.InvokeAsync(() => this.Processes.Add(p), DispatcherPriority.Background);
                }
            });

            this.IsRefreshing = false;
        }

        public void DisposeAllExcept(ProcessViewModel process) {
            foreach (ProcessViewModel p in this.Processes) {
                if (p == process)
                    continue;

                try {
                    p.Dispose();
                }
                catch { /* ignored */ }
            }
        }

        public async Task UpdateProcesses(bool refresh, bool raisePropertiesChanged = true) {
            foreach (ProcessViewModel process in this.Processes) {
                try {
                    await process.Refresh(refresh, raisePropertiesChanged);
                }
                catch { /* ignored */ }
            }
        }

        public void OnErrorsUpdated(Dictionary<string, object> errors) {
            this.ConfirmCommand.IsEnabled = errors.Count < 1;
        }

        public override async Task<bool> CanConfirm() {
            return this.SelectedProcess != null && await base.CanConfirm();
        }
    }
}