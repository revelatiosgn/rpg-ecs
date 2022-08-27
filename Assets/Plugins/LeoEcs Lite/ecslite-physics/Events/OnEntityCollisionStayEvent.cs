using Leopotam.EcsLite;
using UnityEngine;

namespace LeoLite.EntityPhysics
{
    public struct OnEntityCollisionStayEvent
    {
        public EcsPackedEntity senderEntity;
        public EcsPackedEntity otherEntity;
        public ContactPoint firstContactPoint;
        public Vector3 relativeVelocity;
    }
}