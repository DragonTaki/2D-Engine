/* ----- ----- ----- ----- */
// Packet.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/11/01
// Update Date: 2025/11/01
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Engine.Network
{
    public class Packet
    {
        public PacketType Type { get; set; }
        public string SenderId { get; set; } = "";
        public string RoomId { get; set; } = "";
        public string Token { get; set; } = "";    // 驗證用
        public string Data { get; set; }

        private static readonly JsonSerializerOptions _options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            Converters = { new JsonStringEnumConverter() }
        };

        public static string Serialize(Packet packet)
            => JsonSerializer.Serialize(packet, _options);

        public static Packet Deserialize(string json)
            => JsonSerializer.Deserialize<Packet>(json, _options)!;

        public static Packet Create(PacketType type, string senderId, string roomId, string data)
        {
            return new Packet
            {
                Type = type,
                SenderId = senderId,
                RoomId = roomId,
                Data = data,
                Token = ""
            };
        }
    }
}
