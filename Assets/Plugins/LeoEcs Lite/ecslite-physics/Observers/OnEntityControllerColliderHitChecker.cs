using Leopotam.EcsLite;
using UnityEngine;
using Voody.UniLeo.Lite;

namespace LeoLite.EntityPhysics
{
    public class OnEntityControllerColliderHitChecker : EntityPhysicObserver
    {
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.gameObject.TryGetComponent(out ConvertToEntity convertOther))
            {
                if (_convertCurrent.TryGetEntity(out int currentEntity, out EcsWorld currentWorld) &&
                    convertOther.TryGetEntity(out int otherEntity, out EcsWorld otherWorld))
                {
                    EntityPhysicHandler.RegisterControllerColliderHitEvent(
                        EcsEntityExtensions.PackEntity(currentWorld, currentEntity), 
                        EcsEntityExtensions.PackEntity(otherWorld, otherEntity),
                        hit.normal,
                        hit.moveDirection);
                }
            }
        }
    }
}