using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPGGame.Model
{
    public static class Constants
    {
        public static class Prefs
        {
            public static class Player
            {
                public const string Nickname = "Player.Nickname";
            }
        }

        public static class Server
        {
            public const int MaxPlayers = 16;
        }

        public static class Player
        {
            public const int InventorySlots = 64;
        }
    }
}
