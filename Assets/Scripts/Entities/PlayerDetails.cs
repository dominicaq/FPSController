using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDetails", menuName = "Player/Details")]
public class PlayerDetails : ScriptableObject
{
    [Header("General")]
    public new string name = "Player";

    [Header("Health")]
    public float currentHealth = 100;
    public float maximumHealth = 100;
    public float currentArmor = 0;
    public float maxArmor = 100;
    public float damageResistance = 0.0f;
}
