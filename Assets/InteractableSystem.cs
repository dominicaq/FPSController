using UnityEngine;

public class InteractableSystem : PlayerHandyMan
{
    public float interactLength;
    private Camera cam;

    private void Start()
    {
        cam = transform.GetComponent<Camera>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Interact"))
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            if (Physics.Raycast(ray, out RaycastHit hit, interactLength, LayerMask.GetMask("Interactable")))
            {
                PlayerHandyMan entity= hit.transform.GetComponent<PlayerHandyMan>();
                
                if(entity)
                    entity.OnInteract();
            }
        }
    }
}

public class PlayerHandyMan : MonoBehaviour
{
    public virtual void OnInteract()
    {
        Debug.LogWarning("No interaction set for: " + transform.name);
    }
}


