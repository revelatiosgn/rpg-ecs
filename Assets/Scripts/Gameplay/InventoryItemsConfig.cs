using System.Collections;
using System.Collections.Generic;
using RPGGame.Model;
using UnityEngine;

namespace RPGGame.Gameplay
{
    [CreateAssetMenu(menuName = "RPG/Inventory/ItemsConfig")]
    public class InventoryItemsConfig : ScriptableObject
    {
        [SerializeField] private List<InventoryItemConfig> _items;
        public List<InventoryItemConfig> Items => _items;

        private Dictionary<string, InventoryItemConfig> _cache;

        public InventoryItemConfig GetItem(string id)
        {
            if (_cache == null)
                InitializeCache();

            return _cache[id];
        }

        private void InitializeCache()
        {
            _cache = new Dictionary<string, InventoryItemConfig>(Constants.Player.InventorySlots);

            foreach (InventoryItemConfig item in _items)
                _cache[item.ID] = item;
        }
    }
}
