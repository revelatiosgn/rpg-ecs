using Leopotam.EcsLite;

namespace LeoLite.EntityPhysics
{
    public struct OnEntityTriggerEnter2DEvent
    {
        public EcsPackedEntity senderEntity;
        public EcsPackedEntity otherEntity;
    }
}