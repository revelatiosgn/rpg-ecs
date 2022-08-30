using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using RPGGame.Gameplay;
using RPGGame.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPGGame.UiControllers
{
    public class InventorySlotUiController : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _count;

        private InventoryItemConfig _itemConfig;
        public InventoryItemConfig ItemConfig => _itemConfig;

        private void Awake()
        {
            Hide();
        }

        public void Initialize(InventoryItemConfig itemConfig)
        {
            _itemConfig = itemConfig;
            _icon.sprite = _itemConfig.Icon;
            _count.gameObject.SetActive(false);

            Show();
        }

        public void Initialize(InventoryItemConfig itemConfig, int count)
        {
            _itemConfig = itemConfig;
            _icon.sprite = _itemConfig.Icon;
            _count.text = count.ToString();

            Show();
        }

        public void Initialize(InventoryItemConfig itemConfig, string count, Color countColor)
        {
            _itemConfig = itemConfig;
            _icon.sprite = _itemConfig.Icon;
            _count.text = count;
            _count.color = countColor;

            Show();
        }

        public void Show()
        {
            _icon.gameObject.SetActive(true);
            _count.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _icon.gameObject.SetActive(false);
            _count.gameObject.SetActive(false);
        }
    }
}
