using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using RPGGame.Model;
using UnityEngine;
using UnityEngine.UI;

namespace RPGGame.UiControllers
{
    public class SessionUiController : MonoBehaviour
    {
        [SerializeField] private Button _leaveButton;
        [SerializeField] private Button _startGameButton;
        [SerializeField] private Transform _playersList;
        [SerializeField] private PlayerItemUiController _playerItemPrefab;

        private SessionInfo _sessionInfo;

        private void Awake()
        {
            _leaveButton.onClick.AddListener(OnLeave);
            _startGameButton.onClick.AddListener(OnStartGame);
        }

        private void Start()
        {
            NetworkManager.Instance.PlayerSpawned += PlayerSpawned;
            NetworkManager.Instance.PlayerDespawned += PlayerDespawned;
        }

        private void OnDestroy()
        {
            NetworkManager.Instance.PlayerSpawned -= PlayerSpawned;
            NetworkManager.Instance.PlayerDespawned -= PlayerDespawned;
        }

        private void Initialize(SessionInfo sessionInfo)
        {
            _sessionInfo = sessionInfo;
        }

        private void OnLeave()
        {
            NetworkManager.Instance.LeaveSessionTask().Forget();
        }

        private void OnStartGame()
        {
            NetworkManager.Instance.StartGame();
        }

        private void PlayerSpawned(Player player)
        {
            UpdatePlayersList();
        }

        private void PlayerDespawned(Player player)
        {
            UpdatePlayersList();
        }

        private void UpdatePlayersList()
        {
            foreach (Transform child in _playersList.transform)
                Destroy(child.gameObject);

            foreach (Player player in NetworkManager.Instance.Players)
            {
                PlayerItemUiController playerItem = Instantiate(_playerItemPrefab, _playersList.transform);
                playerItem.Initialize(player);
            }
        }
    }
}
