using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Entity Details", menuName = "Game/Entites/Details")]
public class EntityDetails : ScriptableObject
{
    [Header("General")]
    public new string name;

    [Header("Health")]
    public float currentHealth = 100;
    public float maximumHealth = 100;
    public float currentArmor = 0;
    public float maxArmor = 0;
    public float damageResistance = 0.0f;

    [Header("Movement")]
    public float moveSpeed = 1;
}

