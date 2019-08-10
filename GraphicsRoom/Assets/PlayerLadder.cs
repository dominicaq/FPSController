using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLadder : MonoBehaviour
{
    private PlayerController PlayerMovement;

    private void Awake()
    {
        PlayerMovement = transform.GetComponent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Ladder")
        {
            PlayerMovement.isClimbingLadder = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Ladder")
        {
            PlayerMovement.isClimbingLadder = false;
        }
    }
}
