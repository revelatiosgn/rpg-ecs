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
    public class RoomItemUiController : MonoBehaviour
    {
        [SerializeField] private Button _joinButton;

        private string _sessionName;

        private void Awake()
        {
            _joinButton.onClick.AddListener(OnJoin);
        }

        public void Initialize(SessionInfo sessionInfo)
        {
            _sessionName = sessionInfo.Name;
        }

        private void OnJoin()
        {
            NetworkManager.Instance.JoinSessionTask(_sessionName).Forget();
        }
    }
}
