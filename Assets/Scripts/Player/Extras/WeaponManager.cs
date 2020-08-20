using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public Transform inventory;
    [SerializeField] private GameObject selectedItem;
    [SerializeField] private GameObject[] playerInventory;
    private int itemIndex = 0;

    void Start()
    {
       playerInventory = GetItems(inventory);
       SelectWeapon();
    }

    private GameObject[] GetItems(Transform backpack)
    {
        GameObject[] inventoryList = new GameObject[inventory.childCount];
        for(int i = 0; i < inventory.childCount; i++)
        {
            inventoryList[i] = backpack.GetChild(i).gameObject;
        }

        return inventoryList;
    }

    public void AddNewItem(Transform newItem)
    {
        // WIP
        Vector3 newPos = new Vector3(0.327f, -0.215f, 0);
        newItem.SetParent(transform, false);
        newItem.localPosition = newPos;
        playerInventory = GetItems(inventory);
    }

    void Update()
    {
        if(!inventory.gameObject.activeSelf)
            return;

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
