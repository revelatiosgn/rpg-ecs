using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace RPGGame.Gameplay.Events
{
    [CreateAssetMenu(menuName = "RPG/Events/GameplayEvents")]
    public class GameplayEvents : BaseEvents
    {
        public Action<NetworkDictionary<NetworkString<_32>, int>> OnInventoryChanged;
        public Action<NetworkString<_32>> OnWorkbench;
        public Action<float> OnCraftProgressChanged;
    }
}
