using System.Diagnostics;
using System.Threading.Tasks;
using PFXToolKitUI;
using PFXToolKitUI.Avalonia.Services.UserInputs;
using VWeaponEditor.Processes;

namespace VWeaponEditor.Avalonia.Processes;

public class ProcessSelectionServiceImpl : IProcessSelectionService {
    public async Task<Process?> SelectProcess() {
        ProcessSelectionUserInputInfo info = new ProcessSelectionUserInputInfo();
        
        // no await, do it eventually
        ApplicationPFX.Instance.Dispatcher.InvokeAsync(() => info.LoadProcessListAndBeginUpdating(), DispatchPriority.Background);
        if (await UserInputDialog.ShowDialogAsync(info) == true) {
            ProcessInfo? selected = info.SelectedProcess;
            info.ClearProcessListAndStopUpdating(selected);
            return selected?.Process;
        }

        return null;
    }
}