using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies.NPC
{
    public class NPCMovement : MonoBehaviour
    {
        public Transform target;
        private NavMeshAgent m_Agent;
        private Vector3 m_Destination;

        private void Start()
        {
            m_Agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            Vector3 targetVec = target.position;
            if (Vector3.Distance(m_Destination, targetVec) > 3.0f)
            {
                m_Destination = target.position;
                m_Agent.destination = m_Destination;
            }
        }
    }
}

