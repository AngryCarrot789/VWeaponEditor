using System.Threading.Tasks;
using System.Windows.Input;
using VWeaponEditor.Core;
using VWeaponEditor.Processes;

namespace VWeaponEditor {
    public class MainViewModel : BaseViewModel {
        private ProcessViewModel process;
        public ProcessViewModel Process {
            get => this.process;
            set => this.RaisePropertyChanged(ref this.process, value);
        }

        public ICommand OpenProcessCommand { get; }

        public MainViewModel() {
            this.OpenProcessCommand = new AsyncRelayCommand(this.OpenProcessAction);
        }

        private async Task OpenProcessAction() {
            ProcessViewModel process = ProcessSelectorDialogService.Instance.SelectProcess();
            if (process != null) {
                this.Process = process;
            }
        }
    }
}