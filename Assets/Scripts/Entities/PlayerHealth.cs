using UnityEngine;

public class PlayerHealth : EntityHealth
{
    [Header("Armor")]
    public float maxArmor = 200;
    public float currentArmor = 0;

    public void AddArmor(float armor)
    {
        currentArmor += armor;
    }

    public override void ProcessDamage(float damage)
    {
        if(currentArmor < damage)
        {
            damage -= currentArmor;
            currentHealth -= damage;
            currentArmor = 0;
        }
        else
            currentArmor -= damage;
            
        if(currentHealth <= 0)
        {
            OnDeath();
            currentHealth = 0;
        }
    }
}
