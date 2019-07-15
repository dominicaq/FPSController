using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectMoney : MonoBehaviour
{
    public int moneyValue = 0;

    // Start is called before the first frame update
    void Awake()
    {
        moneyValue = Random.Range(1, 5) * 5;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
