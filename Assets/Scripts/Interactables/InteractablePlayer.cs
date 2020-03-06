using UnityEngine;

public class InteractablePlayer : Interactable
{
    public float armLength = 5;
    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Interact"))
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            if (Physics.Raycast(ray, out RaycastHit hit, armLength, LayerMask.GetMask("Interactive")))
            {
                Interactable interactObject = hit.transform.GetComponent<Interactable>();
                
                if(interactObject)
                    interactObject.OnInteract();
            }
        }
    }
}

