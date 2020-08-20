using System.Collections.Generic;
using UnityEngine;

public class DropTable : MonoBehaviour
{
    [System.Serializable]
    public enum ItemRarity
    {
        Common = 50,
        UnCommon = 30,
        Rare = 15,
        Legendary = 5,
    };

    [System.Serializable]
    public struct DroppableItem
    {
        public GameObject item;
        public ItemRarity rarity;
    };

    [SerializeField]
    #pragma warning disable 0649
    private DroppableItem[] m_Table;

    public List<GameObject> DropItems()
    {
        List<GameObject> itemList = new List<GameObject>();

        for(int i = 0; i < m_Table.Length; i++)
        {
            if ((ItemRarity)Random.Range(1, 100) < m_Table[i].rarity)
                itemList.Add(m_Table[i].item);
        }
        
        return itemList;
    }
}