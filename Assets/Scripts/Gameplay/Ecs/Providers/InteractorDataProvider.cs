using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voody.UniLeo.Lite;

namespace RPGGame.Gameplay.Ecs
{
    public sealed class InteractorDataProvider : MonoProvider<InteractorData>
    {
        [SerializeField] private Interactor _interactor;

        private void Awake()
        {
            value.Interactor = _interactor;
        }
    }
}


