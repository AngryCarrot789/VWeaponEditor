using System.Threading.Tasks;
using VWeaponEditor.Core.Views.Windows;

namespace VWeaponEditor.Views {
    public class BaseWindow : BaseWindowCore, IWindow {
        public void CloseWindow() {
            this.Close();
        }

        public async Task CloseWindowAsync() {
            await this.Dispatcher.InvokeAsync(this.CloseWindow);
        }
    }
}