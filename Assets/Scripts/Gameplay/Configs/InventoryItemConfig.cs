using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Gameplay
{
    [CreateAssetMenu(menuName = "RPG/Inventory/ItemConfig")]
    public class InventoryItemConfig : ItemConfig
    {
        [SerializeField] private Sprite _icon;
        public Sprite Icon => _icon;
    }
}
