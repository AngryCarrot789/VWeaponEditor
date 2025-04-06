using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using GTA;
using GTA.UI;
using JetPacketSystem.Packeting.Ack;
using JetPacketSystem.Sockets;
using JetPacketSystem.Systems;
using VWeaponEditor.Comms;

namespace VWeaponEditor.SHVDN3 {
    /// <summary>
    /// A script used to allow the VWeaponEditor app to communicate with GTA5 internals via a communication channel
    /// </summary>
    public class VWeaponEditorScript : Script {
        private long tickIndex;
        private volatile Socket serverSocket;
        private volatile SocketToClientConnection connectionToApp;
        private volatile ThreadPacketSystem packetSystem;
        private volatile bool isConnecting;
        private SimpleAckProcessor<Packet2GetUserName> processor_2;
        private SimpleAckProcessor<Packet3TranslateHashString> processor_3;

        static VWeaponEditorScript() {
            VWEPacketRegistry.RegisterScriptPackets();
        }
        
        public VWeaponEditorScript() {
            this.KeyDown += this.OnKeyDown;
            this.KeyUp += this.OnKeyUp;
            this.Tick += this.OnTick;
        }

        private static string RandomString() {
            Random rnd = new Random();
            char[] cbuf = new char[4];
            for (int i = 0; i < 4; i++)
                cbuf[i] = (char) ('0' + rnd.Next(0, 10));
            return new string(cbuf);
        }

        private async void ConnectToApp() {
            this.isConnecting = true;
            try {
                if (this.serverSocket == null) {
                    this.serverSocket = SocketHelper.CreateServerSocket(IPAddress.Any, 27354);
                    this.serverSocket.Listen(1);
                }

                try {
                    this.connectionToApp = await SocketHelper.AcceptClientConnectionAsync(this.serverSocket);
                }
                catch {
                    if (this.tickIndex % 60 == 0) {
                        Notification.PostTicker($"[{RandomString()}] Unable to accept client connection", false, false);
                    }
                }

                if (this.packetSystem != null) {
                    this.packetSystem.Dispose();
                    this.packetSystem = null;
                }

                if (this.connectionToApp != null) {
                    this.packetSystem = new ThreadPacketSystem(this.connectionToApp);
                    this.packetSystem.RegisterHandler(this.processor_2 = new SimpleAckProcessor<Packet2GetUserName>(this.packetSystem, (x) => new Packet2GetUserName() { name = Game.Player.Name ?? "<unknown>" }) {AllowResendPacket = false});
                    this.packetSystem.RegisterHandler(this.processor_3 = new SimpleAckProcessor<Packet3TranslateHashString>(this.packetSystem, (x) => new Packet3TranslateHashString() { resp_String = Game.GetLocalizedString(x.req_Hash) }) {AllowResendPacket = false});
                    // this.packetSystem.RegisterListener((p) => {
                    //     Notification.PostTicker($"[{RandomString()}] Received packet: {p.GetType().Name}", false, false);
                    // });
                    
                    this.packetSystem.StartThreads();
                }
            }
            finally {
                this.isConnecting = false;
            }
        }

        private void OnTick(object sender, EventArgs e) {
            if (this.connectionToApp != null && !this.connectionToApp.IsConnected) {
                this.connectionToApp = null;
            }

            if (this.connectionToApp != null && !this.connectionToApp.Client.Connected) {
                this.connectionToApp.Disconnect();
                this.connectionToApp.Dispose();
                this.connectionToApp = null;
            }

            if (this.connectionToApp == null && !this.isConnecting) {
                this.ConnectToApp();
            }

            this.packetSystem?.ProcessReadQueue();
            this.tickIndex++;
        }

        private void OnKeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.F11) {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Server: " + (this.serverSocket == null ? "Invalid" : (this.serverSocket.IsBound ? "Bound" : "Unbound")));
                sb.AppendLine("Client: " + (this.connectionToApp == null ? "Connecting..." : (this.connectionToApp.Client.Connected ? "Connected" : "Socket Disconnected")));
                Notification.PostTicker("Status: \n" + sb.ToString(), true, false);
            }
            else if (e.KeyCode == Keys.F10) {
                if (this.packetSystem != null) {
                    this.packetSystem.Dispose();
                    this.packetSystem = null;
                }
                
                if (this.connectionToApp != null && !this.connectionToApp.Client.Connected) {
                    this.connectionToApp.Disconnect();
                    this.connectionToApp.Dispose();
                    this.connectionToApp = null;
                }
                
                this.serverSocket?.Close();
                this.serverSocket?.Dispose();
                this.serverSocket = null;
                
                Notification.PostTicker("Force disconnected existing client. Reconnecting...", true, false);
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e) {
        }
    }
}