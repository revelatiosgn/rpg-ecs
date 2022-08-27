using Leopotam.EcsLite;

namespace LeoLite.EntityPhysics
{
    public struct OnEntityTriggerExitEvent
    {
        public EcsPackedEntity senderEntity;
        public EcsPackedEntity otherEntity;
    }
}