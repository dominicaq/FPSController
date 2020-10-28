using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Menu Reader", menuName = "Game/Input/Menu Reader")]
public class MenuInputReader : ScriptableObject, GameInput.IMenuActions
{
    public UnityAction pauseEvent;
    GameInput gameInput;

	private void OnEnable()
	{
		if (gameInput == null)
		{
			gameInput = new GameInput();
			gameInput.Menu.SetCallbacks(this);
		}
		gameInput.Menu.Enable();
	}

    private void OnDisable() 
    {
        gameInput.Menu.Disable();
    }

	public void OnPause(InputAction.CallbackContext context)
	{
		if (pauseEvent != null && context.phase == InputActionPhase.Started)
			pauseEvent.Invoke();
	}
}
