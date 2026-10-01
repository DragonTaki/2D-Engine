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
        AuthRequest,      // Authentication request
        AuthResponse,     // Authentication response

        // Room
        JoinRoom,         // Join a room
        LeaveRoom,        // Leave a room

        // Chess game
        StartGame,        // Marks the start of a game
        EndGame,          // Marks the end of a game
        GameAction,       // A game action (e.g. a move)
        TimerSync,        // Timer synchronization

        // Chat
        Chat,             // Chat message

        // Other
        Server,           // Server message
        Heartbeat,        // Heartbeat
        Error,            // Error message
    }
}
