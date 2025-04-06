using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using PFXToolKitUI.Avalonia.Bindings;
using PFXToolKitUI.Avalonia.Services.UserInputs;
using PFXToolKitUI.Services.UserInputs;
using PFXToolKitUI.Utils.Collections.Observable;
using VWeaponEditor.Processes;

namespace VWeaponEditor.Avalonia.Processes;

public partial class ProcessSelectionUserInputControl : UserControl, IUserInputContent {
    private readonly IBinder<ProcessSelectionUserInputInfo> selectedIndexBinder = new AutoUpdateAndEventPropertyBinder<ProcessSelectionUserInputInfo>(SelectingItemsControl.SelectedIndexProperty, nameof(ProcessSelectionUserInputInfo.SelectedProcessIndexChanged), (x) => ((ListBox) x.Control).SelectedIndex = x.Model.SelectedProcessIndex, (x) => x.Model.SelectedProcessIndex = ((ListBox) x.Control).SelectedIndex);
    private readonly IBinder<ProcessSelectionUserInputInfo> isRefreshingBinder = new AutoUpdateAndEventPropertyBinder<ProcessSelectionUserInputInfo>(nameof(ProcessSelectionUserInputInfo.IsRefreshingProcessListChanged), (x) => ((ProgressBar) x.Control).IsVisible = x.Model.IsRefreshingProcessList, null);
    
    private UserInputDialog? myDialog;
    private ProcessSelectionUserInputInfo? myInfo;
    private ObservableItemProcessorIndexing<ProcessInfo>? processor;

    public ProcessSelectionUserInputControl() {
        this.InitializeComponent();
        this.PART_ListBox.SelectionMode = SelectionMode.Single;
    }

    public void Connect(UserInputDialog dialog, UserInputInfo info) {
        this.myDialog = dialog;
        this.myDialog.ContentMargin = default;
        this.myInfo = (ProcessSelectionUserInputInfo) info;
        
        this.processor = ObservableItemProcessor.MakeIndexable(this.myInfo.Processes, this.OnItemAdded, this.OnItemRemoved, this.OnItemMoved).AddExistingItems();
        this.selectedIndexBinder.Attach(this.PART_ListBox, this.myInfo);
        this.isRefreshingBinder.Attach(this.PART_ProgressBar, this.myInfo);
    }

    public void Disconnect() {
        this.selectedIndexBinder.Detach();
        this.isRefreshingBinder.Detach();
        this.processor!.RemoveExistingItems().Dispose();
        this.myDialog = null;
        this.myInfo = null;
    }

    public bool FocusPrimaryInput() {
        return false;
    }
    
    private void OnItemAdded(object sender, int index, ProcessInfo item) {
        this.PART_ListBox.Items.Insert(index, new ProcessListBoxItem(item));
    }

    private void OnItemRemoved(object sender, int index, ProcessInfo item) {
        this.PART_ListBox.Items.RemoveAt(index);
    }
    
    private void OnItemMoved(object sender, int oldindex, int newindex, ProcessInfo item) {
        object? obj = this.PART_ListBox.Items[oldindex];
        this.PART_ListBox.Items.RemoveAt(oldindex);
        this.PART_ListBox.Items.Insert(newindex, obj);
    }
}