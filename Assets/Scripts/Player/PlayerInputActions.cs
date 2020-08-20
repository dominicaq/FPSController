using UnityEngine;

public class PlayerInputActions : MonoBehaviour
{
    public InputStruct input;
    private static PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void Update()
    {
        input = new InputStruct()
        {
            MovePosition = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")),
            LookPosition  = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")),

            Fire = Input.GetButton("Fire"),
            Interact = Input.GetButtonDown("Interact"),

            Jump = Input.GetButtonDown("Jump"),
            Swim = Input.GetButton("Jump"),
            Charge = Input.GetKey(KeyCode.LeftShift),

            Crouch = Input.GetButtonDown("Crouch"),
            UnCrouch = Input.GetButtonUp("Crouch")
        };
    }
}

public struct InputStruct
{
    public Vector3 MovePosition;
    public Vector2 LookPosition;

    public bool Fire;
    public bool Interact;
    public bool Crouch;
    public bool UnCrouch;

    public bool Jump;
    public bool Swim;
    public bool Charge;
}


