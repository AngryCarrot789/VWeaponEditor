using System.Diagnostics;
using PFXToolKitUI;
using PFXToolKitUI.Services.UserInputs;
using PFXToolKitUI.Utils.Collections.Observable;

namespace VWeaponEditor.Processes;

public delegate void ProcessSelectionUserInputInfoSelectedProcessIndexChangedEventHandler(ProcessSelectionUserInputInfo sender, int oldSelectedProcessIndex, int newSelectedProcessIndex);

public delegate void ProcessSelectionUserInputInfoIsRefreshingProcessListChangedEventHandler(ProcessSelectionUserInputInfo sender);

public class ProcessSelectionUserInputInfo : UserInputInfo {
    private readonly ObservableList<ProcessInfo> processes;
    private int selectedProcessIndex;
    private bool isRefreshingProcessList;
    private bool isUpdateDataTaskRunning;
    private Task updateDataTask;
    
    public int SelectedProcessIndex {
        get => this.selectedProcessIndex;
        set {
            int oldSelectedProcessIndex = this.selectedProcessIndex;
            if (oldSelectedProcessIndex == value)
                return;

            this.selectedProcessIndex = value;
            this.SelectedProcessIndexChanged?.Invoke(this, oldSelectedProcessIndex, value);
        }
    }

    public bool IsRefreshingProcessList {
        get => this.isRefreshingProcessList;
        private set {
            if (this.isRefreshingProcessList == value)
                return;

            this.isRefreshingProcessList = value;
            this.IsRefreshingProcessListChanged?.Invoke(this);
        }
    }
    
    public ReadOnlyObservableList<ProcessInfo> Processes { get; }

    public ProcessInfo? SelectedProcess => this.SelectedProcessIndex == -1 ? null : this.processes[this.SelectedProcessIndex];
    
    public event ProcessSelectionUserInputInfoSelectedProcessIndexChangedEventHandler? SelectedProcessIndexChanged;
    public event ProcessSelectionUserInputInfoIsRefreshingProcessListChangedEventHandler? IsRefreshingProcessListChanged;

    public ProcessSelectionUserInputInfo() : this("Select a process", null) {
    }

    public ProcessSelectionUserInputInfo(string? caption, string? message) : base(caption, message) {
        this.processes = new ObservableList<ProcessInfo>();
        this.Processes = new ReadOnlyObservableList<ProcessInfo>(this.processes);
    }

    public override bool HasErrors() {
        return this.SelectedProcessIndex == -1;
    }

    public override void UpdateAllErrors() {
    }

    public void LoadProcessListAndBeginUpdating() {
        if (this.isUpdateDataTaskRunning) {
            return;
        }

        this.isUpdateDataTaskRunning = true;
        this.updateDataTask = Task.Run(async () => {
            if (this.isUpdateDataTaskRunning) {
                await this.RefreshAction();
            }

            while (this.isUpdateDataTaskRunning) {
                if (!this.IsRefreshingProcessList) {
                    await await ApplicationPFX.Instance.Dispatcher.InvokeAsync(() => this.UpdateProcesses(true));
                }

                await Task.Delay(10000);
            }
        });
    }

    public void ClearProcessListAndStopUpdating(ProcessInfo? except) {
        this.isUpdateDataTaskRunning = false;
        this.SelectedProcessIndex = -1;
        this.DisposeAllExcept(except);
        this.processes.Clear();
    }
    
    public async Task RefreshAction() {
        IDispatcher d = ApplicationPFX.Instance.Dispatcher;

        await d.InvokeAsync(() => {
            this.IsRefreshingProcessList = true;
            this.DisposeAllExcept(null);
            this.SelectedProcessIndex = -1;
            this.processes.Clear();
        }, DispatchPriority.Background);

        // Task.Run allows us to load the processes on another task and add them on the main thread
        // because there can be 100s of processes, and querying properties on them can be quite expensive (20+ ms per process)
        await Task.Run(async () => {
            foreach (Process process in Process.GetProcesses()) {
                ProcessInfo p = new ProcessInfo(process);
                try {
                    await p.Refresh(false, false);
                }
                catch { /* ignored because screw it lol */ }

                if (p.IsAlive && !string.IsNullOrWhiteSpace(p.MainWindowTitle)) {
                    // Background priority, so that which function won't ruin the UI performance
                    await d.InvokeAsync(() => this.processes.Add(p), DispatchPriority.Background);
                }
            }
        });

        await d.InvokeAsync(() => this.IsRefreshingProcessList = false, DispatchPriority.Background);
    }
    
    private void DisposeAllExcept(ProcessInfo? process) {
        foreach (ProcessInfo p in this.Processes) {
            if (p != process) {
                try {
                    p.Dispose();
                }
                catch { /* ignored */ }
            }
        }
    }
    
    public async Task UpdateProcesses(bool refresh) {
        foreach (ProcessInfo process in this.Processes) {
            try {
                await process.Refresh(refresh);
            }
            catch { /* ignored */ }
        }
    }
}