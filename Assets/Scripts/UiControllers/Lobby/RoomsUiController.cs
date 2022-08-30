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
    public class RoomsUiController : MonoBehaviour
    {
        [SerializeField] private Button _hostButton;
        [SerializeField] private Transform _roomsList;
        [SerializeField] private RoomItemUiController _roomItemPrefab;
        [SerializeField] private TMP_InputField _nicknameInput;

        private void Awake()
        {
            _hostButton.onClick.AddListener(OnHostButton);
            
            if (PlayerPrefs.HasKey(Constants.Prefs.Player.Nickname))
            {
                string playerName = PlayerPrefs.GetString(Constants.Prefs.Player.Nickname);
                _nicknameInput.text = playerName;
            }

            _nicknameInput.onDeselect.AddListener(OnNicknameInputSubmit);
        }

        private void Start()
        {
            NetworkManager.Instance.SessionListUpdated += SessionListUpdated;
        }

        private void OnDestroy()
        {
            NetworkManager.Instance.SessionListUpdated -= SessionListUpdated;
        }

        private void OnHostButton()
        {
            NetworkManager.Instance.CreateSessionTask().Forget();
        }

        private void SessionListUpdated(List<SessionInfo> sessionList)
        {
            foreach (Transform child in _roomsList.transform)
                Destroy(child.gameObject);

            foreach (SessionInfo sessionInfo in sessionList)
            {
                RoomItemUiController roomItem = Instantiate(_roomItemPrefab, _roomsList.transform);
                roomItem.Initialize(sessionInfo);
            }
        }

        private void OnNicknameInputSubmit(string nickname)
        {
            PlayerPrefs.SetString(Constants.Prefs.Player.Nickname, nickname);
        }
    }
}
