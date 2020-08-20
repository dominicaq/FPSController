using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100;
    public float currentHealth = 100;

    private void Start() 
    {
        if(gameObject.CompareTag("Unit"))
            gameObject.tag = "Unit";
    }

    public float GetHealthPercent()
    {
        return currentHealth / maxHealth;
    }
    
    public void AddHealth(float amount)
    {
        currentHealth += amount;

        if(currentHealth >= maxHealth)
            currentHealth = maxHealth;
    }

    public virtual void ProcessDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            OnDeath();
            currentHealth = 0;
        }
    }

    public void SendDamage(float damage)
    {
        ProcessDamage(damage);
    }

    public virtual void OnDeath()
    {
        Debug.Log("OnDeath() is not set!: " + transform.name);
    }

    public void Suicide()
    {
        SendDamage(9999);
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("OutOfBounds"))
            Suicide();
    }
}
