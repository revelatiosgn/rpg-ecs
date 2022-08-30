using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voody.UniLeo.Lite;

namespace RPGGame.Gameplay.Ecs
{
    public sealed class InventoryDataProvider : MonoProvider<InventoryData>
    {
        [SerializeField] private Inventory _inventory;

        private void Awake()
        {
            value.Inventory = _inventory;
        }
    }
}


