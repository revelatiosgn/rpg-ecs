using Leopotam.EcsLite;
using UnityEngine;
using Voody.UniLeo.Lite;

namespace LeoLite.EntityPhysics
{
    public class OnEntityTriggerStay2D : EntityPhysicObserver
    {
        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.TryGetComponent(out ConvertToEntity convertOther))
            {
                if (_convertCurrent.TryGetEntity(out int currentEntity, out EcsWorld currentWorld) &&
                    convertOther.TryGetEntity(out int otherEntity, out EcsWorld otherWorld))
                {
                    EntityPhysicHandler.RegisterTriggerStay2DEvent(
                        EcsEntityExtensions.PackEntity(currentWorld, currentEntity), 
                        EcsEntityExtensions.PackEntity(otherWorld, otherEntity));
                }
            }
        }
    }
}