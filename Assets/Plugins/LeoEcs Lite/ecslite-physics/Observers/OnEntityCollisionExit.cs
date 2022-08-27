using Leopotam.EcsLite;
using UnityEngine;
using Voody.UniLeo.Lite;

namespace LeoLite.EntityPhysics
{
    public class OnEntityCollisionExit: EntityPhysicObserver
    {
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.TryGetComponent(out ConvertToEntity convertOther))
            {
                if (_convertCurrent.TryGetEntity(out int currentEntity, out EcsWorld currentWorld) &&
                    convertOther.TryGetEntity(out int otherEntity, out EcsWorld otherWorld))
                {
                    EntityPhysicHandler.RegisterCollisionExitEvent(
                        EcsEntityExtensions.PackEntity(currentWorld, currentEntity), 
                        EcsEntityExtensions.PackEntity(otherWorld, otherEntity),
                        other.relativeVelocity);
                }
            }
        }
    }
}