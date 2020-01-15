using UnityEngine;

public class EnableAllCollsions : MonoBehaviour
{
    public bool dropped = false;
    private BoxCollider boxCol;
    private Collider parentObjectCollider;
    public bool reEnableColl;

    private int playerMask, ignorePlayerMask;
    // Start is called before the first frame update
    void Start()
    {
        boxCol = transform.gameObject.AddComponent<BoxCollider>();
        boxCol.isTrigger = true;

        parentObjectCollider = transform.parent.GetComponent<Collider>();
        boxCol.size = parentObjectCollider.bounds.size;

        playerMask = LayerMask.NameToLayer("Player");
        ignorePlayerMask = LayerMask.NameToLayer("IgnorePlayer");

        transform.parent.gameObject.layer = ignorePlayerMask;
        foreach (Transform child in transform.parent)
        {
            if (!child.gameObject || child.gameObject == gameObject)
                break;

            child.gameObject.layer = ignorePlayerMask;
        }
    }

    private void Update()
    {
        if (reEnableColl && dropped)
        {
            transform.parent.gameObject.layer = LayerMask.NameToLayer("Default");
            foreach (Transform child in transform.parent)
            {
                if (!child.gameObject)
                    break;

                child.gameObject.layer = LayerMask.NameToLayer("Default");
            }

            Destroy(gameObject);
        }
    }
    
    void OnTriggerStay(Collider other){
        if(other.gameObject.layer ==  playerMask){
            reEnableColl = false;
        }
    }
 
    void OnTriggerExit(Collider other){
        if(other.gameObject.layer == playerMask){
            reEnableColl = true;
        }
    }
}
