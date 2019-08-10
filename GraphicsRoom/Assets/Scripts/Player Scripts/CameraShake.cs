using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Camera Wobble")]
    public float zShake;
    [SerializeField] private bool enableCameraShake = true;
    [SerializeField] private float recoverySpeed = 1.5f;
    private float trauma = 0;
    private float maxValue = 0;

    [Header("Camera Aim Punch")]
    public float yPunch;
    [SerializeField] private bool enableAimPunch = true;
    [SerializeField] private float punchRecoverySpeedy = 1.5f;
    private float maxPunch = 0;
    private float punchTrauma;

    // Update is called once per frame
    void Update()
    {
        if(enableCameraShake)
            ShakeCamera();
        
        if(enableAimPunch)
            AimPunch();
    }

    // Z Shake
    private void ShakeCamera()
    {
        float shake = Mathf.Pow(trauma, 2);

        zShake = maxValue * (Mathf.PerlinNoise(Random.value, Time.fixedDeltaTime)) * shake;
        trauma = Mathf.Clamp01(trauma - recoverySpeed * Time.fixedDeltaTime);
    }

    public void InduceStress(float stress)
    {
        maxValue = Mathf.Pow(stress, 1.5f);
	    trauma = Mathf.Clamp01(trauma + stress);
    }

    // Y Punch
    private void AimPunch()
    {
        float shake = Mathf.Pow(punchTrauma, 2);

        yPunch = maxPunch * (Mathf.PerlinNoise(Random.value, Time.fixedDeltaTime)) * shake;
        punchTrauma = Mathf.Clamp01(punchTrauma - punchRecoverySpeedy * Time.fixedDeltaTime);
    }

    public void InduceAimPunch(float punchValue)
    {
        maxPunch = Mathf.Pow(punchValue, 2);
        punchTrauma = Mathf.Clamp01(punchTrauma + punchValue);
    }
}
