using System.Threading.Tasks;

namespace VWeaponEditor.Core.Views.Windows {
    public interface IWindow : IViewBase {
        void CloseWindow();

        Task CloseWindowAsync();
    }
}