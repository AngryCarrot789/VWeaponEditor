// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Sockets;
using JetPacketSystem.Packeting;
using JetPacketSystem.Packeting.Ack;
using JetPacketSystem.Sockets;
using JetPacketSystem.Streams;
using JetPacketSystem.Systems;
using JetPacketSystem.Systems.Handling;

namespace JetPacketSystem.Tests;

public static class Program {
    private static volatile bool IsRunningServer;
    private static volatile ThreadPacketSystem currentPacketSystem;
    
    private delegate bool ConvertDelegate<T>(string? input, out T output);
    
    private static T Read<T>(string message, ConvertDelegate<T> converter) {
        T value;
        do {
            Console.Write($"{message} >");
        } while (!converter(Console.ReadLine(), out value));
        return value;
    }
    
    private static async Task<T> ReadAsync<T>(string message, ConvertDelegate<T> converter) {
        T value;
        do {
            Console.Write($"{message} >");
        } while (!converter(await Task.Run(Console.ReadLine), out value));
        return value;
    }
    
    private static int ReadInt(string message) => Read(message, (string input, out int output) => int.TryParse(input, out output));

    public static void Main(string[] args) {
        Packet.Register(1, () => new Packet1Increment());
        Packet.Register(2, () => new Packet2GetName());
        
        string line = Read("Client or Server (c/s)", (string input, out string output) => (output = input) == "s" || input == "c");
        int port = ReadInt("Port");
        Task task = Task.Run(async () => {
            if (line == "s") {
                IsRunningServer = true;
                await RunServer(port);
            }
            else {
                IsRunningServer = false;
                await RunClient(port);
            }
        });

        while (!task.IsCompleted) {
            // Dispatch received packets to handlers
            currentPacketSystem?.ProcessReadQueue();
            
            Thread.Sleep(1);
        }
    }
    
    public static async Task RunServer(int port) {
        Socket socket = SocketHelper.CreateServerSocket(IPAddress.Any, port);
        socket.Listen(1);

        while (true) {
            Console.WriteLine("Accepting client...");
            SocketToClientConnection client;
            try {
                client = await SocketHelper.AcceptClientConnectionAsync(socket);
            }
            catch {
                continue;
            }
            
            Console.WriteLine($"Connected to {client.Client.LocalEndPoint}");
            await RunCommandLoop(true, new ThreadPacketSystem(client));
            client.Disconnect();
        }
    }

    public static async Task RunClient(int port) {
        SocketToServerConnection connection = await SocketHelper.MakeConnectionToServerAsync(IPAddress.Loopback, port);
        Console.WriteLine($"Connected to {connection.Socket.LocalEndPoint}");
        await RunCommandLoop(false, new ThreadPacketSystem(connection));
        connection.Disconnect();
        connection.Dispose();
    }
    
    private static async Task RunCommandLoop(bool isServer, ThreadPacketSystem system) {
        // Register packet handlers and listeners
        system.RegisterListener<Packet1Increment>((s, p) => {
            Console.WriteLine($"{(isServer ? "Server" : "Client")} received increment instruction. Incr {p.index} by {p.incr}");
        });

        SimpleAckProcessor<Packet2GetName> processor2 = system.RegisterHandler(new SimpleAckProcessor<Packet2GetName>(system, (p) => new Packet2GetName() { name = isServer ? "Server" : "Client" }) {AllowResendPacket = false}, Priority.HIGHEST);
        system.StartThreads();
        currentPacketSystem = system;
        
        while (true) {
            string line = await ReadAsync("Input Command", (string? input, out string output) => !string.IsNullOrEmpty(output = input));
            switch (line) {
                case "stop": return;
                case "incr": {
                    system.SendPacket(new Packet1Increment() { index = ReadInt("Index"), incr = ReadInt("Increment By") });
                    Console.WriteLine($"Send increment request");
                    break;
                }
                case "getname": {
                    Console.Write("Making request for name... ");
                    string name = (await processor2.MakeRequestAsync(new Packet2GetName())).name;
                    Console.WriteLine($"response: {name}");
                    break;
                }
            }
        }
    }

    public class Packet1Increment : Packet {
        public int index, incr;
        
        public override void ReadPayLoad(IDataInput input) {
            this.index = input.ReadInt();
            this.incr = input.ReadInt();
        }

        public override void WritePayload(IDataOutput output) {
            output.WriteInt(this.index);
            output.WriteInt(this.incr);
        }
    }
    
    public class Packet2GetName : PacketACK {
        public string name;

        public override void WritePayloadToServer(IDataOutput output) {
        }

        public override void ReadPayloadFromClient(IDataInput input) {
        }

        public override void WritePayloadToClient(IDataOutput output) {
            output.WriteStringLabelledUTF16(this.name);
        }
        
        public override void ReadPayloadFromServer(IDataInput input) {
            this.name = input.ReadStringUTF16Labelled();
        }
    }
}