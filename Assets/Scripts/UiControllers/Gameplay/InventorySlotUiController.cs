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

        private void Awake()
        {
            Hide();
        }

        public void Initialize(InventoryItemConfig item, int count)
        {
            _icon.sprite = item.Icon;
            _count.text = count.ToString();

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
