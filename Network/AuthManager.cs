/* ----- ----- ----- ----- */
// AuthManager.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/11/01
// Update Date: 2025/11/01
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Threading.Tasks;

namespace Engine.Network
{
    public class AuthManager
    {
        public static string AuthString => NetworkManager.AppVersion;
        public const string AuthSuccessString = "Taki";
        private readonly NetworkManager _networkManager;
        private readonly TaskCompletionSource<bool> _authCompletionSource = new();

        public AuthManager(NetworkManager networkManager)
        {
            _networkManager = networkManager;
            _networkManager.OnPacketReceived += HandlePacket; // 訂閱封包回覆
        }

        public void SendAuth()
        {
            var authPacket = Packet.Create(
                type: PacketType.AuthRequest,
                senderId: _networkManager.ClientId.ToString(),
                roomId: "",
                data: AuthString
            );

            _networkManager.Send(authPacket);
        }

        public Task<bool> WaitForAuthResponse()
            => _authCompletionSource.Task;

        private void HandlePacket(Packet packet)
        {
            if (packet.Type != PacketType.AuthResponse)
                return;

            bool success = packet.Data == AuthSuccessString;
            _authCompletionSource.TrySetResult(success);

            Console.WriteLine($"[AuthManager] AuthResponse received → success={success}");
        }
    }
}
