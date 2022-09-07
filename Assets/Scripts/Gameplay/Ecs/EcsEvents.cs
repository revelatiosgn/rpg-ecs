using System;
using System.Collections;
using System.Collections.Generic;

namespace RPGGame.Gameplay.Ecs
{
    public struct OnPlayerInteract : IEcsEvent
    {
        public int SourceEntity;
        public int TargetEntity;
    }

    public struct OnPlayerInterruptInteract : IEcsEvent
    {
        public int SourceEntity;
        public int TargetEntity;
    }

    public struct OnInteractionBegin : IEcsEvent
    {
        public int SourceEntity;
        public int TargetEntity;
    }

    public struct OnInteractionEnd : IEcsEvent
    {
        public int SourceEntity;
        public int TargetEntity;
    }
}


