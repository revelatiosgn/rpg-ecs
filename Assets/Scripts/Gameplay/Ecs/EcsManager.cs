using System.Collections;
using System.Collections.Generic;
using Fusion;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using RPGGame.Model;
using UnityEngine;
using Voody.UniLeo.Lite;

namespace RPGGame.Gameplay.Ecs
{
    public class EcsManager : NetworkBehaviour
    {
        private EcsWorld _world;
        private EcsSystems _systems;
        private EcsSharedData _sharedData;

        private static NetworkRunner _networkRunner;
        public static NetworkRunner NetworkRunner => _networkRunner;
        public static float DeltaTime => _networkRunner.DeltaTime;

        private static EcsEventBus _eventBus;
        public static EcsEventBus EventBus => _eventBus;

        private IEcsRunSystem[] _runSystems = null;

        private void Start()
        {
            if (NetworkManager.Instance.NetworkRunner.IsServer)
                Setup();
        }

        private void Setup()
        {
            _world = new EcsWorld();
            _sharedData = new EcsSharedData();
            _systems = new EcsSystems(_world, _sharedData);

            _systems.ConvertScene();
            _systems
                .Add(new AnimationSystem())
                .Add(new InteractionSystem())
                .Add(new HarvestSystem())
                .Add(new CraftSystem())
                .Add(new ScenePortalSystem())
                .Inject()
                .Init();

            _systems.GetRunSystems(ref _runSystems);
            _eventBus = new EcsEventBus(_runSystems.Length);   
        }

        public override void Spawned()
        {
            _networkRunner = Runner;
        }

        public override void FixedUpdateNetwork()
        {
            if (NetworkManager.Instance.NetworkRunner.IsServer)
            {
                for (int i = 0; i < _runSystems.Length; i++)
                {
                    _eventBus.Swallow();
                    _runSystems[i].Run(_systems);
                }
            }
        }

        private void OnDestroy()
        {
            if (NetworkManager.Instance.NetworkRunner.IsServer)
            {
                _systems?.Destroy();
                _world?.Destroy();
                _eventBus?.Destroy();
            }
        }
    }
}


