using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public virtual void OnInteract()
    {
        throw new Exception("Interaction not set on: " + gameObject);
    }
}
