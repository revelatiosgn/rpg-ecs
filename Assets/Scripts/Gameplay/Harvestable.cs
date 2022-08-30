using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.KCC;
using RPGGame.Model;
using UnityEngine;

namespace RPGGame.Gameplay
{
    public class Harvestable : NetworkBehaviour
    {
        [SerializeField] private InventoryItemConfig _harvestItem;
        public InventoryItemConfig HarvestItem => _harvestItem;
    }
}

