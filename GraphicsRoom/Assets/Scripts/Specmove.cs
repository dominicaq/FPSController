using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Specmove : MonoBehaviour
{
    public float movementSpeed = 5.0f;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        rb.velocity = Vector3.zero;
        transform.Translate(Input.GetAxis("Horizontal") * Time.deltaTime * movementSpeed, 0, 0);
        transform.Translate(0, 0, Input.GetAxis("Vertical") * Time.deltaTime * movementSpeed);
    }
}
