using Leopotam.EcsLite;
using UnityEngine;

namespace LeoLite.EntityPhysics
{
    public struct OnEntityCollisionExitEvent
    {
        public EcsPackedEntity senderEntity;
        public EcsPackedEntity otherEntity;
        public Vector3 relativeVelocity;
    }
}