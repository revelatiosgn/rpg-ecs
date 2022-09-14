using System.Collections;
using System.Collections.Generic;
using Example;
using Fusion;
using Fusion.KCC;
using RPGGame.Gameplay.Ecs;
using RPGGame.Model;
using TMPro;
using UnityEngine;
using Voody.UniLeo.Lite;

namespace RPGGame.Gameplay
{
    public class Combatant : NetworkBehaviour
    {
        [SerializeField] private EntityObject _entityObject;
        [SerializeField] private ThirdPersonPlayerController _playerController;
        [SerializeField] private CharacterAnimation _characterAnimation;

        private void Awake()
        {
            _playerController.OnAttackBegin += OnAttackBegin;
            _playerController.OnAttackEnd += OnAttackEnd;
        }

        private void OnDestroy()
        {
            _playerController.OnAttackBegin -= OnAttackBegin;
            _playerController.OnAttackEnd -= OnAttackEnd;
        }

        private void OnAttackBegin()
        {
			// EcsManager.EventBus.RaiseEvent<OnPlayerAttackBegin>(new OnPlayerAttackBegin { Entity = _entityObject.Id });
            _characterAnimation.PlayAnimation("AttackBegin", CharacterAnimation.AnimatorLayer.Combat);
        }

        private void OnAttackEnd()
        {
             _characterAnimation.PlayAnimation("AttackEnd", CharacterAnimation.AnimatorLayer.Combat, 0.2f);
        }
    }
}

