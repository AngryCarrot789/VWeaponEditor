using System.Diagnostics;
using PFXToolKitUI;

namespace VWeaponEditor.Processes;

public delegate void ProcessInfoEventHandler(ProcessInfo sender);

public sealed class ProcessInfo {
    private bool isAlive;
    private string? processName;
    private int processId;
    private string? mainWindowTitle;
    private bool isResponding;
    private int sessionId;

    public bool IsAlive {
        get => this.isAlive;
        private set {
            if (this.isAlive == value)
                return;
            this.isAlive = value;
            this.IsAliveChanged?.Invoke(this);
        }
    }

    public string? ProcessName {
        get => this.processName;
        private set {
            if (this.processName == value)
                return;
            this.processName = value;
            this.ProcessNameChanged?.Invoke(this);
        }
    }
    
    public int ProcessId {
        get => this.processId;
        private set {
            if (this.processId == value)
                return;
            this.processId = value;
            this.ProcessIdChanged?.Invoke(this);
        }
    }
    
    public string? MainWindowTitle {
        get => this.mainWindowTitle;
        private set {
            if (this.mainWindowTitle == value)
                return;
            this.mainWindowTitle = value;
            this.MainWindowTitleChanged?.Invoke(this);
        }
    }
    
    public bool IsResponding {
        get => this.isResponding;
        private set {
            if (this.isResponding == value)
                return;

            this.isResponding = value;
            this.IsRespondingChanged?.Invoke(this);
        }
    }
    
    public int SessionId {
        get => this.sessionId;
        private set {
            if (this.sessionId == value)
                return;
            this.sessionId = value;
            this.SessionIdChanged?.Invoke(this);
        }
    }
    
    public Process Process { get; }

    public event ProcessInfoEventHandler? IsAliveChanged;
    public event ProcessInfoEventHandler? ProcessNameChanged;
    public event ProcessInfoEventHandler? ProcessIdChanged;
    public event ProcessInfoEventHandler? MainWindowTitleChanged;
    public event ProcessInfoEventHandler? IsRespondingChanged;
    public event ProcessInfoEventHandler? SessionIdChanged;

    public ProcessInfo(Process process) {
        this.Process = process;
        this.isAlive = true;
    }
    
    public async Task Refresh(bool refreshProcess, bool raiseEvents = true) {
        if (refreshProcess) {
            this.Process.Refresh();
        }

        this.isAlive = true;
        
        bool isAlive = this.TryGet(x => !x.HasExited, false);
        string? processName = this.TryGet(x => x.ProcessName, null);
        int processId = this.TryGet(x => x.Id, 0);
        string? mainWindowTitle = this.TryGet(x => x.MainWindowTitle, null);
        bool isResponding = this.TryGet(x => x.Responding, false);
        int sessionId = this.TryGet(x => x.SessionId, 0);

        if (raiseEvents) {
            await ApplicationPFX.Instance.Dispatcher.InvokeAsync(() => {
                this.IsAlive = isAlive;
                this.ProcessName = processName;
                this.ProcessId = processId;
                this.MainWindowTitle = mainWindowTitle;
                this.IsResponding = isResponding;
                this.SessionId = sessionId;
            });
        }
        else {
            this.isAlive = isAlive;
            this.processName = processName;
            this.processId = processId;
            this.mainWindowTitle = mainWindowTitle;
            this.isResponding = isResponding;
            this.sessionId = sessionId;
        }
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