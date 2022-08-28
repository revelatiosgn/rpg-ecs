using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voody.UniLeo.Lite;

namespace RPGGame.Gameplay.Ecs
{
    public sealed class AnimationDataProvider : MonoProvider<AnimationData>
    {
        [SerializeField] private CharacterAnimation _characterAnimation;

        private void Awake()
        {
            value.CharacterAnimation = _characterAnimation;
        }
    }
}


