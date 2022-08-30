using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using RPGGame.Model;
using UnityEngine;

namespace RPGGame.UiControllers
{
    public class LobbyUiController : MonoBehaviour
    {
        [SerializeField] private Transform _loadingTransform;
        [SerializeField] private RoomsUiController _roomsUiController;
        [SerializeField] private SessionUiController _sessionUiController;

        private void Awake()
        {
            _loadingTransform.gameObject.SetActive(true);
            _roomsUiController.gameObject.SetActive(true);
            _sessionUiController.gameObject.SetActive(true);
        }

        private void Start()
        {
            NetworkManager.Instance.JoinLobbyStarted += JoinLobbyStarted;
            NetworkManager.Instance.JoinLobbyCompleted += JoinLobbyCompleted;
            NetworkManager.Instance.CreateSessionStarted += CreateSessionStarted;
            NetworkManager.Instance.CreateSessionCompleted += CreateSessionCompleted;
            NetworkManager.Instance.LeaveSessionStarted += LeaveSessionStarted;
            NetworkManager.Instance.JoinSessionStarted += JoinSessionStarted;
            NetworkManager.Instance.JoinSessionCompleted += JoinSessionCompleted;
        }

        private void OnDestroy()
        {
            NetworkManager.Instance.JoinLobbyStarted -= JoinLobbyStarted;
            NetworkManager.Instance.JoinLobbyCompleted -= JoinLobbyCompleted;
            NetworkManager.Instance.CreateSessionStarted -= CreateSessionStarted;
            NetworkManager.Instance.CreateSessionCompleted -= CreateSessionCompleted;
            NetworkManager.Instance.LeaveSessionStarted -= LeaveSessionStarted;
            NetworkManager.Instance.JoinSessionStarted -= JoinSessionStarted;
            NetworkManager.Instance.JoinSessionCompleted -= JoinSessionCompleted;
        }

        private void JoinLobbyStarted()
        {
            ShowSection(_loadingTransform.gameObject);
        }

        private void JoinLobbyCompleted(StartGameResult result)
        {
            if (result.Ok)
            {
                ShowSection(_roomsUiController.gameObject);
            }
            else
            {
                Debug.LogError($"JoinLobbyCompleted {result.ToString()}");
            }
        }

        private void CreateSessionStarted()
        {
            ShowSection(_loadingTransform.gameObject);
        }

        private void CreateSessionCompleted(StartGameResult result)
        {
            if (result.Ok)
            {
                ShowSection(_sessionUiController.gameObject);
            }
            else
            {
                Debug.LogError($"CreateSessionCompleted {result.ToString()}");
            }
        }

        private void LeaveSessionStarted()
        {
            ShowSection(_loadingTransform.gameObject);
        }

        private void JoinSessionStarted()
        {
            ShowSection(_loadingTransform.gameObject);
        }

        private void JoinSessionCompleted(StartGameResult result)
        {
            if (result.Ok)
            {
                ShowSection(_sessionUiController.gameObject);
            }
            else
            {
                Debug.LogError($"JoinSessionCompleted {result.ToString()}");
            }
        }

        private void ShowSection(GameObject gameObject)
        {
            _loadingTransform.gameObject.SetActive(false);
            _roomsUiController.gameObject.SetActive(false);
            _sessionUiController.gameObject.SetActive(false);

            gameObject.SetActive(true);
        }
    }
}
