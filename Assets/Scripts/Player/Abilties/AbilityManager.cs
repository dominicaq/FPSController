using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public AbilityBase currentActiveAbility;
    private List<AbilityBase> activeAbilities;
    
    void Start()
    {
        currentActiveAbility = activeAbilities[0];
    }

    public void UseActiveAbility()
    {
        currentActiveAbility.Use();
    }

    private void Update()
    {
        for(int i = 0; i < activeAbilities.Count; i++)
            activeAbilities[i].Tick();
    }

    private void FixedUpdate() 
    {
        for(int i = 0; i < activeAbilities.Count; i++)
            activeAbilities[i].FixedTick();
    }

    public void AddAbility(AbilityBase newAbility)
    {
        activeAbilities.Add(newAbility);
    }

    public List<AbilityBase> GetCurrentAbilities()
    {
        return activeAbilities;
    }
}
