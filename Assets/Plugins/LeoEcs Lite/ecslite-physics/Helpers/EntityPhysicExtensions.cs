using Leopotam.EcsLite;
using Leopotam.EcsLite.ExtendedSystems;

namespace LeoLite.EntityPhysics
{
    public static class EntityPhysicExtensions
    {
        public static EcsSystems DelHerePhysics(this EcsSystems ecsSystems, string worldName = null)
        {
            ecsSystems.DelHere<OnEntityTriggerEnterEvent>(worldName);
            ecsSystems.DelHere<OnEntityTriggerStayEvent>(worldName);
            ecsSystems.DelHere<OnEntityTriggerExitEvent>(worldName);
            ecsSystems.DelHere<OnEntityCollisionEnterEvent>(worldName);
            ecsSystems.DelHere<OnEntityCollisionStayEvent>(worldName);
            ecsSystems.DelHere<OnEntityCollisionExitEvent>(worldName);
            ecsSystems.DelHere<OnEntityControllerColliderHitEvent>(worldName);
            ecsSystems.DelHere<OnEntityTriggerEnter2DEvent>(worldName);
            ecsSystems.DelHere<OnEntityTriggerStay2DEvent>(worldName);
            ecsSystems.DelHere<OnEntityTriggerExit2DEvent>(worldName);
            ecsSystems.DelHere<OnEntityCollisionEnter2DEvent>(worldName);
            ecsSystems.DelHere<OnEntityCollisionStay2DEvent>(worldName);
            ecsSystems.DelHere<OnEntityCollisionExit2DEvent>(worldName);
            
            return ecsSystems;
        }
    }
}