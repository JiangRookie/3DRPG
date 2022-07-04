using UnityEngine;

[CreateAssetMenu(fileName = "Attack Data", menuName = "Character Stats/Attack")]
public class AttackData_SO : ScriptableObject
{
    public float attackRange;
    public float skillRange;
    public float coolDown;
    public int minDamage;
    public int maxDamage;

    public float criticalMultiplier;
    public float criticalChance;
}