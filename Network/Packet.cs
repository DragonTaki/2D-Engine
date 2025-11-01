/* ----- ----- ----- ----- */
// Packet.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/11/01
// Update Date: 2025/11/01
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Text.Json;

namespace Engine.Network
{
    public class Packet
    {
        public PacketType Type { get; set; }
        public string RoomId { get; set; } = "";
        public string SenderId { get; set; } = "";
        public string Token { get; set; } = "";    // 驗證用
        public string Data { get; set; }

        public static string Serialize(Packet packet)
            => JsonSerializer.Serialize(packet);

        public static Packet Deserialize(string json)
            => JsonSerializer.Deserialize<Packet>(json);
    }
}
