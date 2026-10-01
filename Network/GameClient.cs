/* ----- ----- ----- ----- */
// GameClient.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/11/01
// Update Date: 2025/11/01
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Net.Sockets;
using System.Text;

namespace Engine.Network
{
    public class GameClient
    {
        private TcpClient _client;
        private NetworkStream _stream;

        public void Connect(string host, int port)
        {
            _client = new TcpClient();
            _client.Connect(host, port);
            _stream = _client.GetStream();

            BeginRead();
        }

        private void BeginRead()
        {
            var buffer = new byte[4096];
            _stream.BeginRead(buffer, 0, buffer.Length, ar =>
            {
                int bytesRead = _stream.EndRead(ar);
                if (bytesRead > 0)
                {
                    string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    var packet = Packet.Deserialize(json);

                    // TODO: update the game board or countdown timer according to packet.Type
                    //HandlePacket(packet);

                    BeginRead();
                }
            }, null);
        }

        public void Send(Packet packet)
        {
            if (!_client.Connected) return;
            var bytes = Encoding.UTF8.GetBytes(Packet.Serialize(packet) + "\n");
            _stream.Write(bytes, 0, bytes.Length);
        }
    }
}
