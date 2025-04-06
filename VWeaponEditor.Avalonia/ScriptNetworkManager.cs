using System;
using System.Net;
using System.Threading.Tasks;
using JetPacketSystem.Packeting.Ack;
using JetPacketSystem.Sockets;
using JetPacketSystem.Systems;
using PFXToolKitUI;
using PFXToolKitUI.Services.Messaging;
using VWeaponEditor.Comms;

namespace VWeaponEditor.Avalonia;

public delegate void ScriptNetworkManagerPlayerNameChangedEventHandler(ScriptNetworkManager sender);

public delegate void ScriptNetworkManagerCurrentVehicleHashChangedEventHandler(ScriptNetworkManager sender);

public delegate void ScriptNetworkManagerCurrentVehicleNameChangedEventHandler(ScriptNetworkManager sender);

/// <summary>
/// Manages a connection to the VWE script (which uses ScriptHookVDotNet)
/// </summary>
public class ScriptNetworkManager {
    public SocketToServerConnection? ConnectionToScript { get; private set; }
    public ThreadPacketSystem? PacketSystem { get; private set; }
    public AckProcessor<Packet2GetUserName>? Processor2 { get; private set; }
    public AckProcessor<Packet3TranslateHashString>? Processor3 { get; private set; }

    private string playerName;
    private int currentVehicleHash;
    private string currentVehicleName;

    public string PlayerName {
        get => this.playerName;
        private set {
            if (this.playerName == value)
                return;
            this.playerName = value;
            this.PlayerNameChanged?.Invoke(this);
        }
    }

    public int CurrentVehicleHash {
        get => this.currentVehicleHash;
        set {
            if (this.currentVehicleHash == value)
                return;
            this.currentVehicleHash = value;
            this.CurrentVehicleHashChanged?.Invoke(this);
        }
    }

    public string CurrentVehicleName {
        get => this.currentVehicleName;
        set {
            if (this.currentVehicleName == value)
                return;
            this.currentVehicleName = value;
            this.CurrentVehicleNameChanged?.Invoke(this);
        }
    }


    public event ScriptNetworkManagerPlayerNameChangedEventHandler? PlayerNameChanged;
    public event ScriptNetworkManagerCurrentVehicleHashChangedEventHandler? CurrentVehicleHashChanged;
    public event ScriptNetworkManagerCurrentVehicleNameChangedEventHandler? CurrentVehicleNameChanged;

    public ScriptNetworkManager() {
    }

    public async Task ConnectAsync() {
        if (this.ConnectionToScript != null && this.ConnectionToScript.IsConnected && this.ConnectionToScript.Socket!.Connected) {
            return;
        }

        this.Dispose();

        try {
            this.ConnectionToScript = await SocketHelper.MakeConnectionToServerAsync(IPAddress.Loopback, 27354);
        }
        catch (Exception e) {
            await IMessageDialogService.Instance.ShowMessage("TCP Failure", $"Failed to connect to server: {e.Message}");
            return;
        }

        ThreadPacketSystem system = this.PacketSystem = new ThreadPacketSystem(this.ConnectionToScript);
        this.PacketSystem.RegisterHandler(this.Processor2 = new AckProcessor<Packet2GetUserName>(this.PacketSystem) { AllowResendPacket = false });
        this.PacketSystem.RegisterHandler(this.Processor3 = new AckProcessor<Packet3TranslateHashString>(this.PacketSystem) { AllowResendPacket = false });
        this.PacketSystem.StartThreads();

        long lastKeepAlivePacket = DateTime.Now.Ticks;
        long keepAliveInterval = TimeSpan.FromSeconds(5).Ticks;

        long lastUpdateCurrVehName = lastKeepAlivePacket;
        long updateCurrVehicleNameInterval = TimeSpan.FromSeconds(1).Ticks;
        Task.Run(async () => {
            Task? currUpdateVehicleNameTask = null;
            while (this.PacketSystem == system) {
                long now = DateTime.Now.Ticks;
                if ((now - lastKeepAlivePacket) > keepAliveInterval) {
                    lastKeepAlivePacket = now;
                    system.SendPacket(new Packet1KeepAlive());
                }

                system.ProcessReadQueue();

                if ((now - lastUpdateCurrVehName) > updateCurrVehicleNameInterval) {
                    lastUpdateCurrVehName = now;
                    if (this.CurrentVehicleHash != 0 && (currUpdateVehicleNameTask == null || currUpdateVehicleNameTask.IsCompleted)) {
                        currUpdateVehicleNameTask = Task.Run(async () => {
                            Packet3TranslateHashString response = await this.Processor3.MakeRequestAsync(new Packet3TranslateHashString() { req_Hash = this.CurrentVehicleHash });
                            await ApplicationPFX.Instance.Dispatcher.InvokeAsync(() => {
                                this.CurrentVehicleName = response.resp_String;
                            });
                        });
                    }
                }

                await Task.Delay(10);
            }
        });

        Packet2GetUserName response = await this.Processor2.MakeRequestAsync(new Packet2GetUserName());
        this.PlayerName = response.name ?? "";
    }

    public void Dispose() {
        if (this.PacketSystem != null) {
            this.PacketSystem.Dispose();
            this.PacketSystem = null;
        }

        if (this.ConnectionToScript != null && this.ConnectionToScript.IsConnected) {
            this.ConnectionToScript.Disconnect();
            this.ConnectionToScript.Dispose();
            this.ConnectionToScript = null;
        }
    }
}