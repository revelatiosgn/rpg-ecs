namespace Example
{
	using System;
	using UnityEngine;
	using UnityEngine.InputSystem;
	using UnityEngine.XR;
	using Fusion;
	using Fusion.KCC;

	using XRInputDevice  = UnityEngine.XR.InputDevice;
	using XRCommonUsages = UnityEngine.XR.CommonUsages;

	/// <summary>
	/// Tracks player input for fixed and render updates.
	/// </summary>
	[OrderBefore(typeof(NetworkCulling), typeof(PlayerController))]
	public sealed class PlayerInput : NetworkBehaviour, IBeforeUpdate, IBeforeAllTicks, IBeforeTick
	{
		// PUBLIC MEMBERS

		/// <summary>
		/// Holds input for fixed update.
		/// </summary>
		public GameplayInput FixedInput { get { CheckFixedAccess(false); return _fixedInput; } }

		/// <summary>
		/// Holds input for current frame render update.
		/// </summary>
		public GameplayInput RenderInput { get { CheckRenderAccess(false); return _renderInput; } }

		/// <summary>
		/// Holds combined inputs from all render frames since last fixed update. Used when Fusion input poll is triggered.
		/// </summary>
		public GameplayInput CachedInput { get { CheckRenderAccess(false); return _cachedInput; } }

		/// <summary>
		/// Indicates the input is ignored after resuming app to prevent glitches until the simulation is stable.
		/// </summary>
		public bool IsIgnoringInput => _ignoreTime > 0.0f;

		// PRIVATE MEMBERS

		[SerializeField][Tooltip("Mouse delta multiplier.")]
		private Vector2 _standaloneLookSensitivity = Vector2.one;
		[SerializeField][Range(0.0f, 0.1f)][Tooltip("Look rotation delta for a render frame is calculated as average from all frames within responsivity time.")]
		private float   _lookResponsivity = 0.025f;
		[SerializeField][Range(0.0f, 1.0f)][Tooltip("How long the last known input is repeated before using default.")]
		private float   _maxRepeatTime = 0.25f;
		[SerializeField][Range(0.0f, 5.0f)][Tooltip("Ignores input for [X] seconds after resuming app.")]
		private float   _ignoreInputOnPause = 2.0f;
		[SerializeField][Range(0.0f, 5.0f)][Tooltip("Maximum extension of ignore input window if a simulation instability is detected after resuming app.")]
		private float   _maxIgnoreInputExtension = 5.0f;
		[SerializeField][Tooltip("Outputs missing inputs to console.")]
		private bool    _logMissingInputs;

		// We need to store last known input to compare current input against (to track actions activation/deactivation). It is also used if an input for current frame is lost.
		// This is not needed on proxies, only input authority is registered to nameof(PlayerInput) interest group.
		[Networked(nameof(PlayerInput))]
		private GameplayInput _lastKnownInput { get; set; }

		private GameplayInput _fixedInput;
		private GameplayInput _renderInput;
		private GameplayInput _cachedInput;
		private GameplayInput _baseFixedInput;
		private GameplayInput _baseRenderInput;
		private Vector2       _cachedMoveDirection;
		private float         _cachedMoveDirectionSize;
		private bool          _resetCachedInput;
		private FrameRecord[] _frameRecords = new FrameRecord[32];
		private float         _repeatTime;
		private float         _ignoreTime;
		private float         _ignoreExtension;
		private float         _ignoreRenderTime;
		private int           _missingInputsInRow;
		private int           _missingInputsTotal;
		private int           _logMissingInputFromTick;

		// PUBLIC METHODS

		/// <summary>
		/// Check if an action is active in current input. FUN/Render input is resolved automatically.
		/// </summary>
		public bool HasActive(EGameplayInputAction action)
		{
			if (Runner.Stage != default)
			{
				CheckFixedAccess(false);
				return action.IsActive(_fixedInput);
			}
			else
			{
				CheckRenderAccess(false);
				return action.IsActive(_renderInput);
			}
		}

		/// <summary>
		/// Check if an action was activated in current input.
		/// In FUN this method compares current fixed input agains previous fixed input.
		/// In Render this method compares current render input against previous render input OR current fixed input (first Render call after FUN).
		/// </summary>
		public bool WasActivated(EGameplayInputAction action)
		{
			if (Runner.Stage != default)
			{
				CheckFixedAccess(false);
				return action.WasActivated(_fixedInput, _baseFixedInput);
			}
			else
			{
				CheckRenderAccess(false);
				return action.WasActivated(_renderInput, _baseRenderInput);
			}
		}

		/// <summary>
		/// Check if an action was activated in custom input.
		/// In FUN this method compares custom input agains previous fixed input.
		/// In Render this method compares custom input against previous render input OR current fixed input (first Render call after FUN).
		/// </summary>
		public bool WasActivated(EGameplayInputAction action, GameplayInput customInput)
		{
			if (Runner.Stage != default)
			{
				CheckFixedAccess(false);
				return action.WasActivated(customInput, _baseFixedInput);
			}
			else
			{
				CheckRenderAccess(false);
				return action.WasActivated(customInput, _baseRenderInput);
			}
		}

		/// <summary>
		/// Check if an action was deactivated in current input.
		/// In FUN this method compares current fixed input agains previous fixed input.
		/// In Render this method compares current render input against previous render input OR current fixed input (first Render call after FUN).
		/// </summary>
		public bool WasDeactivated(EGameplayInputAction action)
		{
			if (Runner.Stage != default)
			{
				CheckFixedAccess(false);
				return action.WasDeactivated(_fixedInput, _baseFixedInput);
			}
			else
			{
				CheckRenderAccess(false);
				return action.WasDeactivated(_renderInput, _baseRenderInput);
			}
		}

		/// <summary>
		/// Check if an action was deactivated in custom input.
		/// In FUN this method compares custom input agains previous fixed input.
		/// In Render this method compares custom input against previous render input OR current fixed input (first Render call after FUN).
		/// </summary>
		public bool WasDeactivated(EGameplayInputAction action, GameplayInput customInput)
		{
			if (Runner.Stage != default)
			{
				CheckFixedAccess(false);
				return action.WasDeactivated(customInput, _baseFixedInput);
			}
			else
			{
				CheckRenderAccess(false);
				return action.WasDeactivated(customInput, _baseRenderInput);
			}
		}

		/// <summary>
		/// Updates fixed input. Use after manipulating with fixed input outside.
		/// </summary>
		/// <param name="fixedInput">Input used in fixed update.</param>
		/// <param name="updateBaseInputs">Updates base fixed input and base render input.</param>
		public void SetFixedInput(GameplayInput fixedInput, bool updateBaseInputs)
		{
			CheckFixedAccess(true);

			_fixedInput = fixedInput;

			if (updateBaseInputs == true)
			{
				_baseFixedInput  = fixedInput;
				_baseRenderInput = fixedInput;
			}
		}

		/// <summary>
		/// Updates render input. Use after manipulating with render input outside.
		/// </summary>
		/// <param name="renderInput">Input used in render update.</param>
		/// <param name="updateBaseInput">Updates base render input.</param>
		public void SetRenderInput(GameplayInput renderInput, bool updateBaseInput)
		{
			CheckRenderAccess(false);

			_renderInput = renderInput;

			if (updateBaseInput == true)
			{
				_baseRenderInput = renderInput;
			}
		}

		/// <summary>
		/// Updates last known input. Use after manipulating with fixed input outside.
		/// </summary>
		/// <param name="fixedInput">Input used as last known input.</param>
		/// <param name="updateBaseInputs">Updates base fixed input and base render input.</param>
		public void SetLastKnownInput(GameplayInput fixedInput, bool updateBaseInputs)
		{
			CheckFixedAccess(true);

			_lastKnownInput = fixedInput;

			if (updateBaseInputs == true)
			{
				_baseFixedInput  = fixedInput;
				_baseRenderInput = fixedInput;
			}
		}

		// NetworkBehaviour INTERFACE

		public override void Spawned()
		{
			// Reset to default state.
			_fixedInput         = default;
			_renderInput        = default;
			_cachedInput        = default;
			_lastKnownInput     = default;
			_baseFixedInput     = default;
			_baseRenderInput    = default;
			_repeatTime         = default;
			_ignoreTime         = default;
			_ignoreExtension    = default;
			_ignoreRenderTime   = default;
			_missingInputsTotal = default;
			_missingInputsInRow = default;

			// Wait few seconds before the connection is stable to start tracking missing inputs.
			_logMissingInputFromTick = Runner.Simulation.Tick + Runner.Config.Simulation.TickRate * 4;

			if (Object.HasStateAuthority == true)
			{
				// Only state and input authority works with input and access _lastFixedInput.
				Object.SetInterestGroup(Object.InputAuthority, nameof(PlayerInput), true);
			}

			if (Object.HasInputAuthority == true)
			{
				// Register local player input polling.
				NetworkEvents networkEvents = Runner.GetComponent<NetworkEvents>();
				networkEvents.OnInput.RemoveListener(OnInput);
				networkEvents.OnInput.AddListener(OnInput);
			}
		}

		public override void Despawned(NetworkRunner runner, bool hasState)
		{
			_frameRecords.Clear();

			if (runner == null)
				return;

			NetworkEvents networkEvents = runner.GetComponent<NetworkEvents>();
			if (networkEvents != null)
			{
				// Unregister local player input polling.
				networkEvents.OnInput.RemoveListener(OnInput);
			}
		}

		public override void Render()
		{
			if (IsIgnoringInput == true)
			{
				float renderTime = Runner.SimulationRenderTime;
				if (renderTime < _ignoreRenderTime)
				{
					// Current render time is lower than previous render time, still adjusting clock after resuming app...
					TryExtendIgnoreInputWindow($"Negative render delta time ({(renderTime - _ignoreRenderTime).ToString("F3")}s)");
				}

				_ignoreRenderTime = renderTime;
			}
		}

		// IBeforeUpdate INTERFACE

		/// <summary>
		/// 1. Collect input from devices, can be executed multiple times between FixedUpdateNetwork() calls because of faster rendering speed.
		/// </summary>
		void IBeforeUpdate.BeforeUpdate()
		{
			if (Object == null || Object.HasInputAuthority == false)
				return;

			// Store last render input as a base to current render input.
			_baseRenderInput = _renderInput;

			// Reset input for current frame to default.
			_renderInput = default;

			// Cached input was polled and explicit reset requested.
			if (_resetCachedInput == true)
			{
				_resetCachedInput = false;

				_cachedInput             = default;
				_cachedMoveDirection     = default;
				_cachedMoveDirectionSize = default;
			}

			Keyboard keyboard = Keyboard.current;

			// Don't process the input if the ignore time is active.
			if (_ignoreTime > 0.0f)
				return;

			ProcessStandaloneInput();
		}

		void IBeforeAllTicks.BeforeAllTicks(bool resimulation, int tickCount)
		{
			if (resimulation == false && tickCount >= 10 && IsIgnoringInput == true)
			{
				TryExtendIgnoreInputWindow($"Too many forward ticks ({tickCount})");
			}
		}

		/// <summary>
		/// 3. Read input from Fusion. On input authority the FixedInput will match CachedInput.
		/// </summary>
		void IBeforeTick.BeforeTick()
		{
			if (Object == null || (Object.HasInputAuthority == false && Object.HasStateAuthority == false))
				return;

			// Store last known fixed input. This will be compared agaisnt new fixed input.
			_baseFixedInput = _lastKnownInput;

			// Set fixed input to last known fixed input as a fallback.
			_fixedInput = _lastKnownInput;

			// Clear all properties which should not propagate from last known input in case of missing input. As example, following line will reset look rotation delta.
			// This results to the agent not being incorrectly rotated (by using rotation from last known input) in case of missing input on state authority, followed by a correction on the input authority.
			// _fixedInput.LookRotationDelta = default;

			if (Object.InputAuthority != PlayerRef.None)
			{
				// If this fails, fallback (last known) input will be used as current.
				if (Runner.TryGetInputForPlayer(Object.InputAuthority, out GameplayInput input) == true)
				{
					// New input received, we can store it.
					_fixedInput = input;

					// Update last known input. Will be used next tick as base and fallback.
					_lastKnownInput = input;

					if (Runner.Simulation.Stage == SimulationStages.Forward)
					{
						// Reset statistics.
						_missingInputsInRow = 0;

						// Reset threshold for repeating inputs.
						_repeatTime = 0.0f;
					}
				}
				else
				{
					if (_ignoreTime > 0.0f)
					{
						// Don't repeat last known input if the ignore time is active (after app resume).
						_fixedInput = default;
					}
					else
					{
						if (Runner.Simulation.Stage == SimulationStages.Forward)
						{
							// Update statistics.
							++_missingInputsInRow;
							++_missingInputsTotal;

							// Update threshold for repeating inputs.
							_repeatTime += Runner.DeltaTime;

							if (_logMissingInputs == true && Runner.Simulation.Tick >= _logMissingInputFromTick)
							{
								Debug.LogWarning($"Missing input for {Object.InputAuthority} {Runner.Simulation.Tick}. In Row: {_missingInputsInRow} Total: {_missingInputsTotal} Repeating Last Known Input: {_repeatTime <= _maxRepeatTime}", gameObject);
							}
						}

						if (_repeatTime > _maxRepeatTime)
						{
							_fixedInput = default;
						}
					}
				}
			}

			// The current fixed input will be used as a base to first Render after FUN.
			_baseRenderInput = _fixedInput;
		}

		// MonoBehaviour INTERFACE

		private void LateUpdate()
		{
			// Update ignore time, skip if delta is too big (typically after resuming app).
			if (_ignoreTime > 0.0f)
			{
				float deltaTime = Time.unscaledDeltaTime;
				if (deltaTime < 1.0f)
				{
					_ignoreTime -= deltaTime;
				}
			}
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			if (Application.isMobilePlatform == false)
				return;

			ActivateIgnoreInputWindow();
		}

		// PRIVATE METHODS

		private void ActivateIgnoreInputWindow()
		{
			if (Object == null || Object.HasInputAuthority == false)
				return;

			// Reset input on application pause/resume.
			_renderInput = default;
			_cachedInput = default;

			// Set timeout for ignoring input (polling will be ignored during this period).
			_ignoreTime = _ignoreInputOnPause;

			// Reset ignore input window extension.
			_ignoreExtension = _maxIgnoreInputExtension;

			Debug.LogWarning($"Activating {_ignoreTime.ToString("F3")}s input ignore window with {_ignoreExtension.ToString("F3")}s extension.", gameObject);
		}

		private void TryExtendIgnoreInputWindow(string reason)
		{
			float refillTime = 1.0f;

			if (_ignoreExtension <= 0.0f || _ignoreTime <= 0.0f || _ignoreTime > refillTime)
				return;
			if (Object == null || Object.HasInputAuthority == false)
				return;

			// Detected simulation instability after resuming app, we will extend ignore input window to prevent glitches.

			float consumeExtension = refillTime - _ignoreTime;
			if (consumeExtension > _ignoreExtension)
			{
				consumeExtension = _ignoreExtension;
			}

			_ignoreTime      += consumeExtension;
			_ignoreExtension -= consumeExtension;

			if (consumeExtension > 0.0f)
			{
				Debug.LogWarning($"Detected simulation instability after resuming app. Extending ignore input window by {consumeExtension.ToString("F3")}s to {_ignoreTime.ToString("F3")}s with remaining extension {_ignoreExtension.ToString("F3")}s. Reason:{reason}", gameObject);
			}
		}

		/// <summary>
		/// 2. Push cached input and reset properties, can be executed multiple times within single Unity frame if the rendering speed is slower than Fusion simulation (or there is a performance spike).
		/// </summary>
		private void OnInput(NetworkRunner runner, NetworkInput networkInput)
		{
			GameplayInput gameplayInput = _cachedInput;

			// Input is polled for single fixed update, but at this time we don't know how many times in a row OnInput() will be executed.
			// This is the reason for having a reset flag instead of resetting input immediately, otherwise we could lose input for next fixed updates (for example move direction).

			_resetCachedInput = true;

			// Now we reset all properties which should not propagate into next OnInput() call (for example LookRotationDelta - this must be applied only once and reset immediately).
			// If there's a spike, OnInput() and FixedUpdateNetwork() will be called multiple times in a row without BeforeUpdate() in between, so we don't reset move direction to preserve movement.
			// Instead, move direction and other sensitive properties are reset in next BeforeUpdate() - driven by _resetCachedInput.

			// Input consumed by OnInput() call will be read in FixedUpdateNetwork() and immediately propagated to KCC.
			// Here we should reset render properties so they are not applied twice (fixed + render update).

			// Don't set the input if the ignore time is active.
			if (_ignoreTime > 0.0f)
				return;

			networkInput.Set(gameplayInput);
		}

		private void ProcessStandaloneInput()
		{
			// Always use KeyControl.isPressed, Input.GetMouseButton() and Input.GetKey().
			// Never use KeyControl.wasPressedThisFrame, Input.GetMouseButtonDown() or Input.GetKeyDown() otherwise the action might be lost.

			Vector3 moveDirection     = Vector3.zero;

			Mouse mouse = Mouse.current;
			if (mouse != null)
			{
				Vector2 mouseDelta = mouse.delta.ReadValue();

				_renderInput.LMB = mouse.leftButton.isPressed;
				_renderInput.RMB = mouse.rightButton.isPressed;
				_renderInput.MMB = mouse.middleButton.isPressed;

				Ray ray = Camera.main.ScreenPointToRay(mouse.position.ReadValue());
				Plane plane = new Plane(Vector3.up, Vector3.zero);
				if (plane.Raycast(ray, out float enter))
				{
					Vector3 hitPoint = ray.GetPoint(enter);
					_renderInput.LookTarget = new Vector2(hitPoint.x, hitPoint.z);
				}
			}

			Keyboard keyboard = Keyboard.current;
			if (keyboard != null)
			{
				if (keyboard.mKey.isPressed == true && keyboard.leftCtrlKey.isPressed == true && keyboard.leftShiftKey.isPressed == true)
				{
					// Simulate application pause/resume.
					ActivateIgnoreInputWindow();
				}

				if (keyboard.wKey.isPressed == true) { moveDirection += Camera.main.transform.forward; }
				if (keyboard.sKey.isPressed == true) { moveDirection -= Camera.main.transform.forward; }
				if (keyboard.aKey.isPressed == true) { moveDirection -= Camera.main.transform.right; }
				if (keyboard.dKey.isPressed == true) { moveDirection += Camera.main.transform.right; }

				moveDirection.y = 0f;

				if (moveDirection.IsZero() == false)
				{
					moveDirection.Normalize();
				}

				_renderInput.Jump   = keyboard.spaceKey.isPressed;
				_renderInput.Dash   = keyboard.tabKey.isPressed;
				_renderInput.Sprint = keyboard.leftShiftKey.isPressed;

				if (Object.HasInputAuthority == true)
				{
					// Here we can use KeyControl.wasPressedThisFrame / Input.GetKeyDown() because it is not part of input structure and we send actions through RPC.
					if (keyboard.numpadPlusKey.wasPressedThisFrame  == true) { GetComponent<PlayerController>().ToggleSpeedRPC(1);  }
					if (keyboard.numpadMinusKey.wasPressedThisFrame == true) { GetComponent<PlayerController>().ToggleSpeedRPC(-1); }
				}
			}

			_renderInput.MoveDirection     = moveDirection;

			// Process cached input for next OnInput() call, represents accumulated inputs for all render frames since last fixed update.

			float deltaTime = Time.deltaTime;

			// Move direction accumulation is a special case. Let's say simulation runs 30Hz (33.333ms delta time) and render runs 300Hz (3.333ms delta time).
			// If the player hits W key in last frame before fixed update, the KCC will move in render update by (velocity * 0.003333f).
			// Treating this input the same way for next fixed update results in KCC moving by (velocity * 0.03333f) - 10x more.
			// Following accumulation proportionally scales move direction so it reflects frames in which input was active.
			// This way the next fixed update will correspond more accurately to what happened in render frames.

			_cachedMoveDirection     += new Vector2(moveDirection.x, moveDirection.z) * deltaTime;
			_cachedMoveDirectionSize += deltaTime;

			_cachedInput.Actions            = new NetworkButtons(_cachedInput.Actions.Bits | _renderInput.Actions.Bits);
			_cachedInput.MoveDirection      = _cachedMoveDirection / _cachedMoveDirectionSize;

			_cachedInput.LookTarget = _renderInput.LookTarget;
		}

		private Vector2 ProcessLookRotationDelta(Vector2 lookRotationDelta, Vector2 lookRotationSensitivity)
		{
			lookRotationDelta *= lookRotationSensitivity;

			// If the look rotation responsivity is enabled, calculate average delta instead.
			if (_lookResponsivity > 0.0f)
			{
				// Kill any rotation in opposite direction for instant direction flip.
				CleanLookRotationDeltaHistory(lookRotationDelta);

				FrameRecord frameRecord = new FrameRecord(Time.unscaledDeltaTime, lookRotationDelta);

				// Shift history with frame records.
				Array.Copy(_frameRecords, 0, _frameRecords, 1, _frameRecords.Length - 1);

				// Store current frame to history.
				_frameRecords[0] = frameRecord;

				float   accumulatedDeltaTime         = default;
				Vector2 accumulatedLookRotationDelta = default;

				// Iterate over all frame records.
				for (int i = 0; i < _frameRecords.Length; ++i)
				{
					frameRecord = _frameRecords[i];

					// Accumualte delta time and look rotation delta until we pass responsivity threshold.
					accumulatedDeltaTime         += frameRecord.DeltaTime;
					accumulatedLookRotationDelta += frameRecord.LookRotationDelta;

					if (accumulatedDeltaTime > _lookResponsivity)
					{
						// To have exact responsivity time window length, we have to remove delta overshoot from last accumulation.

						float overshootDeltaTime = accumulatedDeltaTime - _lookResponsivity;

						accumulatedDeltaTime         -= overshootDeltaTime;
						accumulatedLookRotationDelta -= overshootDeltaTime * frameRecord.LookRotationDelta;

						break;
					}
				}

				// Normalize acucmulated look rotation delta and calculate size for current frame.
				lookRotationDelta = (accumulatedLookRotationDelta / accumulatedDeltaTime) * Time.unscaledDeltaTime;
			}

			return lookRotationDelta;
		}

		private void CleanLookRotationDeltaHistory(Vector2 lookRotationDelta)
		{
			int count = _frameRecords.Length;

			// Iterate over all records and clear rotation with opposite direction, giving instant responsivity when direction flips.
			// Each axis is processed separately.

			if (lookRotationDelta.x < 0.0f) { for (int i = 0; i < count; ++i) { if (_frameRecords[i].LookRotationDelta.x > 0.0f) { _frameRecords[i].LookRotationDelta.x = 0.0f; } } }
			if (lookRotationDelta.x > 0.0f) { for (int i = 0; i < count; ++i) { if (_frameRecords[i].LookRotationDelta.x < 0.0f) { _frameRecords[i].LookRotationDelta.x = 0.0f; } } }
			if (lookRotationDelta.y < 0.0f) { for (int i = 0; i < count; ++i) { if (_frameRecords[i].LookRotationDelta.y > 0.0f) { _frameRecords[i].LookRotationDelta.y = 0.0f; } } }
			if (lookRotationDelta.y > 0.0f) { for (int i = 0; i < count; ++i) { if (_frameRecords[i].LookRotationDelta.y < 0.0f) { _frameRecords[i].LookRotationDelta.y = 0.0f; } } }
		}

		[System.Diagnostics.Conditional("UNITY_EDITOR")]
		[System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
		private void CheckFixedAccess(bool checkStage)
		{
			if (checkStage == true && Runner.Stage == default)
			{
				throw new InvalidOperationException("This call should be executed from FixedUpdateNetwork!");
			}

			if (Runner.Stage != default && Object.IsProxy == true)
			{
				throw new InvalidOperationException("Fixed input is available only on State & Input authority!");
			}
		}

		[System.Diagnostics.Conditional("UNITY_EDITOR")]
		[System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
		private void CheckRenderAccess(bool checkStage)
		{
			if (checkStage == true && Runner.Stage != default)
			{
				throw new InvalidOperationException("This call should be executed outside of FixedUpdateNetwork!");
			}

			if (Runner.Stage == default && Object.HasInputAuthority == false)
			{
				throw new InvalidOperationException("Render and cached inputs are available only on Input authority!");
			}
		}

		// DATA STRUCTURES

		private struct FrameRecord
		{
			public float   DeltaTime;
			public Vector2 LookRotationDelta;

			public FrameRecord(float deltaTime, Vector2 lookRotationDelta)
			{
				DeltaTime         = deltaTime;
				LookRotationDelta = lookRotationDelta;
			}
		}
	}
}
