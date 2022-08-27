using Leopotam.EcsLite;
using UnityEngine;
using Voody.UniLeo.Lite;

namespace LeoLite.EntityPhysics
{
    public class OnEntityTriggerExit : EntityPhysicObserver
    {
        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.TryGetComponent(out ConvertToEntity convertOther))
            {
                if (_convertCurrent.TryGetEntity(out int currentEntity, out EcsWorld currentWorld) &&
                    convertOther.TryGetEntity(out int otherEntity, out EcsWorld otherWorld))
                {
                    EntityPhysicHandler.RegisterTriggerExitEvent(
                        EcsEntityExtensions.PackEntity(currentWorld, currentEntity), 
                        EcsEntityExtensions.PackEntity(otherWorld, otherEntity));
                }
            }
        }
    }
}