using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.KCC;
using RPGGame.Model;
using UnityEngine;

namespace RPGGame.Gameplay
{
    public class CharacterAnimation : NetworkBehaviour
    {
        [SerializeField] private Animator _animator;

	    [Networked(OnChanged = nameof(OnChangedAnimation))]
	    public int AnimationHash { get; set; }

        public void PlayAnimation(int animationHash)
        {
            AnimationHash = animationHash;
        }

        public void PlayAnimation(string animation)
        {
            AnimationHash = Animator.StringToHash(animation);
        }

        private static void OnChangedAnimation(Changed<CharacterAnimation> changed)
        {
            changed.Behaviour.OnChangedAnimation();
        }

        private void OnChangedAnimation()
        {
            _animator.Play(AnimationHash);
        }
    }
}

