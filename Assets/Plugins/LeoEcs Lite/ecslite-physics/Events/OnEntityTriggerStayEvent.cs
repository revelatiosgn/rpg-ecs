using Leopotam.EcsLite;

namespace LeoLite.EntityPhysics
{
    public struct OnEntityTriggerStayEvent
    {
        public EcsPackedEntity senderEntity;
        public EcsPackedEntity otherEntity;
    }
}