using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.KCC;
using RPGGame.Model;
using UnityEngine;

namespace RPGGame.Gameplay
{
    public class Interactor : NetworkBehaviour
    {
        [SerializeField] private TriggerEvent _triggerEvent;
        [SerializeField] private LayerMask _layerMask;

        [Networked, Capacity(100)]
        public NetworkLinkedList<NetworkId> Interactables => default;

        [Networked(OnChanged = nameof(OnChangedCurrentInteractable))]
        public NetworkId CurrentInteractable { get; set; }

        private Interactable _interactTarget;
        public Interactable InteractTarget => _interactTarget;

        private void OnEnable()
        {
            _triggerEvent.TriggerEnter += TriggerEnter;
            _triggerEvent.TriggerExit += TriggerExit;
        }

        private void OnDisable()
        {
            _triggerEvent.TriggerEnter -= TriggerEnter;
            _triggerEvent.TriggerExit -= TriggerExit;
        }

        private void Update()
        {
            if (Object.HasInputAuthority)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit, 100f, _layerMask))
                    {
                        InteractableCollider collider = hit.collider.GetComponent<InteractableCollider>();
                        Interactable interactable = collider.Interactable;
                        if (Interactables.Contains(interactable.Object.Id))
                        {
                            // Debug.Log("Call Interact RPC");
                            RPC_Interact(interactable.Object.Id);
                        }
                    }
                }
            }
        }
        
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        private void RPC_Interact(NetworkId networkId)
        {
            if (Object.HasStateAuthority)
            {
                Interactable interactable = Interactable.GetInteractable(networkId);
                _interactTarget = interactable;
                Debug.Log($"RPC_Interact {NetworkManager.Instance.GetPlayer(Object.InputAuthority).Nickname} with {interactable.gameObject.name}");
            }
        }

        private static void OnChangedCurrentInteractable(Changed<Interactor> changed)
        {

        }

        public void ResetTarget()
        {
            _interactTarget = null;
            CurrentInteractable = default;
        }

        private void TriggerEnter(Collider collider)
        {
            if (collider.TryGetComponent<Interactable>(out Interactable interactable))
            {
                if (Object.HasStateAuthority)
                    Interactables.Add(interactable.Object.Id);

                if (Object.HasInputAuthority)
                    interactable.SetUiActive(true);
            }
        }

        private void TriggerExit(Collider collider)
        {
            if (collider.TryGetComponent<Interactable>(out Interactable interactable))
            {
                if (Object.HasStateAuthority)
                    Interactables.Remove(interactable.Object.Id);

                if (Object.HasInputAuthority)
                    interactable.SetUiActive(false);
            }
        }
    }
}

