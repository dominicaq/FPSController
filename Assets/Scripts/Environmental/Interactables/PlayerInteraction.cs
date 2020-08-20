using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance = 5;
    private Camera m_Camera;
    private LayerMask m_InteractMask;

    private void Start()
    {
        m_Camera       = GetComponent<Camera>();
        m_InteractMask = LayerMask.GetMask("Interactive");
    }

    void Update()
    {
        if (Input.GetButtonDown("Interact"))
        {
            Ray ray = m_Camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, m_InteractMask))
            {
                IInteractable targetInteractable = hit.collider.GetComponent<IInteractable>();

                if(targetInteractable != null)
                    targetInteractable.OnInteract();
            }
        }
    }
}

