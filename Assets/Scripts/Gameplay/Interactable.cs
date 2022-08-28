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

        private void Awake()
        {
            _ui.gameObject.SetActive(false);
        }

        public void SetUiActive(bool value)
        {
            _ui.gameObject.SetActive(value);
        }
    }
}

