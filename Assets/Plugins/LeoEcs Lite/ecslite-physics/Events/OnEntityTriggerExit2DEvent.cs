using Leopotam.EcsLite;

namespace LeoLite.EntityPhysics
{
    public struct OnEntityTriggerExit2DEvent
    {
        public EcsPackedEntity senderEntity;
        public EcsPackedEntity otherEntity;
    }
}