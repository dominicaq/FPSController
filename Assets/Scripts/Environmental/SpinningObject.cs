using Managers;
using UnityEngine;

public class SpinningObject : EnvironmentalHandyMan
{
    public float speed = 1.0f;
    [Range(0.001f, 1)] public float acceleration = 1.0f;
    public bool active = true;
    private float currentSpeed;
    void Update()
    {
        if(!GameState.isPaused)
            SpinTransform();
    }

    public void SpinTransform()
    {
        currentSpeed = Mathf.Clamp(currentSpeed, 0, speed);

        if (active)
            currentSpeed += acceleration;
        else
            currentSpeed -= acceleration;
        
        // Time is already applied within function
        transform.Rotate( currentSpeed, 0,  0);
    }

    public override void OnActivation()
    {
        active = !active;
    }
}
