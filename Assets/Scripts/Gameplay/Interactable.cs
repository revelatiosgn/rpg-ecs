using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.KCC;
using RPGGame.Model;
using UnityEngine;

namespace RPGGame.Gameplay
{
    public class Interactable : NetworkBehaviour
    {
        [SerializeField] private WorldUi _ui;

        private static Dictionary<NetworkId, Interactable> _interactables = new Dictionary<NetworkId, Interactable>();

        public override void Spawned()
        {
            _interactables[Object.Id] = this;

            SetUiActive(false);
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            _interactables.Remove(Object.Id);
        }

        public void SetUiActive(bool value)
        {
            _ui.gameObject.SetActive(value);
        }

        public static Interactable GetInteractable(NetworkId networkId)
        {
            return _interactables[networkId];
        }
    }
}

