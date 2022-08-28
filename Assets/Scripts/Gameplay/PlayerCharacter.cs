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
        public KCC KCC => _kcc;

        [SerializeField] private Animator _animator;

        [SerializeField][Range(0f, 100f)] private float _speed = 5f;
        public float Speed => _speed;

	    [SerializeField][Range(0f, 100f)] private float _lookTurnRate = 1.5f;
        public float LookTurnRate => _lookTurnRate;

        private static PlayerCharacter _local;
        public static PlayerCharacter Local => _local;

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                _local = this;
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out InputData inputData))
            {
                Vector3 direction = inputData.MoveDirection;

                direction.y = 0f;
                direction.Normalize();

                _kcc.SetInputDirection(direction);
                _kcc.SetKinematicVelocity(direction * _speed);

                if (direction.sqrMagnitude > 0f)
                {
                    Quaternion targetQ = Quaternion.AngleAxis(Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg - 90, Vector3.down);
                    _kcc.SetLookRotation(Quaternion.RotateTowards(transform.rotation, targetQ, _lookTurnRate * 360 * Runner.DeltaTime));
                }
            }

            _animator.SetFloat("MoveSpeed", _kcc.RenderData.RealSpeed);
        }
    }
}

