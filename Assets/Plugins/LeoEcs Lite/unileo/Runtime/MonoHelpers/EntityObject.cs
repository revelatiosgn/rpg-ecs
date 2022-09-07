using Leopotam.EcsLite;
using UnityEngine;

namespace Voody.UniLeo.Lite
{
    public class EntityObject : MonoBehaviour
    {
        private EcsPackedEntity _packedEntity;
        private EcsWorld _world;

        public int Id { get {
            
            if (_packedEntity.Unpack(_world, out int entity))
                return entity;

            return -1;
        }}

        public void Initialize(EcsPackedEntity packedEntity, EcsWorld world)
        {
            _packedEntity = packedEntity;
            _world = world;
        }
    }
}


