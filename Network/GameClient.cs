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
        private TcpClient client;
        private NetworkStream stream;

        public void Connect(string host, int port)
        {
            client = new TcpClient();
            client.Connect(host, port);
            stream = client.GetStream();

            BeginRead();
        }

        private void BeginRead()
        {
            var buffer = new byte[4096];
            stream.BeginRead(buffer, 0, buffer.Length, ar =>
            {
                int bytesRead = stream.EndRead(ar);
                if (bytesRead > 0)
                {
                    string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    var packet = Packet.Deserialize(json);

                    // TODO: 根據 packet.Type 更新棋局或倒計時
                    //HandlePacket(packet);

                    BeginRead();
                }
            }, null);
        }

        public void Send(Packet packet)
        {
            if (!client.Connected) return;
            var bytes = Encoding.UTF8.GetBytes(Packet.Serialize(packet) + "\n");
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}
