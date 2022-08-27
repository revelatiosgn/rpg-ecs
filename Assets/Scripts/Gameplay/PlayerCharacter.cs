using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.KCC;
using RPGGame.Model;
using UnityEngine;

namespace RPGGame.Gameplay
{
    public class PlayerCharacter : NetworkBehaviour
    {
        [SerializeField] private KCC _kcc;
        [SerializeField][Range(0f, 100f)] private float _speed = 5f;

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out InputData inputData))
            {
                Vector3 direction = inputData.MoveDirection;

                direction.y = 0f;
                direction.Normalize();

                _kcc.SetInputDirection(direction);
                _kcc.SetKinematicVelocity(direction * _speed);
            }
        }
    }
}

