using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using RPGGame.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPGGame.UiControllers
{
    public class PlayerItemUiController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nickname;

        private NetworkId _playerId;

        private void Start()
        {
            NetworkManager.Instance.PlayerUpdated += PlayerUpdated;
        }

        private void OnDestroy()
        {
            NetworkManager.Instance.PlayerUpdated -= PlayerUpdated;
        }

        public void Initialize(Player player)
        {
            _playerId = player.Object.Id;
            _nickname.text = player.Nickname.Value;
        }

        private void PlayerUpdated(Player player)
        {
            if (player.Object.Id == _playerId)
                _nickname.text = player.Nickname.Value;
        }
    }
}
