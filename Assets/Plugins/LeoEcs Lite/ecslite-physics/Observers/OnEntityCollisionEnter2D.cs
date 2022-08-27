using Leopotam.EcsLite;
using UnityEngine;
using Voody.UniLeo.Lite;

namespace LeoLite.EntityPhysics
{
    public class OnEntityCollisionEnter2D : EntityPhysicObserver
    {
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.TryGetComponent(out ConvertToEntity convertOther))
            {
                if (_convertCurrent.TryGetEntity(out int currentEntity, out EcsWorld currentWorld) &&
                    convertOther.TryGetEntity(out int otherEntity, out EcsWorld otherWorld))
                {
                    EntityPhysicHandler.RegisterCollisionEnter2DEvent(
                        EcsEntityExtensions.PackEntity(currentWorld, currentEntity), 
                        EcsEntityExtensions.PackEntity(otherWorld, otherEntity),
                        other.GetContact(0), 
                        other.relativeVelocity);
                }
            }
        }
    }
}