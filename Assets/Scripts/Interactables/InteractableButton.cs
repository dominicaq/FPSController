using UnityEngine;

public class InteractableButton : Interactable
{
    public Transform entity;

    public override void OnInteract()
    {
        Interactable savedEnt = entity.GetComponent<Interactable>();
        if (savedEnt)
        {
            savedEnt.OnInteract();
        }
    }
}