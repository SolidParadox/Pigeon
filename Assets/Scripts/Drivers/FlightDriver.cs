using System;
using UnityEngine;
using UnityEngine.InputSystem;

public struct FloatTransfer {
	private float accumulator;
	public void Add(float input) => accumulator += input;
	public void TryReset(int ticker) {
		if (ticker == 0) accumulator = 0f;
	}
	public float GetValue(int ticker) => ticker > 0 ? accumulator / ticker : accumulator;
}
public struct Vector2Transfer {
	private Vector2 accumulator;
	public void Add(Vector2 input) => accumulator += input;
	public void TryReset(int ticker) {
		if (ticker == 0) accumulator = Vector2.zero;
	}
	public Vector2 GetValue(int ticker) => ticker > 0 ? accumulator / ticker : accumulator;
}

public class FlightDriver : MonoBehaviour {
	public FlightCore[] flightCores;
	public enum FcState { Grounded, GroundedBoost, Flight, FlightBoost, DashInterdiction };

	public DodgeCore DodgeCore;

	public InputActionAsset InputActions;
	public Anchor CameraAnchor;

	public TriggerCore groundTrigger;

	private InputAction rightStickAction;
	private InputAction leftStickAction;
	private InputAction verticalAction;
	private InputAction releaseControl;
	private InputAction releaseCamera;
	private InputAction boostAction;

	public bool inControl;
	public bool cameraControl;
	public Vector2 cameraSensitivityMultiplier;

	public Celll Celll;
	public float STRBoostCellDrain;
	public float STRDodgeCost;

	private Vector2Transfer transferLeft;
	private Vector2Transfer transferRight;
	private FloatTransfer transferVertical;
	private FloatTransfer transferBoost;

	private bool boostInterdiction = false;

	private int ticker;

	private float dodgeHack;
	private bool dodgeCatch;
	public float THRDodgeWindow;

	public void SetBoostInterdiction(bool alpha) {
		// We might need to trigger things on change, that's why im using a function
		boostInterdiction = alpha;
	}

	private void OnEnable() {
		inControl = true;

		var map = InputActions.FindActionMap("Player");
		map.Enable();

		verticalAction = map.FindAction("Vertical");
		rightStickAction = map.FindAction("Look");
		leftStickAction = map.FindAction("Move");
		releaseControl = map.FindAction("TEMPReleaseControl");
		boostAction = map.FindAction("Boost");

		Cursor.lockState = inControl ? CursorLockMode.Locked : CursorLockMode.None;

		dodgeHack = 0;
		dodgeCatch = false;
		ticker = 0;
	}

	private void OnDisable() {
		InputActions.FindActionMap("Player").Disable();
	}

	public void TryResetAllTransfers(int _ticker) {
		transferLeft.TryReset(_ticker);
		transferRight.TryReset(_ticker);
		transferVertical.TryReset(_ticker);
		transferBoost.TryReset(_ticker);
	}

	void Update() {
		if (releaseControl.WasPressedThisDynamicUpdate()) {
			inControl = !inControl;
			Cursor.lockState = inControl ? CursorLockMode.Locked : CursorLockMode.None;
		}

		if (!inControl) {
			TryResetAllTransfers(0);
			return;
		}

		dodgeHack -= Time.deltaTime;
		if (dodgeHack < 0) dodgeHack = 0;

		TryResetAllTransfers(ticker);

		Vector2 delta = leftStickAction.ReadValue<Vector2>();
		if (delta.magnitude > 1) delta.Normalize();
		transferLeft.Add(delta);

		// Boost control
		if (boostAction.IsPressed()) {
			bool hasDodged = false;
			if (!dodgeCatch) {
				if (dodgeHack > 0 && !DodgeCore.isDodging && Celll.Drain(STRDodgeCost)) {
					if (delta.magnitude < 0.1f) {
						delta = Vector2.up;
					}
					DodgeCore.RXRelative(new Vector3(delta.x, 0, delta.y));
					hasDodged = true;
				}
				dodgeHack = THRDodgeWindow;
				dodgeCatch = true;
			}
			if (!hasDodged && !DodgeCore.isDodging && Celll.Drain(STRBoostCellDrain * Time.deltaTime)) {
				transferBoost.Add(1);
			}
		}
		else {
			dodgeCatch = false;
		}

		delta = rightStickAction.ReadValue<Vector2>();
		if (delta.magnitude > 1) delta.Normalize();
		transferRight.Add(delta);

		transferVertical.Add(verticalAction.ReadValue<float>());

		ticker++;
	}

	private void LateUpdate() {
		if (inControl) {
			CameraAnchor.angularInterpolationOffset.x -= rightStickAction.ReadValue<Vector2>().y * cameraSensitivityMultiplier.y * Time.deltaTime;
			if (Mathf.Abs(CameraAnchor.angularInterpolationOffset.x) > 90) {
				CameraAnchor.angularInterpolationOffset.x = Mathf.Sign(CameraAnchor.angularInterpolationOffset.x) * 90;
			}
		}
	}

	private void FixedUpdate() {
		Vector2 deltaRight = transferRight.GetValue(ticker);
		Vector2 deltaLeft = transferLeft.GetValue(ticker);
		float deltaVertical = transferVertical.GetValue(ticker);

		FcState selector = FcState.Flight;
		if (groundTrigger.breached) {
			selector = FcState.Grounded;
		}
		if (transferBoost.GetValue(ticker) > 0) {
			selector++;
		}

		for (int i = 0; i < 4; i++) {
			flightCores[i].enabled = i == (int)selector;
		}

		if (selector != FcState.DashInterdiction) {
			flightCores[(int)selector].RX(new Vector3(deltaLeft.x, deltaRight.x, deltaLeft.y), deltaVertical);
		}
		if (selector == FcState.GroundedBoost || selector == FcState.FlightBoost) {
			flightCores[(int)selector].Boost();
		}

		ticker = 0;
	}
}