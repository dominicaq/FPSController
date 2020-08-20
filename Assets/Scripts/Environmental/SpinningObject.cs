using UnityEngine;

public class SpinningObject : MonoBehaviour, IListener
{
    public bool isActive = true;
    public Light lightBulb;

    [Header("Speed Properties")]
    public float currentSpeed;
    public float maxSpeed = 200f;
    public float accelerationRate = 20f;
    public float decelerationRate = 20f;
    
    private void Start() 
    {
        if(isActive)
            OnActivate();
        else
            OnDeactivate();    
    }

    public void Update()
    {
        if (isActive)
            currentSpeed += accelerationRate * Time.deltaTime;
        else
            currentSpeed -= decelerationRate * Time.deltaTime;

        currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);
        transform.Rotate(Vector3.right * currentSpeed * Time.deltaTime);
    }

    public void OnActivate()
    {
        isActive = true;
        lightBulb.color = Color.green;
    }

    public void OnDeactivate()
    {
        isActive = false;
        lightBulb.color = Color.red;
    }
}
