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

        [Networked, Capacity(100)]
        public NetworkLinkedList<NetworkId> Interactables => default;

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

