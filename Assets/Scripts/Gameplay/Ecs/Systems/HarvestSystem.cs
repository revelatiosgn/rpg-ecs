using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace RPGGame.Gameplay.Ecs
{
    public class HarvestSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<InteractorData, AnimationData>> _interactorFilter = default;
        
        private readonly EcsPoolInject<InteractorData> _interactorPool = default;
        private readonly EcsPoolInject<AnimationData> _animationPool = default;
        private readonly EcsPoolInject<HarvestableData> _harvestablePool = default;

        private int _slashHash = Animator.StringToHash("Slash");

        public void Run(EcsSystems systems)
        {
            foreach (OnInteractBegin onInteractBegin in EcsManager.EventBus.GetEvents<OnInteractBegin>())
            {
                Debug.Log($"begin {onInteractBegin.InteractorEntity} {onInteractBegin.InteractableEntity}");

                if (_harvestablePool.Value.Has(onInteractBegin.InteractableEntity))
                {
                    _animationPool.Value.Get(onInteractBegin.InteractorEntity).CharacterAnimation.PlayAnimation(_slashHash);
                }
            }
        }
    }
}


