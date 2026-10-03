/* ----- ----- ----- ----- */
// AuthManager.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/11/01
// Update Date: 2026/10/04
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Text.Json;
using System.Threading.Tasks;

using Engine.Diagnostics;

namespace Engine.Network
{
    /// <summary>
    /// Runs the two-step login for one connection attempt of <see cref="NetworkManager"/>.
    /// Subscribed to <see cref="NetworkManager.OnPacketReceived"/> only until the attempt
    /// finishes or is abandoned (<see cref="Dispose"/>), so a reconnect never leaves an old
    /// instance reacting to the new connection's packets.
    /// </summary>
    public class AuthManager : IDisposable
    {
        public static string AuthString => NetworkManager.AppVersion;
        public const string AuthSuccessString = "Taki";
        private readonly NetworkManager _networkManager;
        // Continuations run asynchronously: Dispose completes this from inside
        // NetworkManager.Disconnect (under its lock), and the awaiting ConnectAsync must not
        // resume on that stack.
        private readonly TaskCompletionSource<bool> _authCompletionSource = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private bool _disposed;

        // Steps of the two-step authentication (version check, then credentials)
        private enum AuthStep
        {
            None,
            VersionSent,
            CredentialsSent,
            Completed
        }

        private AuthStep _currentStep = AuthStep.None;

        // Test account credentials
        private readonly string _username = "1@test.com";
        private readonly string _password = "1";

        public AuthManager(NetworkManager networkManager)
        {
            _networkManager = networkManager;
            _networkManager.OnPacketReceived += HandlePacket; // Subscribe to incoming packets (the server's replies)
        }

        public void SendAuth()
        {
            _currentStep = AuthStep.VersionSent;

            var versionObj = new
            {
                type = "version",
                senderId = _networkManager.ClientId.ToString(),
                version = AuthString
            };

            var authPacket = Packet.Create(
                type: PacketType.AuthRequest,
                senderId: _networkManager.ClientId.ToString(),
                roomId: "",
                data: JsonSerializer.Serialize(versionObj)
            );

            _networkManager.Send(authPacket);
        }

        public Task<bool> WaitForAuthResponse()
            => _authCompletionSource.Task;

        private void HandlePacket(Packet packet)
        {
            if (_currentStep == AuthStep.Completed)
                return;

            if (_currentStep == AuthStep.VersionSent)
            {
                // Wait for the server to ask for username/password
                if (packet.Type == PacketType.AuthRequest &&
                    packet.Data?.Trim() == "Please provide username/password")
                {
                    if (DebugOptions.ConsoleTrace)
                        Console.WriteLine("[AuthManager] Server requests credentials.");

                    // Step 2: send username/password
                    var credentialsObj = new
                    {
                        type = "credentials",
                        senderId = _networkManager.ClientId.ToString(),
                        username = _username,
                        password = _password
                    };

                    var credentialsPacket = Packet.Create(
                        type: PacketType.AuthRequest,
                        senderId: _networkManager.ClientId.ToString(),
                        roomId: "",
                        data: JsonSerializer.Serialize(credentialsObj)
                    );

                    _networkManager.Send(credentialsPacket);
                    _currentStep = AuthStep.CredentialsSent;
                }
            }

            if (_currentStep == AuthStep.CredentialsSent)
            {
                if (packet.Type == PacketType.AuthResponse)
                {
                    bool success = packet.Data == AuthSuccessString;
                    _authCompletionSource.TrySetResult(success);
                    _currentStep = AuthStep.Completed;
                    _networkManager.OnPacketReceived -= HandlePacket;

                    if (DebugOptions.ConsoleTrace)
                        Console.WriteLine($"[AuthManager] Auth completed → success={success}");
                }
            }
        }

        /// <summary>
        /// Stops listening for packets and, if the login hasn't finished, completes
        /// <see cref="WaitForAuthResponse"/> with false (the connection went away), so the
        /// caller awaiting it doesn't wait forever.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;

            _networkManager.OnPacketReceived -= HandlePacket;
            _currentStep = AuthStep.Completed;
            _authCompletionSource.TrySetResult(false);
        }
    }
}
