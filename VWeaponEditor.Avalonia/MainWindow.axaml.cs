using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Interactivity;
using PFXToolKitUI;
using PFXToolKitUI.Avalonia.Themes.Controls;
using PFXToolKitUI.Utils;
using PFXToolKitUI.Utils.Commands;
using VWeaponEditor.Avalonia.MemoryUtils;
using VWeaponEditor.Comms;
using VWeaponEditor.Processes;

namespace VWeaponEditor.Avalonia;

public partial class MainWindow : WindowEx {
    private readonly AsyncRelayCommand openProcessCommand;
    private readonly AsyncRelayCommand connectScriptCommand;
    private Mem64? mem;
    private IntPtr addr_of_world;
    private IntPtr addr_of_weapon_manager;
    private IntPtr addr_of_curr_veh_wpn_inf; // pointer to the current vehicle weapon info
    private Process? process;
    private Task updateTask;
    private bool isRefreshingValues;

    public ScriptNetworkManager ScriptNetworkManager { get; } = new ScriptNetworkManager();

    public MainWindow() {
        this.InitializeComponent();
        
        VWEPacketRegistry.RegisterApplicationPackets();
        
        this.PART_ReloadTimeSP.ValueFinalized += (s,a) => { if (!this.isRefreshingValues && this.mem != null) WeaponInfo.reload_time_sp(this.mem, Mem64.Dereference(this.mem.ProcessHandle, this.addr_of_curr_veh_wpn_inf), (float) this.PART_ReloadTimeSP.Value); }; 
        this.PART_ReloadTimeMP.ValueFinalized += (s,a) => { if (!this.isRefreshingValues && this.mem != null) WeaponInfo.reload_time_mp(this.mem, Mem64.Dereference(this.mem.ProcessHandle, this.addr_of_curr_veh_wpn_inf), (float) this.PART_ReloadTimeMP.Value); }; 
        this.PART_VehicleReloadTime.ValueFinalized += (s,a) => { if (!this.isRefreshingValues && this.mem != null) WeaponInfo.vehicle_reload_time(this.mem, Mem64.Dereference(this.mem.ProcessHandle, this.addr_of_curr_veh_wpn_inf), (float) this.PART_VehicleReloadTime.Value); }; 
        this.PART_AnimReloadTime.ValueFinalized += (s,a) => { if (!this.isRefreshingValues && this.mem != null) WeaponInfo.anim_reload_time(this.mem, Mem64.Dereference(this.mem.ProcessHandle, this.addr_of_curr_veh_wpn_inf), (float) this.PART_AnimReloadTime.Value); }; 
        this.PART_BulletsPerAnimLoop.ValueFinalized += (s,a) => { if (!this.isRefreshingValues && this.mem != null) WeaponInfo.bullets_per_anime_loop(this.mem, Mem64.Dereference(this.mem.ProcessHandle, this.addr_of_curr_veh_wpn_inf), (int) this.PART_BulletsPerAnimLoop.Value); }; 
        this.PART_TimeBetweenShots.ValueFinalized += (s,a) => { if (!this.isRefreshingValues && this.mem != null) WeaponInfo.time_between_shots(this.mem, Mem64.Dereference(this.mem.ProcessHandle, this.addr_of_curr_veh_wpn_inf), (float) this.PART_TimeBetweenShots.Value); }; 
        this.PART_SpinUpTime.ValueFinalized += (s,a) => { if (!this.isRefreshingValues && this.mem != null) WeaponInfo.spinup_time(this.mem, Mem64.Dereference(this.mem.ProcessHandle, this.addr_of_curr_veh_wpn_inf), (float) this.PART_SpinUpTime.Value); }; 
        this.PART_SpinDownTime.ValueFinalized += (s,a) => { if (!this.isRefreshingValues && this.mem != null) WeaponInfo.spindown_time(this.mem, Mem64.Dereference(this.mem.ProcessHandle, this.addr_of_curr_veh_wpn_inf), (float) this.PART_SpinDownTime.Value); }; 
        this.PART_AltWaitTime.ValueFinalized += (s,a) => { if (!this.isRefreshingValues && this.mem != null) WeaponInfo.alternate_wait_time(this.mem, Mem64.Dereference(this.mem.ProcessHandle, this.addr_of_curr_veh_wpn_inf), (float) this.PART_AltWaitTime.Value); }; 
        this.PART_Range.ValueFinalized += (s,a) => {
            if (!this.isRefreshingValues && this.mem != null) {
                IntPtr addr = Mem64.Dereference(this.mem.ProcessHandle, this.addr_of_curr_veh_wpn_inf);
                WeaponInfo.lock_on_range(this.mem, addr, (float) this.PART_Range.Value);
                WeaponInfo.weapon_range(this.mem, addr, (float) this.PART_Range.Value);
            }
        }; 
        
        this.openProcessCommand = new AsyncRelayCommand(async () => {
            Process? proc = await ApplicationPFX.Instance.ServiceManager.GetService<IProcessSelectionService>().SelectProcess();
            if (proc == null) {
                return;
            }

            const int OFFSET_1 = 0x8;    // Offset to assume CWorld::curr_player_ped
            const int OFFSET_2 = 0x10B8; // Offset to CPed::m_weapon_manager
            const int OFFSET_3 = 0x0070; // Offset to CPedWeaponManager::m_vehicle_weapon_info

            await ApplicationPFX.Instance.Dispatcher.InvokeAsync(() => {
                this.process = proc;
                this.mem = new Mem64(proc);
                IntPtr addr = PatternScanner.FindPattern(this.mem.Process, "48 8B 05 ? ? ? ? 45 ? ? ? ? 48 8B 48 08 48 85 C9 74 07");
                this.addr_of_world = addr + this.mem.ReadInt32(addr + 3) + 7;
                
                // deref the world object and player object and add offset to m_weapon_manager
                this.addr_of_weapon_manager = Mem64.Dereference(this.mem.ProcessHandle, this.addr_of_world, new int[] { OFFSET_1 }) + OFFSET_2;
                
                // deref the player's weapon manager and add offset to m_vehicle_weapon_info
                this.addr_of_curr_veh_wpn_inf = Mem64.Dereference(this.mem.ProcessHandle, this.addr_of_weapon_manager) + OFFSET_3;

                this.RefreshUI();

                if (this.updateTask == null || this.updateTask.IsCompleted) {
                    this.updateTask = Task.Run(async () => {
                        while (this.mem != null) {
                            await ApplicationPFX.Instance.Dispatcher.InvokeAsync(this.RefreshUI);
                            await Task.Delay(1000);
                        }
                    });
                }
            });
        });
        
        this.connectScriptCommand = new AsyncRelayCommand(async () => {
            await this.ScriptNetworkManager.ConnectAsync();
        });
    }

