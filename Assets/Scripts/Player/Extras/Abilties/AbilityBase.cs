using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum AbilityType
{
    Active,
    Passive
};

public abstract class AbilityBase : ScriptableObject
{
    // Learn these concepts: Unity events
    /*
    public class MyFloatEvent : UnityEvent<float> { }
    public MyFloatEvent OnAbilityUse = new MyFloatEvent();
    */

    public string aName = "UNDEFINED_ABILITY_NAME";
    public AbilityType aType;
    public float aCooldown = 1;
    public float aDuration = 1;
    public AudioClip aClip;

    private bool m_canUse = true;
    private WaitForSeconds m_cd;

    public abstract void Initialize(GameObject obj);

    public virtual void InitAbility()
    {
        m_cd = new WaitForSeconds(aCooldown);    
    }

    public virtual void Ability()
    {

    }

    public void UseAbility()
    {
        if(m_canUse && aType != AbilityType.Passive)
        {
            StartCoolDown();
            Ability();
        }
    }

    private IEnumerator StartCoolDown()
    {
        m_canUse = false;
        yield return m_cd;
        m_canUse = true;
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