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
using System.Threading.Tasks;

namespace Engine.Network
{
    public class NetworkManager
    {
        public const string AppVersion = "v1.0.0";
        private readonly string _host;
        private readonly int _port;

        public Guid ClientId { get; private set; }

        private AuthManager _authManager;

        private TcpClient _client;
        private NetworkStream _stream;
        private CancellationTokenSource _cts;

        private readonly object _lock = new();

        // Serializes Send: the heartbeat timer thread and callers write concurrently.
        private readonly object _sendLock = new();

        public bool IsConnected => _client?.Connected ?? false;

        public event Action<Packet> OnPacketReceived;
        public event Action OnDisconnected;

        private Timer _heartbeatTimer;
        private DateTime _lastHeartbeat = DateTime.UtcNow;

        private const int HeartbeatInterval = 1000;      // Unit: millisecond
        private const int ServerTimeoutLimit = 8 * 1000;  // Unit: millisecond

        public NetworkManager(string host = "127.0.0.1", int port = 8080)
        {
            _host = host;
            _port = port;
            ClientId = Guid.NewGuid();
        }

        public async Task<bool> ConnectAsync()
        {
            TcpClient client;
            CancellationTokenSource cts;
            lock (_lock)
            {
                // _client stays set from here until Disconnect, so a second call while this
                // one is still connecting/authenticating returns instead of opening a
                // parallel connection (IsConnected is still false at that point).
                if (_client != null)
                    return false;

                cts = _cts = new CancellationTokenSource();
                client = _client = new TcpClient();
            }

            try
            {
                await client.ConnectAsync(_host, _port);
                _stream = client.GetStream();
            }
            catch (Exception ex) when (ex is SocketException || ex is ObjectDisposedException || ex is InvalidOperationException)
            {
                // Server unreachable, or Disconnect closed the client meanwhile. Release
                // this attempt (only if it is still the current one) so a retry can start,
                // instead of leaving a dead _client behind and an unobserved exception.
                Console.WriteLine("[NetworkManager] Connect failed: " + ex.Message);
                bool isCurrent;
                lock (_lock)
                    isCurrent = _client == client;
                if (isCurrent)
                    Disconnect();
                else
                    client.Dispose();
                return false;
            }

            StartListening(cts.Token);

            // Initialize AuthManager (a local: Disconnect may clear the field meanwhile)
            var authManager = new AuthManager(this);
            lock (_lock)
                _authManager = authManager;
            authManager.SendAuth();

            // Wait for the auth result (false as well when Disconnect abandoned the attempt)
            bool authSuccess = await authManager.WaitForAuthResponse();

            if (!authSuccess)
            {
                // Only tear down if this attempt is still the current one: after a
                // Disconnect (e.g. from Reconnect) a newer attempt may already be running.
                bool isCurrent;
                lock (_lock)
                    isCurrent = _authManager == authManager;
                if (isCurrent)
                {
                    Console.WriteLine("[NetworkManager] Auth failed, disconnecting...");
                    Disconnect();
                }
                return false;
            }

            StartHeartbeat();
            Console.WriteLine("[NetworkManager] Network connected.");
            return true;
        }

        public void Disconnect()
        {
            lock (_lock)
            {
                _cts?.Cancel();
                _heartbeatTimer?.Dispose();
                _authManager?.Dispose();
                _stream?.Close();
                _client?.Close();

                _cts = null;
                _authManager = null;
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
            _ = ConnectAsync();
        }

        private void StartHeartbeat()
        {
            _heartbeatTimer?.Dispose();
            // Start the timeout window now: the last heartbeat may be from an earlier
            // connection, which would time a reconnect out on the first tick.
            _lastHeartbeat = DateTime.UtcNow;
            _heartbeatTimer = new Timer(_ =>
            {
                if (!IsConnected) return;

                if ((DateTime.UtcNow - _lastHeartbeat).TotalMilliseconds > ServerTimeoutLimit)
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
#nullable enable
                    string? line = await reader.ReadLineAsync();
#nullable disable
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

                    // Only the parse is guarded here; the packet is raised once, below
                    // (it used to also be raised inside this try, so every non-heartbeat
                    // packet reached subscribers twice and heartbeats once).
                    Packet packet;
                    try
                    {
                        packet = Packet.Deserialize(line);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[NetworkManager] Invalid JSON: {line}\n{ex.Message}");
                        continue;
                    }

                    // The JSON literal "null" deserializes to no packet at all.
                    if (packet == null)
                    {
                        Console.WriteLine($"[NetworkManager] Empty packet ignored: {line}");
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
            catch (Exception ex)
            {
                Console.WriteLine("[NetworkManager] Receive error: " + ex.Message);
                Disconnect();
            }
        }

        public void Send(Packet packet)
        {
            // One read of the field: Disconnect may null it between the check and the write.
            var stream = _stream;
            if (_client?.Connected == true && stream != null)
            {
                try
                {
                    string json = Packet.Serialize(packet) + "\n";
                    byte[] data = System.Text.Encoding.UTF8.GetBytes(json);
                    // One packet at a time, so two packets' bytes never interleave on the stream.
                    lock (_sendLock)
                        stream.Write(data, 0, data.Length);
                }
                catch
                {
                    Disconnect();
                }
            }
        }
    }
}
