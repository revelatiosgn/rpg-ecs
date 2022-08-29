using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace RPGGame.Gameplay.Ecs
{
    public class AnimationSystem : IEcsRunSystem
    {
        public void Run(EcsSystems systems)
        {
            foreach(TestEvent testEvent in EcsManager.EventBus.GetPool<TestEvent>())
            {
                Debug.Log($"AnimationSystem event {testEvent.message} : {testEvent.ival}");
            }
        }
    }
}


