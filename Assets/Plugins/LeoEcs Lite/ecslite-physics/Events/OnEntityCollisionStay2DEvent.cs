using Leopotam.EcsLite;
using UnityEngine;

namespace LeoLite.EntityPhysics
{
    public struct OnEntityCollisionStay2DEvent
    {
        public EcsPackedEntity senderEntity;
        public EcsPackedEntity otherEntity;
        public ContactPoint2D firstContactPoint;
        public Vector2 relativeVelocity;
    }
}