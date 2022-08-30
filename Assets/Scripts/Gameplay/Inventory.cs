using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.KCC;
using RPGGame.Gameplay.Events;
using RPGGame.Model;
using UnityEngine;

namespace RPGGame.Gameplay
{
    public class Inventory : NetworkBehaviour
    {
        [Networked(OnChanged = nameof(OnInventoryChanged)), Capacity(64)]
        public NetworkDictionary<NetworkString<_32>, int> Items => default; // <itemid, count>

        [SerializeField] private GameplayEvents _gameplayEvents;

        private static void OnInventoryChanged(Changed<Inventory> changed)
        {
            if (changed.Behaviour.HasInputAuthority)
                changed.Behaviour._gameplayEvents.OnInventoryChanged?.Invoke(changed.Behaviour.Items);
        }
    }
}

