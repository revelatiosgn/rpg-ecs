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

        public override void Spawned()
        {
            SetUiActive(false);
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
        }

        public void SetUiActive(bool value)
        {
            _ui.gameObject.SetActive(value);
        }
    }
}

