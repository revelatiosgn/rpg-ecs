using System;
using System.Collections;
using System.Collections.Generic;

namespace RPGGame.Gameplay.Ecs
{
    struct TestEvent
    {
        public int ival;
        public string message;

        public override string ToString()
        {
            return $"Event: {ival} {message}";
        }
    }
}


