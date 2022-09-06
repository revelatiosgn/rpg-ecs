using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using RPGGame.Gameplay.Events;
using RPGGame.Model;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPGGame.UiControllers
{
    public class GameplayUiController : MonoBehaviour
    {
        [Header("Ui")]
        [SerializeField] private Transform _background;
        [SerializeField] private InventoryUiController _inventory;
        [SerializeField] private CraftUiController _craft;

        [Header("Actions")]
        [SerializeField] private InputActionReference _inventoryAction;
        [SerializeField] private InputActionReference _cancelAction;

        [Header("Events")]
        [SerializeField] private GameplayEvents _gameplayEvents;

        private State _state;

        private void Awake()
        {
            _inventory.Closed += () => SetState(State.Game);
            _craft.Closed += () => SetState(State.Game);
        }

        private void OnEnable()
        {
            _gameplayEvents.OnWorkbench += OnWorkbench;
        }

        private void OnDisable()
        {
            _gameplayEvents.OnWorkbench -= OnWorkbench;
        }

        private void Start()
        {
            _inventoryAction.action.Enable();
            _cancelAction.action.Enable();

            SetState(State.Game);
        }

        private void Update()
        {
            if (_state == State.Game)
            {
                if (_inventoryAction.action.WasReleasedThisFrame())
                    SetState(State.Inventory);
            }
            else if (_state == State.Inventory)
            {
                if (_inventoryAction.action.WasReleasedThisFrame() || _cancelAction.action.WasReleasedThisFrame())
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

                case State.Craft:
                    Show(_craft.transform);
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
            _craft.gameObject.SetActive(false);
        }

        private void OnWorkbench(NetworkString<_32> workbenchId)
        {
            if (workbenchId != "0")
            {
                SetState(State.Craft);
                _craft.Initialize(workbenchId);
            }
            else
            {
                SetState(State.Game);
            }
            
        }

        private enum State
        {
            None,
            Game,
            Inventory,
            Craft
        }
    }
}
