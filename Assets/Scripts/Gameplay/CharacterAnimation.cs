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
        [SerializeField] private NetworkMecanimAnimator _networkAnimator;
        [SerializeField] private Animator _animator;
        [SerializeField] private KCC _kcc;

	    [Networked(OnChanged = nameof(OnChangedAnimation))]
	    public AnimParam Animation { get; set; } = AnimParam.Defaults;

	    [Networked(OnChanged = nameof(OnChangedLayerWeight))]
	    public WeightParam LayerWeight { get; set; } = WeightParam.Defaults;

	    [Networked(OnChanged = nameof(OnChangedBehaviour))]
	    public float Behaviour { get; set; }

        public override void FixedUpdateNetwork()
        {
            _animator.SetFloat("MoveSpeed", _kcc.RenderData.RealSpeed);
            float angle = Vector3.SignedAngle(_kcc.RenderData.InputDirection, _kcc.RenderData.LookDirection, Vector3.up);
            _animator.SetFloat("MoveX", -Mathf.Sin(angle * Mathf.Deg2Rad));
            _animator.SetFloat("MoveY", Mathf.Cos(angle * Mathf.Deg2Rad));
        }

        public void PlayAnimation(int hash, AnimatorLayer layer = AnimatorLayer.Base, float transitionDuration = 0f)
        {
            if (HasStateAuthority)
            {
                Animation = new AnimParam {
                    Layer = layer,
                    Hash = hash,
                    TransitionDuration = transitionDuration
                };
            }

            if (HasInputAuthority)
            {
                if (transitionDuration == 0f)
                    _animator.Play(hash, (int) layer);
                else
                    _animator.CrossFade(hash, transitionDuration, (int) layer);
            }
        }

        public void PlayAnimation(string name, AnimatorLayer layer = AnimatorLayer.Base, float transitionDuration = 0f)
        {
            PlayAnimation(Animator.StringToHash(name), layer, transitionDuration);
        }

        public void SetLayerWeight(float weight, AnimatorLayer layer)
        {
            if (HasStateAuthority)
            {
                LayerWeight = new WeightParam {
                    Layer = layer,
                    Weight = weight
                };
            }

            if (HasInputAuthority)
            {
                _animator.SetLayerWeight((int) layer, weight);
            }
        }

        private static void OnChangedAnimation(Changed<CharacterAnimation> changed)
        {
            AnimParam animation = changed.Behaviour.Animation;
            if (animation.TransitionDuration == 0f)
                changed.Behaviour._animator.Play(animation.Hash, (int) animation.Layer);
            else
                changed.Behaviour._animator.CrossFade(animation.Hash, animation.TransitionDuration, (int) animation.Layer);
        }

        private static void OnChangedLayerWeight(Changed<CharacterAnimation> changed)
        {
            WeightParam layerWeight = changed.Behaviour.LayerWeight;
            changed.Behaviour._animator.SetLayerWeight((int) layerWeight.Layer, layerWeight.Weight);
        }

        private static void OnChangedBehaviour(Changed<CharacterAnimation> changed)
        {
            changed.Behaviour._animator.SetFloat("Behaviour", changed.Behaviour.Behaviour);
            changed.Behaviour.SetLayerWeight(changed.Behaviour.Behaviour, AnimatorLayer.Combat);
        }

        public enum AnimatorLayer
        {
            Base,
            Labor,
            Combat
        }

        [System.Serializable]
        public struct AnimParam : INetworkStruct
        {
            public AnimatorLayer Layer;
            public int Hash;
            public float TransitionDuration;

            public static AnimParam Defaults
            {
                get
                {
                    return new AnimParam() {
                        Layer = AnimatorLayer.Base,
                        Hash = Animator.StringToHash("Move"),
                        TransitionDuration = 0f
                    };
                }
            }
        }

        [System.Serializable]
        public struct WeightParam : INetworkStruct
        {
            public AnimatorLayer Layer;
            public float Weight;

            public static WeightParam Defaults
            {
                get
                {
                    return new WeightParam() {
                        Layer = AnimatorLayer.Base,
                        Weight = 1f
                    };
                }
            }
        }
    }
}

