using UnityEngine;

public class WeaponSwitch : MonoBehaviour
{
    [SerializeField] private int itemIndex = 0;
    [SerializeField] private GameObject[] playerInventory;
    [SerializeField] private GameObject selectedItem;
    private int childCount = 0;

    void Start()
    {
       playerInventory = GetItems(gameObject);
       SelectWeapon();
    }

    private GameObject[] GetItems(GameObject backpack)
    {
        childCount = transform.childCount;
        GameObject[] ret = new GameObject[childCount];
        for(int i = 0; i < childCount; i++)
        {
            ret[i] = backpack.transform.GetChild(i).gameObject;
        }

        return ret;
    }

    public void AddNewItem(Transform newItem)
    {
        // WIP
        Vector3 newPos = new Vector3(0.327f, -0.215f, 0);
        newItem.SetParent(transform, false);
        newItem.localPosition = newPos;
        playerInventory = GetItems(gameObject);
    }

    void Update()
    {
        int previousItem = itemIndex;
        // Input
        if (Input.GetAxis("Mouse ScrollWheel") > 0f ) // forward
        {
            if(itemIndex >= playerInventory.Length-1)
                itemIndex = 0;
            else
                itemIndex++;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f ) // backward
        {
            if (itemIndex <= 0)
                itemIndex = playerInventory.Length-1;
            else
                itemIndex--;
        }

        if(previousItem != itemIndex)
            SelectWeapon();
    }

    void SelectWeapon()
    {
        selectedItem = playerInventory[itemIndex];

        for(int i = 0; i < playerInventory.Length; i++)
        {
            if(i == itemIndex)
                playerInventory[i].SetActive(true);
            else
                playerInventory[i].SetActive(false);
        }
    }
}
