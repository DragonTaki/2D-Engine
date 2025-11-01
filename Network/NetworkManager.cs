/* ----- ----- ----- ----- */
// NetworkManager.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/11/01
// Update Date: 2025/11/01
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Engine.Network
{
    public class NetworkManager
    {
        private readonly string _host;
        private readonly int _port;
        
        public Guid ClientId { get; private set; }

        private TcpClient _client;
        private NetworkStream _stream;
        private CancellationTokenSource _cts;

        private readonly object _lock = new();

        public bool IsConnected => _client?.Connected ?? false;

        public event Action<Packet> OnPacketReceived;
        public event Action OnDisconnected;

        private Timer _heartbeatTimer;
        private DateTime _lastHeartbeat = DateTime.UtcNow;

        private const int HeartbeatInterval = 1000;      // Unit: millisecond
        private const int TimeoutLimit = 5 * 60 * 1000;  // Unit: millisecond

        public NetworkManager(string host = "127.0.0.1", int port = 8080)
        {
            _host = host;
            _port = port;
            ClientId = Guid.NewGuid();
        }

        public void Connect()
        {
            lock (_lock)
            {
                if (IsConnected)
                    return;

                _cts = new CancellationTokenSource();
                _client = new TcpClient();
                _client.Connect(_host, _port);
                _stream = _client.GetStream();

                StartListening(_cts.Token);
                StartHeartbeat();

                Console.WriteLine("[NetworkManager] Network connected.");
            }
        }

        public void Disconnect()
        {
            lock (_lock)
            {
                _cts?.Cancel();
                _heartbeatTimer?.Dispose();
                _stream?.Close();
                _client?.Close();

                _cts = null;
                _stream = null;
                _client = null;

                OnDisconnected?.Invoke();

                Console.WriteLine("[NetworkManager] Network disconnected.");
            }
        }

        public void Reconnect()
        {
            Disconnect();
            Console.WriteLine("[NetworkManager] Network reconnecting...");
            Connect();
        }

        private void StartHeartbeat()
        {
            _heartbeatTimer?.Dispose();
            _heartbeatTimer = new Timer(_ =>
            {
                if (!IsConnected) return;

                // 超過5分鐘沒收到心跳 → 斷線
                if ((DateTime.UtcNow - _lastHeartbeat).TotalMilliseconds > TimeoutLimit)
                {
                    Console.WriteLine("[NetworkManager] Heartbeat timeout. Disconnecting...");
                    Disconnect();
                    return;
                }

                SendHeartbeat();

            }, null, 0, HeartbeatInterval);
        }

        private void SendHeartbeat()
        {
            try
            {
                if (IsConnected)
                {
                    var packet = new Packet
                    {
                        Type = PacketType.Heartbeat,
                        SenderId = ClientId.ToString(),
                        Data = ""
                    };
                    Send(packet);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[NetworkManager] Failed to send heartbeat: " + ex.Message);
                Disconnect();
            }
        }

        public void ReceiveHeartbeat()
        {
            _lastHeartbeat = DateTime.UtcNow;
        }

        private async void StartListening(CancellationToken token)
        {
            var reader = new StreamReader(_stream);

            try
            {
                while (!token.IsCancellationRequested && _client.Connected)
                {
                    string? line = await reader.ReadLineAsync();
                    if (line == null)
                    {
                        Disconnect();
                        break;
                    }

                    // If packet empty
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        Console.WriteLine("[NetworkManager] Empty line ignored.");
                        continue;
                    }

                    // If packet not JSON
                    if (!line.TrimStart().StartsWith("{"))
                    {
                        Console.WriteLine($"[NetworkManager] Not JSON, ignored. Message: {line}");
                        continue;
                    }

                    Packet packet;
                    try
                    {
                        packet = Packet.Deserialize(line);
                        OnPacketReceived?.Invoke(packet);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[NetworkManager] Invalid JSON: {line}\n{ex.Message}");
                        continue;
                    }

                    if (packet.Type == PacketType.Heartbeat)
                    {
                        ReceiveHeartbeat();
                        continue;
                    }

                    OnPacketReceived?.Invoke(packet);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("[NetworkManager] Receive error: " + ex.Message);
                Disconnect();
            }
        }

        public void Send(Packet packet)
        {
            if (_client?.Connected == true && _stream != null)
            {
                try
                {
                    string json = Packet.Serialize(packet) + "\n";
                    byte[] data = System.Text.Encoding.UTF8.GetBytes(json);
                    _stream.Write(data, 0, data.Length);
                }
                catch
                {
                    Disconnect();
                }
            }
        }
    }
}
