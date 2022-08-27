using Leopotam.EcsLite;
using UnityEngine;

namespace LeoLite.EntityPhysics
{
    public struct OnEntityCollisionExit2DEvent
    {
        public EcsPackedEntity senderEntity;
        public EcsPackedEntity otherEntity;
        public Vector2 relativeVelocity;
    }
}