using Leopotam.EcsLite;
using UnityEngine;

namespace LeoLite.EntityPhysics
{
    public static class EntityPhysicHandler
    {
        public static EcsWorld ecsWorld;

        public static void RegisterTriggerEnterEvent(EcsPackedEntity senderEntity, EcsPackedEntity otherEntity)
        {
            var eventEntity = ecsWorld.NewEntity();
            var pool = ecsWorld.GetPool<OnEntityTriggerEnterEvent>();
            pool.Add(eventEntity);
            ref var eventComponent = ref pool.Get(eventEntity);
            eventComponent.senderEntity = senderEntity;
            eventComponent.otherEntity = otherEntity;
        }

        public static void RegisterTriggerStayEvent(EcsPackedEntity senderEntity, EcsPackedEntity otherEntity)
        {
            var eventEntity = ecsWorld.NewEntity();
            var pool = ecsWorld.GetPool<OnEntityTriggerStayEvent>();
            pool.Add(eventEntity);
            ref var eventComponent = ref pool.Get(eventEntity);
            eventComponent.senderEntity = senderEntity;
            eventComponent.otherEntity = otherEntity;
        }

        public static void RegisterTriggerExitEvent(EcsPackedEntity senderEntity, EcsPackedEntity otherEntity)
        {
            var eventEntity = ecsWorld.NewEntity();
            var pool = ecsWorld.GetPool<OnEntityTriggerExitEvent>();
            pool.Add(eventEntity);
            ref var eventComponent = ref pool.Get(eventEntity);
            eventComponent.senderEntity = senderEntity;
            eventComponent.otherEntity = otherEntity;
        }

        public static void RegisterCollisionEnterEvent(EcsPackedEntity senderEntity, EcsPackedEntity otherEntity, ContactPoint firstContactPoint, Vector3 relativeVelocity)
        {
            var eventEntity = ecsWorld.NewEntity();
            var pool = ecsWorld.GetPool<OnEntityCollisionEnterEvent>();
            pool.Add(eventEntity);
            ref var eventComponent = ref pool.Get(eventEntity);
            eventComponent.senderEntity = senderEntity;
            eventComponent.otherEntity = otherEntity;
            eventComponent.firstContactPoint = firstContactPoint;
            eventComponent.relativeVelocity = relativeVelocity;
        }

        public static void RegisterCollisionStayEvent(EcsPackedEntity senderEntity, EcsPackedEntity otherEntity, ContactPoint firstContactPoint, Vector3 relativeVelocity)
        {
            var eventEntity = ecsWorld.NewEntity();
            var pool = ecsWorld.GetPool<OnEntityCollisionStayEvent>();
            pool.Add(eventEntity);
            ref var eventComponent = ref pool.Get(eventEntity);
            eventComponent.senderEntity = senderEntity;
            eventComponent.otherEntity = otherEntity;
            eventComponent.firstContactPoint = firstContactPoint;
            eventComponent.relativeVelocity = relativeVelocity;
        }

        public static void RegisterCollisionExitEvent(EcsPackedEntity senderEntity, EcsPackedEntity otherEntity, Vector3 relativeVelocity)
        {
            var eventEntity = ecsWorld.NewEntity();
            var pool = ecsWorld.GetPool<OnEntityCollisionExitEvent>();
            pool.Add(eventEntity);
            ref var eventComponent = ref pool.Get(eventEntity);
            eventComponent.senderEntity = senderEntity;
            eventComponent.otherEntity = otherEntity;
            eventComponent.relativeVelocity = relativeVelocity;
        }

        public static void RegisterControllerColliderHitEvent(EcsPackedEntity senderEntity, EcsPackedEntity otherEntity, Vector3 hitNormal, Vector3 moveDirection)
        {
            var eventEntity = ecsWorld.NewEntity();
            var pool = ecsWorld.GetPool<OnEntityControllerColliderHitEvent>();
            pool.Add(eventEntity);
            ref var eventComponent = ref pool.Get(eventEntity);
            eventComponent.senderEntity = senderEntity;
            eventComponent.otherEntity = otherEntity;
            eventComponent.hitNormal = hitNormal;
            eventComponent.moveDirection = moveDirection;
        }
        
        public static void RegisterCollisionEnter2DEvent(EcsPackedEntity senderEntity, EcsPackedEntity otherEntity, ContactPoint2D firstContactPoint, Vector2 relativeVelocity)
        {
            var eventEntity = ecsWorld.NewEntity();
            var pool = ecsWorld.GetPool<OnEntityCollisionEnter2DEvent>();
            pool.Add(eventEntity);
            ref var eventComponent = ref pool.Get(eventEntity);
            eventComponent.senderEntity = senderEntity;
            eventComponent.otherEntity = otherEntity;
            eventComponent.firstContactPoint = firstContactPoint;
            eventComponent.relativeVelocity = relativeVelocity;
        }

        public static void RegisterCollisionStay2DEvent(EcsPackedEntity senderEntity, EcsPackedEntity otherEntity, ContactPoint2D firstContactPoint, Vector2 relativeVelocity)
        {
            var eventEntity = ecsWorld.NewEntity();
            var pool = ecsWorld.GetPool<OnEntityCollisionStay2DEvent>();
            pool.Add(eventEntity);
            ref var eventComponent = ref pool.Get(eventEntity);
            eventComponent.senderEntity = senderEntity;
            eventComponent.otherEntity = otherEntity;
            eventComponent.firstContactPoint = firstContactPoint;
            eventComponent.relativeVelocity = relativeVelocity;
        }

        public static void RegisterCollisionExit2DEvent(EcsPackedEntity senderEntity, EcsPackedEntity otherEntity, Vector2 relativeVelocity)
        {
            var eventEntity = ecsWorld.NewEntity();
            var pool = ecsWorld.GetPool<OnEntityCollisionExit2DEvent>();
            pool.Add(eventEntity);
            ref var eventComponent = ref pool.Get(eventEntity);
            eventComponent.senderEntity = senderEntity;
            eventComponent.otherEntity = otherEntity;
            eventComponent.relativeVelocity = relativeVelocity;
        }
        
        public static void RegisterTriggerEnter2DEvent(EcsPackedEntity senderEntity, EcsPackedEntity otherEntity)
        {
            var eventEntity = ecsWorld.NewEntity();
            var pool = ecsWorld.GetPool<OnEntityTriggerEnter2DEvent>();
            pool.Add(eventEntity);
            ref var eventComponent = ref pool.Get(eventEntity);
            eventComponent.senderEntity = senderEntity;
            eventComponent.otherEntity = otherEntity;
        }

        public static void RegisterTriggerStay2DEvent(EcsPackedEntity senderEntity, EcsPackedEntity otherEntity)
        {
            var eventEntity = ecsWorld.NewEntity();
            var pool = ecsWorld.GetPool<OnEntityTriggerStay2DEvent>();
            pool.Add(eventEntity);
            ref var eventComponent = ref pool.Get(eventEntity);
            eventComponent.senderEntity = senderEntity;
            eventComponent.otherEntity = otherEntity;
        }

        public static void RegisterTriggerExit2DEvent(EcsPackedEntity senderEntity, EcsPackedEntity otherEntity)
        {
            var eventEntity = ecsWorld.NewEntity();
            var pool = ecsWorld.GetPool<OnEntityTriggerExit2DEvent>();
            pool.Add(eventEntity);
            ref var eventComponent = ref pool.Get(eventEntity);
            eventComponent.senderEntity = senderEntity;
            eventComponent.otherEntity = otherEntity;
        }
    }
}