using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum AbilityType
{
    Active,
    Passive
};

public abstract class AbilityBase : MonoBehaviour
{
    public AbilityType aType;
    public float coolDown;
    public float duration;

    public bool abilityAvaliable = true;
    private WaitForSeconds waitForSeconds;

    public virtual void Init(){
        waitForSeconds = new WaitForSeconds(coolDown);
    }

    public virtual void Ability(){}
    public virtual void Tick(){}
    public virtual void FixedTick(){}

    public void Use()
    {
        if(!abilityAvaliable)
            return;
        
        Ability();

        if(coolDown > 0)
            StartCoroutine(Cooldown());
    }

    public IEnumerator Cooldown()
    {
        abilityAvaliable = false;
        yield return waitForSeconds;
        abilityAvaliable = true;
    }
}

/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class AbilityBase : MonoBehaviour
{
    public class MyFloatEvent : UnityEvent<float> { }
    public MyFloatEvent OnAbilityUse = new MyFloatEvent();
    [Header("Ability Info")]
    public string title;
    public Sprite icon;
    public float cooldownTime = 1;
    private bool canUse = true;


    public void TriggerAbility()
    {
        if (canUse)
        {
            OnAbilityUse.Invoke(cooldownTime);
            Ability();
            StartCooldown();
        }

    }
    public abstract void Ability();
    void StartCooldown()
    {
        StartCoroutine(Cooldown());
        IEnumerator Cooldown()
        {
            canUse = false;
            yield return new WaitForSeconds(cooldownTime);
            canUse = true;
        }
    }
}
*/