using TMPro;
using UnityEngine;

public class interfacePlayerHP : MonoBehaviour
{
    public Transform player;
    private EntityHealth m_playerHp;
    public TextMeshProUGUI m_PlayerHpText;
    //private int oldVal = int.MaxValue;
    
    void Start() 
    {
        m_playerHp     = player.GetComponent<EntityHealth>();
        m_PlayerHpText = transform.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        int val = (int)m_playerHp.currentHealth;
        if (val != oldVal)
        {
            oldVal = val;
            m_PlayerHpText.text = val.ToString();
        }
        */
    }
}
