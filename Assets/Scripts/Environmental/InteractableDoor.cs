using System;
using UnityEngine;

public class InteractableDoor : Interactable
{
    public bool isOpen = false;
    //public Animator doorAnim;
    
    public override void OnInteract()
    {
        isOpen = !isOpen;
        gameObject.SetActive(false);
        //doorAnim.SetBool("open", isOpen);
    }
}
