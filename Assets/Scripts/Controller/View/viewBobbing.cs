using UnityEngine;

public class viewBobbing : MonoBehaviour
{
    [Header("Properties")]
    public float smooth = 0.3f;
    public float angle = 1;
    private float smoothVelocity;
    private CharacterController controller;

    private PlayerCamera cam;
    void Start()
    {
        cam = GetComponent<PlayerCamera>();
        controller = gameObject.GetComponentInParent<CharacterController>();
    }
    
    void Update()
    {
        float input = Input.GetAxisRaw("Horizontal");
        float targetRoll = input * angle;
        
        if(input < 0 || input > 0)
            cam.roll =  Mathf.SmoothDamp(cam.roll,  -targetRoll, ref smoothVelocity, smooth);
        else
            cam.roll = Mathf.SmoothDamp(cam.roll, 0, ref smoothVelocity, smooth);
    }
}
