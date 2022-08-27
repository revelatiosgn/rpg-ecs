using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPGGame.Model
{
    public struct InputData : INetworkInput
    {
	    public Vector3 MoveDirection;
    }
}
