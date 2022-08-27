using UnityEngine;
using Leopotam.EcsLite;

namespace Voody.UniLeo.Lite
{
    public abstract class MonoProvider<T> : BaseMonoProvider, IConvertToEntity where T : struct
    {
        [SerializeField] protected T value;
        
        protected virtual void Initialize()
        {
        }

        void IConvertToEntity.Convert(int entity, EcsWorld world)
        {
            Initialize();
            
            var pool = world.GetPool<T>();
            if (pool.Has(entity))
            {
                pool.Del(entity);
            }

            pool.Add(entity) = value;
        }
    }
}
