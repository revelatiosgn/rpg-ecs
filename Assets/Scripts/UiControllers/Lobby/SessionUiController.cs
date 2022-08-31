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
using static TMPro.TMP_Dropdown;

namespace RPGGame.UiControllers
{
    public class SessionUiController : MonoBehaviour
    {
        [SerializeField] private Button _leaveButton;
        [SerializeField] private Button _startGameButton;
        [SerializeField] private Transform _playersList;
        [SerializeField] private PlayerItemUiController _playerItemPrefab;
        [SerializeField] private CharactersConfig _charactersConfig;
        [SerializeField] private TMP_Dropdown _charactersDropdown;

        private SessionInfo _sessionInfo;

        private void Awake()
        {
            _leaveButton.onClick.AddListener(OnLeave);
            _startGameButton.onClick.AddListener(OnStartGame);
            _charactersDropdown.onValueChanged.AddListener(OnCharacterChaged);
        }

        private void Start()
        {
            NetworkManager.Instance.PlayerSpawned += PlayerSpawned;
            NetworkManager.Instance.PlayerDespawned += PlayerDespawned;

            List<OptionData> options = new List<OptionData>();
            _charactersConfig.PlayerCharacterPrefabs.ForEach(prefab => {
                options.Add(new OptionData(prefab.gameObject.name));
            });

            _charactersDropdown.ClearOptions();
            _charactersDropdown.AddOptions(options);
            _charactersDropdown.value = 0;
        }

        private void OnDestroy()
        {
            NetworkManager.Instance.PlayerSpawned -= PlayerSpawned;
            NetworkManager.Instance.PlayerDespawned -= PlayerDespawned;
        }

        public void Initialize(SessionInfo sessionInfo)
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

        private void PlayerSpawned(PlayerRef playerRef, Player player)
        {
            UpdatePlayersList();
        }

        private void PlayerDespawned(PlayerRef playerRef, Player player)
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

        private void OnCharacterChaged(int characterIndex)
        {
            Player.Local.SetCharacterIndex(characterIndex);
        }
    }
}
