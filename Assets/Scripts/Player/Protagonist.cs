using UnityEngine;

public class Protagonist : MonoBehaviour
{
	public InputReader inputReader;
	public PlayerStateManager stateManager;
	[System.NonSerialized] public CameraController cameraController;

	// Temporary ----
	private Telekinesis telekinesis;
	private AbilityManager abilityManager;
	// Temporary ----
	
	public Transform cameraTransform;

    public bool controlsEnabled = true;
    public bool crouchEnabled = true;
    

    private void Awake() 
    {
		stateManager = GetComponent<PlayerStateManager>();
		abilityManager = GetComponent<AbilityManager>();
		
		// Section this off later
		telekinesis = GetComponent<Telekinesis>();
        //

		cameraTransform = transform.GetChild(0).GetChild(0);
		cameraController = cameraTransform.GetComponent<CameraController>();
    }

	private void OnEnable()
	{
		inputReader.moveEvent += OnMove;
		inputReader.cameraMoveEvent += OnMoveCamera;
		inputReader.jumpEvent += OnJump;
		inputReader.crouchEvent += OnCrouch;
		
		inputReader.useEvent += OnUse;
		//...
	}

    private void OnDisable()
	{
		inputReader.moveEvent -= OnMove;
		inputReader.cameraMoveEvent -= OnMoveCamera;
		inputReader.jumpEvent -= OnJump;
		inputReader.crouchEvent -= OnCrouch;
		
		inputReader.useEvent -= OnUse;
		//...
	}

    private void OnUse()
    {
		if (controlsEnabled)
		{
			//abilityManager.currentActiveAbility.Use();
			telekinesis.Use();
		}
    }

	private void OnCrouch()
	{
		if(controlsEnabled && crouchEnabled)
			stateManager.currentController.Crouch(inputReader.enableCrouchToggle);
	}

    private void OnMove(Vector2 movement)
	{
		if (controlsEnabled)
			stateManager.currentController.Move(movement);
	}

	private void OnMoveCamera(Vector2 movement)
	{
		if (controlsEnabled)
			cameraController.Move(movement);
	}

    private void OnJump()
	{
		if (controlsEnabled)
			stateManager.currentController.Jump();
	}
}