    private void RefreshUI() {
        if (this.mem == null) {
            return;
        }
        
        // deref vehicle weapon info object
        IntPtr addr_curr_veh_wpn_inf = Mem64.Dereference(this.mem.ProcessHandle, this.addr_of_curr_veh_wpn_inf);
        if (addr_curr_veh_wpn_inf == IntPtr.Zero) {
            // null-ptr; not in a vehicle maybe?
            this.PART_EditorPanel.IsEnabled = false;
            return;
        }
        
        this.PART_EditorPanel.IsEnabled = true;
        
        // // read 0x150 bytes ahead of weapon info object, which is the AlternateWaitTime field
        // float awt1 = this.mem.ReadFloat(addr_curr_veh_wpn_inf + 0x150);
        unsafe {
            // read entire CWeaponInfo object into temporary memory, saves reading process memory lots of time
            byte* info = stackalloc byte[0x990];
            if (!this.mem.Read(addr_curr_veh_wpn_inf, info, 0x990)) {
                // maybe process was closed or pointer is invalid for some reason.
                this.mem = null;
                return;
            }
            
            // float awt = WeaponInfo.alternate_wait_time(info);
            this.isRefreshingValues = true;
            this.ScriptNetworkManager.CurrentVehicleHash = (int) WeaponInfo.human_name_hash(info);
            
            // this is updated every 1 second
            this.PART_WeaponName.Text = string.IsNullOrEmpty(this.ScriptNetworkManager.CurrentVehicleName) ? "<no vehicle weapon>" : this.ScriptNetworkManager.CurrentVehicleName; 
            
            if (!this.PART_ReloadTimeSP.IsEditing && !this.PART_ReloadTimeSP.IsDragging)
                this.PART_ReloadTimeSP.Value = WeaponInfo.reload_time_sp(info);
            if (!this.PART_ReloadTimeMP.IsEditing && !this.PART_ReloadTimeMP.IsDragging)
                this.PART_ReloadTimeMP.Value = WeaponInfo.reload_time_mp(info);
            if (!this.PART_VehicleReloadTime.IsEditing && !this.PART_VehicleReloadTime.IsDragging)
                this.PART_VehicleReloadTime.Value = WeaponInfo.vehicle_reload_time(info);
            if (!this.PART_AnimReloadTime.IsEditing && !this.PART_AnimReloadTime.IsDragging)
                this.PART_AnimReloadTime.Value = WeaponInfo.anim_reload_time(info);
            if (!this.PART_BulletsPerAnimLoop.IsEditing && !this.PART_BulletsPerAnimLoop.IsDragging)
                this.PART_BulletsPerAnimLoop.Value = WeaponInfo.bullets_per_anime_loop(info);
            if (!this.PART_TimeBetweenShots.IsEditing && !this.PART_TimeBetweenShots.IsDragging)
                this.PART_TimeBetweenShots.Value = WeaponInfo.time_between_shots(info);
            if (!this.PART_SpinUpTime.IsEditing && !this.PART_SpinUpTime.IsDragging)
                this.PART_SpinUpTime.Value = WeaponInfo.spinup_time(info);
            if (!this.PART_SpinDownTime.IsEditing && !this.PART_SpinDownTime.IsDragging)
                this.PART_SpinDownTime.Value = WeaponInfo.spindown_time(info);
            if (!this.PART_AltWaitTime.IsEditing && !this.PART_AltWaitTime.IsDragging)
                this.PART_AltWaitTime.Value = WeaponInfo.alternate_wait_time(info);
            if (!this.PART_Range.IsEditing && !this.PART_Range.IsDragging)
                this.PART_Range.Value = WeaponInfo.lock_on_range(info);
            this.isRefreshingValues = false;
        }
    }

    private void OpenProcess_MenuItemClick(object? sender, RoutedEventArgs e) {
        this.openProcessCommand.Execute(null);
    }
    
    private void ConnectScript_MenuItemClick(object? sender, RoutedEventArgs e) {
        this.connectScriptCommand.Execute(null);
    }
}