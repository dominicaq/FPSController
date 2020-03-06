using UnityEngine;

public class SpinningObject : Interactable
{
    public Light lightBulb;
    public float currentSpeed;
    public float maxSpeed = 1.75f;
    
    public float accelerationRate = 0.1f;
    public float decelerationRate = 0.05f;
    public bool isActive = true;
    
    public void Update()
    {
        if (isActive)
            currentSpeed += accelerationRate * Time.deltaTime;
        else
            currentSpeed -= decelerationRate * Time.deltaTime;

        currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);
        transform.Rotate( currentSpeed, 0,  0);
    }

    public override void OnInteract()
    {
        isActive = !isActive;
        lightBulb.color = isActive ? Color.green : Color.red;
    }
}
