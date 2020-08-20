using UnityEngine;

public class SpinningCube : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        
        Vector3 currentRot = transform.eulerAngles;
        currentRot.x += Time.deltaTime * 180;
        currentRot.z += Time.deltaTime * 180;
        currentRot.y += Time.deltaTime * 30;
        transform.eulerAngles = currentRot;
    }
}
