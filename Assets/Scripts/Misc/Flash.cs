using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Flash : MonoBehaviour
{
    public float radius = 5.0f;
    public float armDelay = 2.0f;
    public float flashTime = 0.5f;
    public Image flashImage;

    private Collider[] m_affectedUnits = new Collider[2];
    private float m_randTime;
    private bool m_isActive;
    private int m_playerMask = 8;

    void Start()
    {
        StartCoroutine(SetArmDelay(armDelay));
    }

    void Update()
    {
        if(m_isActive)
        {
            int numPossibleUnits = Physics.OverlapSphereNonAlloc(transform.position, radius, m_affectedUnits, 1 << m_playerMask);

            for(int i = 0; i < numPossibleUnits; i++)
            {
                if (Physics.Linecast(transform.position, m_affectedUnits[i].transform.position, out RaycastHit hit))
                {
                    if(hit.transform.gameObject.layer == m_playerMask)
                    {
                        Color newColor = Color.white;
                        flashImage.color = newColor;

                        StartCoroutine(Unflash(flashTime));
                    }
                }
            }
            m_isActive = false;
        }
    }

    IEnumerator Unflash(float flashTime)
    {
        float lerpParam = 0;
        while(lerpParam <= 1)
        {
            lerpParam += Time.deltaTime * flashTime;
            float alpha = Mathf.Lerp(1, 0, lerpParam);

            Color newColor = new Color(1, 1, 1, alpha);
            flashImage.color = newColor;
            yield return null;
        }
    }

    IEnumerator SetArmDelay(float time)
    {
        m_isActive = false;
        yield return new WaitForSeconds(time);
        m_isActive = true;
    }
}
