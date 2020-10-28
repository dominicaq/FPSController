using UnityEngine;

public class Protagonist : MonoBehaviour
{
	public InputReader inputReader;
	private PlayerStateManager stateManager;
	public BaseController activeController;
	private CameraController cameraController;

	// Temporary ----
	private Telekinesis telekinesis;
	private PlayerInteraction worldInteract;
	// Temporary ----
	
	private Transform m_CamTransform;

    public bool controlsEnabled = true;
    public bool crouchEnabled = true;
    

    private void Awake() 
    {
		stateManager = GetComponent<PlayerStateManager>();

		// Section this off later
		telekinesis = GetComponent<Telekinesis>();
		worldInteract = GetComponent<PlayerInteraction>();
        //

		m_CamTransform = transform.GetChild(0).GetChild(0);
		cameraController = m_CamTransform.GetComponent<CameraController>();
    }

	private void OnEnable()
	{
		inputReader.moveEvent += OnMove;
		inputReader.jumpEvent += OnJump;
		inputReader.cameraMoveEvent += OnMoveCamera;

		inputReader.crouchEvent += OnCrouch;
		
		inputReader.useEvent += OnUse;
		//...
	}

    private void OnDisable()
	{
		inputReader.moveEvent -= OnMove;
		inputReader.jumpEvent -= OnJump;
		inputReader.cameraMoveEvent -= OnMoveCamera;

		inputReader.crouchEvent -= OnCrouch;
		
		inputReader.useEvent -= OnUse;
		//...
	}

    private void OnUse()
    {
		if (controlsEnabled)
		{
			worldInteract.Use();
			telekinesis.Use();
		}
    }

	private void OnCrouch()
	{
		if(controlsEnabled && crouchEnabled)
			activeController.Crouch(inputReader.enableCrouchToggle);
	}

    private void OnMove(Vector2 movement)
	{
		if (controlsEnabled)
			activeController.Move(movement);
	}

	private void OnMoveCamera(Vector2 movement)
	{
		if (controlsEnabled)
			cameraController.Move(movement);
	}

    private void OnJump()
	{
		if (controlsEnabled)
			activeController.Jump();
	}
}
