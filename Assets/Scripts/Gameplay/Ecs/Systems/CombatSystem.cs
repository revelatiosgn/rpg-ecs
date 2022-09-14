using System.Collections;
using System.Collections.Generic;
using Example;
using Fusion;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace RPGGame.Gameplay.Ecs
{
    public class CombatSystem : IEcsRunSystem
    {
        private readonly EcsPoolInject<CombatantData> _combatantPool = default;
        private readonly EcsPoolInject<AnimationData> _animationPool = default;

        private int _attackBeginHash = Animator.StringToHash("AttackBegin");

        public void Run(EcsSystems systems)
        {
            foreach (OnPlayerAttackBegin onPlayerAttackBegin in EcsManager.EventBus.GetEvents<OnPlayerAttackBegin>())
            {
                int entity = onPlayerAttackBegin.Entity;

                ref CombatantData combatantData = ref _combatantPool.Value.Get(entity);
                ref AnimationData animationData = ref _animationPool.Value.Get(entity);

                animationData.CharacterAnimation.PlayAnimation(_attackBeginHash);

                Debug.Log("Attack begin!");
            }
        }
    }
}


