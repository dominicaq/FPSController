using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies.NPC
{
    public class NPCMovement : MonoBehaviour
    {
        [SerializeField] private Transform targetObject;

        private NavMeshAgent navMeshAgent;
        // Start is called before the first frame update
        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            
            if(!navMeshAgent)
                Debug.Log("Nav mesh agent not attached to " + gameObject);
        }

        private void Update()
        {
            if (targetObject)
            {
                Vector3 targetVector = targetObject.transform.position;
                navMeshAgent.SetDestination(targetVector);
            }
        }

        public void ToggleMovement(bool key)
        {
            navMeshAgent.enabled = key;
            enabled = key;
        }
    }
}

