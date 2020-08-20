using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBall : DamageExplosion
{
    [Header("Ball Properties")]
    public float sphereHitDamage = 15.0f;
    public int maxBounces = 2;
    public float movementSpeed = 3;
    public float sphereRadius = 0.5f;
    private int m_CurrentCount = 0;
    private Vector3 m_CurrentDir;

    [Header("Audio")]
    public AudioClip[] clips = new AudioClip[3];
    private AudioSource m_AudioSource;

    private void Start()
    {
        m_CurrentDir = transform.forward;
        m_AudioSource = GetComponent<AudioSource>();
    }

    private void Update() 
    {
        RunTimer();
        transform.Translate(m_CurrentDir * Time.deltaTime * movementSpeed, Space.World);
        
        //if(Physics.SphereCast(transform.position, sphereRadius / 2, m_CurrentDir, out RaycastHit hit, 0.1f, ~0, QueryTriggerInteraction.Ignore))
        if(Physics.Raycast(transform.position, m_CurrentDir, out RaycastHit hit , sphereRadius / 2, ~0 ,QueryTriggerInteraction.Ignore))
        {
            m_CurrentDir = Vector3.Reflect(m_CurrentDir, hit.normal);
            m_CurrentCount++;

            m_AudioSource.clip = clips[1];
            m_AudioSource.Play();

            EntityHealth entityHP = hit.transform.GetComponent<EntityHealth>();

            if(entityHP)
                entityHP.SendDamage(sphereHitDamage);
        }

        if(m_CurrentCount == maxBounces)
            Explode();
    }
}
