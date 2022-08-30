using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.KCC;
using RPGGame.Model;
using TMPro;
using UnityEngine;

namespace RPGGame.Gameplay
{
    public abstract class ItemsCollectionConfig<T> : ScriptableObject where T : ItemConfig
    {
        [SerializeField] private List<T> _items;
        public List<T> Items => _items;

        private Dictionary<string, T> _cache;

        public T GetItem(string id)
        {
            if (_cache == null)
                InitializeCache();

            return _cache[id];
        }

        private void InitializeCache()
        {
            _cache = new Dictionary<string, T>(_items.Count);

            foreach (T item in _items)
                _cache[item.ID] = item;
        }
    }
}

