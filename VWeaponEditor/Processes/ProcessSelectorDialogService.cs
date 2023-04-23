using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using VWeaponEditor.Core;
using VWeaponEditor.Utils;

namespace VWeaponEditor.Processes {
    public class ProcessSelectorDialogService {
        public static ProcessSelectorDialogService Instance { get; } = new ProcessSelectorDialogService();

        public ProcessViewModel SelectProcess() {
            ProcessSelectorWindow window = new ProcessSelectorWindow();
            ProcessSelectorViewModel vm = new ProcessSelectorViewModel(window);
            window.DataContext = vm;

            DispatcherUtils.InvokeLater(vm.RefreshAction);
            // vm.StartRefreshTask();
            if (window.ShowDialog() == true) {
                ProcessViewModel selected = vm.SelectedProcess;
                if (selected != null) {
                    // vm.StopRefreshTask();
                    vm.DisposeAllExcept(selected);
                    vm.Processes.Clear();
                    return selected;
                }
            }

            return null;
        }
    }
}