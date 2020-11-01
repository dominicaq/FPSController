using UnityEngine;

public class Interact : AbilityBase
{
    public float interactDistance = 5;
    private LayerMask interactLayer;
    private Ray screenRay;

    public override void Init() 
    {
        screenRay = GetComponent<Protagonist>().cameraController.centerOfScreenRay;
        interactLayer = LayerMask.GetMask("Interactive");
    }

    public override void Ability()
    {
        if (Physics.Raycast(screenRay, out RaycastHit hit, interactDistance, interactLayer))
        {
            IInteractable targetInteractable = hit.collider.GetComponent<IInteractable>();

            if(targetInteractable != null)
                targetInteractable.OnInteract();
        }
    }
}
