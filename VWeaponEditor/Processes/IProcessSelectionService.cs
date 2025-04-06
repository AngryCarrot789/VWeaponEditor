using System.Diagnostics;
using PFXToolKitUI;

namespace VWeaponEditor.Processes;

/// <summary>
/// A service for selecting a process to open
/// </summary>
public interface IProcessSelectionService {
    public static IProcessSelectionService Instance => ApplicationPFX.Instance.ServiceManager.GetService<IProcessSelectionService>();
    
    /// <summary>
    /// Opens a dialog to select a process
    /// </summary>
    /// <returns></returns>
    Task<Process?> SelectProcess();
}