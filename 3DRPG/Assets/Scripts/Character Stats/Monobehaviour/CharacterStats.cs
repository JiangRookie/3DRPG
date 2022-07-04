using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterStats : MonoBehaviour
{
    public event Action<int, int> UpdateHealthBarOnAttack;
    public CharacterData_SO templateData;
    public CharacterData_SO characterData;
    public AttackData_SO attackData;
    private AttackData_SO m_BaseAttackData;

    [HideInInspector] public bool isCritical;

    private static readonly int Hit = Animator.StringToHash("Hit");

    private void Awake()
    {
        if (templateData != null)
            characterData = Instantiate(templateData);

        m_BaseAttackData = Instantiate(attackData);
    }

    #region Read from Data_SO

    public int MaxHealth
    {
        get => characterData.maxHealth = characterData != null ? characterData.maxHealth : 0;
        set => characterData.maxHealth = value;
    }

    public int CurrentHealth
    {
        get => characterData.currentHealth = characterData != null ? characterData.currentHealth : 0;
        set => characterData.currentHealth = value;
    }

    public int BaseDefence
    {
        get => characterData.baseDefence = characterData != null ? characterData.baseDefence : 0;
        set => characterData.baseDefence = value;
    }

    public int CurrentDefence
    {
        get => characterData.currentDefence = characterData != null ? characterData.currentDefence : 0;
        set => characterData.currentDefence = value;
    }

    #endregion

    #region Character Combat

    public void TakeDamage(CharacterStats theAttacker, CharacterStats theDefender)
    {
        var theRealDamage = Mathf.Max(theAttacker.CurrentDamage() - theDefender.CurrentDefence, 0);
        CurrentHealth = Mathf.Max(CurrentHealth - theRealDamage, 0);

        if (theAttacker.isCritical)
        {
            theDefender.GetComponent<Animator>().SetTrigger(Hit);
        }


        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);

        if (CurrentHealth <= 0)
            theAttacker.characterData.UpdateExp(characterData.killPoint);
    }

    public void TakeDamage(int damage, CharacterStats theDefender)
    {
        var theRealDamage = Mathf.Max(damage - theDefender.CurrentDefence, 0);
        CurrentHealth = Mathf.Max(CurrentHealth - theRealDamage, 0);

        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);

        if (CurrentHealth <= 0)
            GameManager.Instance.playerStats.characterData.UpdateExp(characterData.killPoint);
    }

    private int CurrentDamage()
    {
        float coreDamage = Random.Range(attackData.minDamage, attackData.maxDamage);

        if (isCritical)
        {
            coreDamage *= attackData.criticalMultiplier;
        }

        return (int)coreDamage;
    }

    #endregion
}