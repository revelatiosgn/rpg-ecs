namespace Example
{
	using System;
	using UnityEngine;
	using Fusion;
	using Fusion.KCC;

	/// <summary>
	/// Advanced player implementation with third person view.
	/// </summary>
	[OrderBefore(typeof(KCC))]
	public sealed class ThirdPersonPlayerController : PlayerController
	{
        [SerializeField] private Animator _animator;

		public RotationMode Rotation;

		public override sealed void FixedUpdateNetwork()
		{
			base.FixedUpdateNetwork();

			if (!HasInputAuthority && !HasStateAuthority)
				return;

			// Skip processing for culled players
			if (Culling.IsCulled == true)
				return;

			// For following lines, we should use Input.FixedInput only. This property holds input for fixed updates.

			// Clamp input look rotation delta
			Vector2 lookRotation      = KCC.FixedData.GetLookRotation(true, true);

			// Calculate input direction based on recently updated look rotation (the change propagates internally also to KCCData.TransformRotation)
			// Vector3 inputDirection = KCC.FixedData.TransformRotation * new Vector3(Input.FixedInput.MoveDirection.x, 0.0f, Input.FixedInput.MoveDirection.y);
			Vector3 inputDirection = new Vector3(Input.FixedInput.MoveDirection.x, 0.0f, Input.FixedInput.MoveDirection.y);

			KCC.SetInputDirection(inputDirection);

			if (Rotation == RotationMode.RotateTowardsMove)
			{
				if (inputDirection != Vector3.zero)
				{
					Quaternion targetQ = Quaternion.AngleAxis(Mathf.Atan2(inputDirection.z, inputDirection.x) * Mathf.Rad2Deg - 90, Vector3.down);
					KCC.SetLookRotation(Quaternion.RotateTowards(KCC.Data.LookRotation, targetQ, LookTurnRate * 360 * Runner.DeltaTime));
				}
			}
			else
			{
				Vector2 lookDirection = Input.FixedInput.LookTarget - new Vector2(transform.position.x, transform.position.z);
				if (lookDirection != Vector2.zero)
				{
					Quaternion targetQ = Quaternion.AngleAxis(Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90, Vector3.down);
					KCC.SetLookRotation(targetQ);
				}
			}

			if (Input.WasActivated(EGameplayInputAction.Jump) == true)
			{
				// By default the character jumps forward in facing direction
				Quaternion jumpRotation = KCC.FixedData.TransformRotation;

				// If we are moving, jump in that direction instead
				if (inputDirection.IsAlmostZero() == false)
				{
					jumpRotation = Quaternion.LookRotation(inputDirection);
				}

				// Applying jump impulse
				KCC.Jump(jumpRotation * JumpImpulse);
			}

			if (KCC.FixedData.IsGrounded == true)
			{
				// Sprint is updated only when grounded
				KCC.SetSprint(Input.FixedInput.Sprint);
			}

			if (Input.WasActivated(EGameplayInputAction.Dash) == true)
			{
				// We only care about registering processor to the KCC, responsibility for cleanup is on dash processor.
				KCC.AddModifier(DashProcessor);
			}

			if (Input.WasActivated(EGameplayInputAction.LMB) == true)
			{
				// Left mouse button action
			}

			if (Input.WasActivated(EGameplayInputAction.RMB) == true)
			{
				// Right mouse button action
			}

			if (Input.WasActivated(EGameplayInputAction.MMB) == true)
			{
				// Middle mouse button action
			}

			// Additional input processing goes here
		}

		public override sealed void Render()
		{
			base.Render();

			if (Culling.IsCulled == true)
				return;

			// For following lines, we should use Input.RenderInput and Input.CachedInput only. These properties hold input for render updates.
			// Input.RenderInput holds input for current render frame.
			// Input.CachedInput holds combined input for all render frames from last fixed update. This property will be used to set input for next fixed update (polled by Fusion).

			// Look rotation have to be updated to get smooth camera rotation

			// At his point, KCC haven't been updated yet (except look rotation, which propagates immediately).
			// Camera have to be synced later (LateUpdate in this case) or we have to update KCC manually (this approach is used in AdvancedPlayer).
		}

		public enum RotationMode
		{
			RotateTowardsMove,
			RotateTowardsCursor
		}
	}
}
