using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using RPGGame.Gameplay;
using RPGGame.Gameplay.Events;
using RPGGame.Model;
using UnityEngine;
using UnityEngine.UI;

namespace RPGGame.UiControllers
{
    public class InventoryUiController : MonoBehaviour
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Transform _itemsList;
        [SerializeField] private InventorySlotUiController _slotPrefab;
        [SerializeField] private InventoryItemsConfig _itemsConfig;
        [SerializeField] private GameplayEvents _gameplayEvents;

        private List<InventorySlotUiController> _slots = new List<InventorySlotUiController>(Constants.Player.InventorySlots);

        public event Action Closed;

        private void Awake()
        {
            _closeButton.onClick.AddListener(OnClose);

            Initialize();
        }

        private void OnEnable()
        {
            _gameplayEvents.OnInventoryChanged += OnInventoryChanged;

            if (PlayerCharacter.Local != null)
                FillInventory(PlayerCharacter.Local.GetComponent<Inventory>().Items);
        }

        private void OnDisable()
        {
            _gameplayEvents.OnInventoryChanged -= OnInventoryChanged;
        }

        private void Initialize()
        {
            while (_slots.Count < Constants.Player.InventorySlots)
            {
                InventorySlotUiController slot = Instantiate(_slotPrefab, _itemsList);
                _slots.Add(slot);
            }
        }

        private void OnClose()
        {
            Closed?.Invoke();
        }

        private void OnInventoryChanged(NetworkDictionary<NetworkString<_32>, int> items)
        {
            FillInventory(items);
        }

        private void FillInventory(NetworkDictionary<NetworkString<_32>, int> items)
        {
            int index = 0;

            foreach (var item in items)
            {
                _slots[index].Initialize(_itemsConfig.GetItem(item.Key.Value), item.Value);
                index++;
            }

            for (int i = index; i < _slots.Count; i++)
            {
                _slots[index].Hide();
            }
        }
    }
}
