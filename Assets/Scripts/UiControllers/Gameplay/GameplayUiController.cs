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
    public class GameplayUiController : MonoBehaviour
    {
        [SerializeField] private Transform _background;
        [SerializeField] private InventoryUiController _inventory;

        private State _state;

        private void Awake()
        {
            _inventory.Closed += () => SetState(State.Game);
        }

        private void Start()
        {
            SetState(State.Game);
        }

        private void Update()
        {
            if (_state == State.Game)
            {
                if (Input.GetKeyDown(KeyCode.I))
                    SetState(State.Inventory);
            }
            else if (_state == State.Inventory)
            {
                if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Escape))
                    SetState(State.Game);
            }
        }

        private void SetState(State state)
        {
            _state = state;
            switch (_state)
            {
                case State.None:
                case State.Game:
                    Hide();
                    break;

                case State.Inventory:
                    Show(_inventory.transform);
                    break;
            }
        }

        private void Show(Transform transform)
        {
            Hide();

            _background.gameObject.SetActive(true);
            transform.gameObject.SetActive(true);
        }

        private void Hide()
        {
            _background.gameObject.SetActive(false);
            _inventory.gameObject.SetActive(false);
        }

        private enum State
        {
            None,
            Game,
            Inventory
        }
    }
}
