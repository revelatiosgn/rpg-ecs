using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voody.UniLeo.Lite;

namespace RPGGame.Gameplay.Ecs
{
    public sealed class InteractableDataProvider : MonoProvider<InteractableData>
    {
        [SerializeField] private Interactable _interactable;

        private void Awake()
        {
            value.Interactable = _interactable;
        }
    }
}


