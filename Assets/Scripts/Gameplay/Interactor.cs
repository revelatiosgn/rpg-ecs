using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.KCC;
using RPGGame.Model;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RPGGame.Gameplay
{
    public class Interactor : NetworkBehaviour
    {
        [SerializeField] private TriggerEvent _triggerEvent;
        [SerializeField] private LayerMask _layerMask;

        [Networked, Capacity(100), UnitySerializeField]
        public NetworkDictionary<NetworkId, Interactable> Interactables => default;

        public Interactable IntendedInteractable;
        public Interactable TargetInteractable;

        [Networked(OnChanged = nameof(OnChangedTargetInteractableId))]
        public NetworkId TargetInteractableId { get; set; }

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
                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit, 100f, _layerMask))
                    {
                        InteractableCollider collider = hit.collider.GetComponent<InteractableCollider>();
                        Interactable target = collider.Interactable;
                        if (Interactables.TryGet(target.Object.Id, out Interactable interactable))
                        {
                            RPC_IntendInteract(interactable.Object.Id);
                        }
                    }
                }
            }
        }
        
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        private void RPC_IntendInteract(NetworkId interactableId)
        {
            if (Object.HasStateAuthority)
            {
                if (Interactables.TryGet(interactableId, out Interactable interactable))
                {
                    IntendedInteractable = interactable;
                    Debug.Log($"RPC_IntendInteract {NetworkManager.Instance.GetPlayer(Object.InputAuthority).Nickname} with {IntendedInteractable.gameObject.name}");
                }
            }
        }

        public void InterruptInteract()
        {
            if (Object.HasInputAuthority)
            {
                RPC_InterruptInteract();
            }
        }
        
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        private void RPC_InterruptInteract()
        {
            if (Object.HasStateAuthority)
            {
                IntendedInteractable = null;
            }
        }

        private static void OnChangedTargetInteractableId(Changed<Interactor> changed)
        {
            Interactor interactor = changed.Behaviour;
            if (interactor.Interactables.TryGet(interactor.TargetInteractableId, out Interactable interactable))
            {
                interactor.TargetInteractable = interactable;
            }
            else
            {
                interactor.TargetInteractable = null;
            }
        }

        private void TriggerEnter(Collider collider)
        {
            if (collider.TryGetComponent<Interactable>(out Interactable interactable))
            {
                if (Object.HasStateAuthority)
                    Interactables.Set(interactable.Object.Id, interactable);

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

