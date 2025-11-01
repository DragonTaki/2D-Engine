/* ----- ----- ----- ----- */
// PacketType.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/11/01
// Update Date: 2025/11/01
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Engine.Network
{
    public enum PacketType
    {
        NotDefined,

        // Auth
        AuthRequest,      // 驗證請求
        AuthResponse,     // 驗證回應

        // Room
        JoinRoom,         // 加入房間
        LeaveRoom,        // 離開房間

        // Chess game
        StartGame,        // 標記遊戲開始
        EndGame,          // 標記遊戲終止
        GameAction,       // 棋局行為
        TimerSync,        // 時間同步

        // Chat
        Chat,             // 聊天訊息

        // Other
        Server,           // 伺服器
        Heartbeat,        // 心跳
        Error,            // 錯誤訊息
    }
}
