/* ----- ----- ----- ----- */
// AuthManager.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/11/01
// Update Date: 2025/11/01
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Engine.Network
{
    public class AuthManager
    {
        public static string AuthString => NetworkManager.AppVersion;
        public const string AuthSuccessString = "Taki";
        private readonly NetworkManager _networkManager;
        private readonly TaskCompletionSource<bool> _authCompletionSource = new();

        // 用於兩步驗證
        private enum AuthStep
        {
            None,
            VersionSent,
            CredentialsSent,
            Completed
        }

        private AuthStep _currentStep = AuthStep.None;

        // 測試用帳號密碼
        private readonly string _username = "1@test.com";
        private readonly string _password = "1";

        public AuthManager(NetworkManager networkManager)
        {
            _networkManager = networkManager;
            _networkManager.OnPacketReceived += HandlePacket; // 訂閱封包回覆
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
                // 等待伺服器回覆要求帳號密碼
                if (packet.Type == PacketType.AuthRequest && 
                    packet.Data.Trim() == "Please provide username/password")
                {
                    Console.WriteLine("[AuthManager] Server requests credentials.");

                    // Step2: 送帳號密碼
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

                    Console.WriteLine($"[AuthManager] Auth completed → success={success}");
                }
            }
        }
    }
}
