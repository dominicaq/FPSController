using UnityEngine;

namespace Environmental
{
    public class SunCycle : MonoBehaviour
    {
        [SerializeField] private float turnRate = 2.0f;
        private float rate;

        void Update()
        {
            rate += Time.deltaTime * turnRate;

            Vector3 v = new Vector3(rate, -45, -45);
            transform.localRotation = Quaternion.Euler(v);
        }
    }
}