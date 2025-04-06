using System;
using Avalonia;
using Avalonia.Controls;
using VWeaponEditor.Processes;

namespace VWeaponEditor.Avalonia.Processes;

public class ProcessListBoxItem : ListBoxItem {
    public ProcessInfo ProcessInfo { get; }

    protected override Type StyleKeyOverride => typeof(ListBoxItem);

    public ProcessListBoxItem(ProcessInfo processInfo) {
        this.ProcessInfo = processInfo;
    }

    private void ProcessNameChanged(ProcessInfo sender) {
        this.Content = sender.ProcessName;
    }
    
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e) {
        base.OnAttachedToVisualTree(e);
        this.ProcessInfo.ProcessNameChanged += this.ProcessNameChanged;
        this.ProcessNameChanged(this.ProcessInfo);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e) {
        base.OnDetachedFromVisualTree(e);
        this.ProcessInfo.ProcessNameChanged -= this.ProcessNameChanged;
    }
}