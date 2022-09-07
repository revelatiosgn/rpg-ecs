using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.KCC;
using RPGGame.Gameplay.Ecs;
using RPGGame.Model;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Voody.UniLeo.Lite;

namespace RPGGame.Gameplay
{
    public class Interactor : NetworkBehaviour
    {
        [SerializeField] private TriggerEvent _triggerEvent;
        [SerializeField] private LayerMask _layerMask;

        [Networked, Capacity(100), UnitySerializeField]
        public NetworkDictionary<NetworkId, Interactable> Interactables => default;

        public Interactable Target;

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
			    Mouse mouse = Mouse.current;
                if (mouse != null && mouse.leftButton.wasReleasedThisFrame && !EventSystem.current.IsPointerOverGameObject())
                {
                    Ray ray = Camera.main.ScreenPointToRay(mouse.position.ReadValue());
                    if (Physics.Raycast(ray, out RaycastHit hit, 100f, _layerMask))
                    {
                        InteractableCollider collider = hit.collider.GetComponent<InteractableCollider>();
                        Interactable target = collider.Interactable;
                        if (Interactables.TryGet(target.Object.Id, out Interactable interactable))
                        {
                            RPC_Interact(interactable.Object.Id);
                        }
                    }
                }
            }
        }
        
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        private void RPC_Interact(NetworkId interactableId)
        {
            if (Object.HasStateAuthority)
            {
                if (Interactables.TryGet(interactableId, out Interactable interactable))
                {
                    EcsManager.EventBus.RaiseEvent<OnPlayerInteract>(new OnPlayerInteract { 
                        SourceEntity = GetComponent<EntityObject>().Id,
                        TargetEntity = interactable.GetComponent<EntityObject>().Id
                    });
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
                if (Target != null)
                {
                    EcsManager.EventBus.RaiseEvent<OnPlayerInterruptInteract>(new OnPlayerInterruptInteract { 
                        SourceEntity = GetComponent<EntityObject>().Id,
                        TargetEntity = Target.GetComponent<EntityObject>().Id
                    });
                }
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

