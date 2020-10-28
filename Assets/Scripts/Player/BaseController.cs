using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class BaseController : MonoBehaviour
{
    #region Components
    [System.NonSerialized] public Transform cameraTransform;
    [System.NonSerialized] public CharacterController characterController;

    #endregion

    [Header("Current")]
    public float currentGravity;
    public Vector3 inputVector;
    public Vector3 velocity;

    [Header("Properties")]
    public float movementSpeed = 7.0f;

    private void Awake() 
    {
        Init();    
    }

    public virtual void Init() 
    {
        characterController = GetComponent<CharacterController>();
        cameraTransform     = transform.GetChild(0).GetChild(0);
    }

    /* -- State Machine -- */
    public virtual void Tick(){}
    public virtual void OnEnter(){}
    public virtual void OnExit()
    {
        currentGravity = 0;
        inputVector = Vector3.zero;
        velocity = Vector3.zero;
    }

    /* -- Controller based input -- */
    public void Move(Vector3 input)
    {
        if (input.sqrMagnitude > 1)
            input = input.normalized;

        inputVector = new Vector3(input.x, 0, input.y);
    }

    public virtual void Jump(){}
    public virtual void Crouch(bool toggle){}


    /* -- Helper functions --- */
    public bool ObjectAbove(float rayLength = 0.6f)
    {
        return Physics.SphereCast(transform.position, characterController.radius, Vector3.up, out RaycastHit hitInfo, rayLength);
    }

    public IEnumerator AdjustMovementSpeed(float newSpeed, float rate = 0)
    {
        float elapsedTime = 0.0f;
        
        if(rate == 0) {
            movementSpeed = newSpeed;
            yield return null;
        }

        while (elapsedTime < 1f) {
            elapsedTime += Time.deltaTime * rate;
            movementSpeed = Mathf.Lerp(movementSpeed, newSpeed, elapsedTime);

            yield return null;
        }
    }
}
