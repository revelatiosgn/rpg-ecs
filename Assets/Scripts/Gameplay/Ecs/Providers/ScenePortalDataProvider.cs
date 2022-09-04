using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voody.UniLeo.Lite;

namespace RPGGame.Gameplay.Ecs
{
    public sealed class ScenePortalDataProvider : MonoProvider<ScenePortalData>
    {
        [SerializeField] private ScenePortal _scenePortal;

        private void Awake()
        {
            value.ScenePortal = _scenePortal;
        }
    }
}


