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
        [SerializeField] private KCC _kcc;

	    [Networked(OnChanged = nameof(OnChangedAnimation))]
	    public int AnimationHash { get; set; }

	    [Networked(OnChanged = nameof(OnChangedBehaviour))]
	    public float Behaviour { get; set; }

        public override void FixedUpdateNetwork()
        {
            _animator.SetFloat("MoveSpeed", _kcc.RenderData.RealSpeed);
            float angle = Vector3.SignedAngle(_kcc.RenderData.InputDirection, _kcc.RenderData.LookDirection, Vector3.up);
            _animator.SetFloat("MoveX", -Mathf.Sin(angle * Mathf.Deg2Rad));
            _animator.SetFloat("MoveY", Mathf.Cos(angle * Mathf.Deg2Rad));
        }

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
            changed.Behaviour._animator.Play(changed.Behaviour.AnimationHash);
        }

        private static void OnChangedBehaviour(Changed<CharacterAnimation> changed)
        {
            changed.Behaviour._animator.SetFloat("Behaviour", changed.Behaviour.Behaviour);
        }
    }
}

