using Leopotam.EcsLite;

namespace LeoLite.EntityPhysics
{
    public struct OnEntityTriggerEnterEvent
    {
        public EcsPackedEntity senderEntity;
        public EcsPackedEntity otherEntity;
    }
}