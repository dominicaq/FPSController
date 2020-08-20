using System;
using UnityEngine;

public class InteractableDoor : MonoBehaviour, IInteractable
{
    public bool isOpen {get; private set;}
    //public Animator doorAnim;
    
    public void OnInteract()
    {
        isOpen = !isOpen;
        gameObject.SetActive(false);
        //doorAnim.SetBool("open", isOpen);
    }
}
