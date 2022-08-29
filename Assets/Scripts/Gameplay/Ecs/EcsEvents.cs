using System;
using System.Collections;
using System.Collections.Generic;

namespace RPGGame.Gameplay.Ecs
{
    public struct OnInteractBegin : IEcsEvent
    {
        public int InteractorEntity;
        public int InteractableEntity;
    }

    public struct OnInteractEnd : IEcsEvent
    {
        public int InteractorEntity;
        public int InteractableEntity;
    }
}


