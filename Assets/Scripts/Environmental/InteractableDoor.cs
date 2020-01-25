using System;
using UnityEngine;

public class InteractableDoor : PlayerHandyMan
{
    public bool isOpen = false;
    public Animator doorAnim;
    
    public override void OnInteract()
    {
        isOpen = !isOpen;
        doorAnim.SetBool("open", isOpen);
    }
}
