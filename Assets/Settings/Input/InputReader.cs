using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Input Reader", menuName = "Game/Input/Input Reader")]
public class InputReader : ScriptableObject, GameInput.IGameplayActions
{
	public bool enableCrouchToggle;
    public UnityAction<Vector2> moveEvent;
	public UnityAction<Vector2> cameraMoveEvent;

	public UnityAction crouchEvent;
	public UnityAction jumpEvent;
	public UnityAction useEvent;

    GameInput gameInput;

	private void OnEnable()
	{
		if (gameInput == null)
		{
			gameInput = new GameInput();
			gameInput.Gameplay.SetCallbacks(this);
		}
		gameInput.Gameplay.Enable();
	}

	private void OnDisable()
	{
		gameInput.Gameplay.Disable();
	}

	public void OnUse(InputAction.CallbackContext context)
	{
		if (useEvent != null && context.phase == InputActionPhase.Started)
			useEvent.Invoke();
	}

    public void OnCrouch(InputAction.CallbackContext context)
	{
		if(crouchEvent == null)
			return;

		if(enableCrouchToggle)
		{
			if (context.phase == InputActionPhase.Started)
				crouchEvent.Invoke();
		}
		else
		{
			if (context.phase == InputActionPhase.Started)
				crouchEvent.Invoke();
			else if (context.phase == InputActionPhase.Canceled)
				crouchEvent.Invoke();
		}
	}
	
    public void OnJump(InputAction.CallbackContext context)
	{
		if(jumpEvent == null)
			return;

		if (context.phase == InputActionPhase.Started)
			jumpEvent.Invoke();
	}

    public void OnMovement(InputAction.CallbackContext context)
	{
		if (moveEvent != null)
			moveEvent.Invoke(context.ReadValue<Vector2>());
	}

    public void OnMoveCamera(InputAction.CallbackContext context)
	{
		if (cameraMoveEvent != null)
			cameraMoveEvent.Invoke(context.ReadValue<Vector2>());
	}
}

