using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterStats : MonoBehaviour
{
    public event Action<int, int> UpdateHealthBarOnAttack;
    [HideInInspector] public bool isCritical;

    public CharacterData_SO templateData;
    public CharacterData_SO characterData;
    public AttackData_SO attackData;
    public AttackData_SO baseAttackData;
    
    private RuntimeAnimatorController m_BaseAnimator;
    private static readonly int Hit = Animator.StringToHash("Hit");

    [Header("武器")] public Transform weaponSlot;

    private void Awake()
    {
        if (templateData != null)
            characterData = Instantiate(templateData);

        baseAttackData = Instantiate(attackData);
        m_BaseAnimator = GetComponent<Animator>().runtimeAnimatorController;
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
            GameManager.Instance.playerCharacterStats.characterData.UpdateExp(characterData.killPoint);
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

    #region Equip Weapon

    public void ChangeWeapon(ItemData_SO weapon)
    {
        UnEquipmentWeapon();
        EquipWeapon(weapon);
    }

    public void EquipWeapon(ItemData_SO weapon)
    {
        if (weapon.weaponPrefab == null) return;
        Instantiate(weapon.weaponPrefab, weaponSlot);

        // TODO:更新属性
        // TODO:切换动画
        attackData.ApplyWeaponData(weapon.weaponData);
        // InventoryManager.Instance.UpdateStatsText(MaxHealth, attackData.minDamage, attackData.maxDamage);
        GetComponent<Animator>().runtimeAnimatorController = weapon.weaponAnimator;
    }

    public void UnEquipmentWeapon()
    {
        if (weaponSlot.transform.childCount != 0)
        {
            for (int i = 0; i < weaponSlot.transform.childCount; i++)
            {
                Destroy(weaponSlot.transform.GetChild(i).gameObject);
            }
        }

        attackData.RestoreWeaponData(baseAttackData);
        // TODO:切换动画
        GetComponent<Animator>().runtimeAnimatorController = m_BaseAnimator;
    }

    #endregion

    #region Apply Data Change

    public void ApplyHealth(int amount)
    {
        if (CurrentHealth + amount <= MaxHealth)
            CurrentHealth += amount;
        else
            CurrentHealth = MaxHealth;
    }

    #endregion
}