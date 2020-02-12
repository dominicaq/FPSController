using UnityEngine;

public class InteractableButton : PlayerHandyMan
{
    public Transform entity;

    public override void OnInteract()
    {
        EnvironmentalHandyMan savedEnt = entity.GetComponent<EnvironmentalHandyMan>();
        if (savedEnt)
        {
            savedEnt.OnActivation();
        }
    }
}

public class EnvironmentalHandyMan : InteractableButton
{
    public virtual void OnActivation()
    {
        
    }
}