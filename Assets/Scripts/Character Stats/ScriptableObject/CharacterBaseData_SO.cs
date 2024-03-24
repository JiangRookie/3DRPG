using System;
using UnityEngine;

namespace Jiang.Games
{
    [Serializable]
    public struct CharacterBaseData
    {
        public int MaxHealth;
        public int CurrentHealth;
        public int BaseDefence;
        public int CurrentDefence;
    }

    [Serializable]
    public class PlayerBaseData
    {
        public CharacterBaseData CharacterBaseData;
        public int CurrentLevel;
        public int MaxLevel;
        public int BaseExp;
        public int CurrentExp;
        public float LevelBuff;
        public float LevelMultiplier => 1 + (CurrentLevel - 1) + LevelBuff;

        public void UpdateExp(int point)
        {
            CurrentExp += point;

            if (CurrentExp >= BaseExp)
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            CurrentLevel = Mathf.Clamp(CurrentLevel + 1, 0, MaxLevel);
            BaseExp += (int)(BaseExp * LevelMultiplier);
            CharacterBaseData.MaxHealth = (int)(CharacterBaseData.MaxHealth * LevelMultiplier);
            CharacterBaseData.CurrentHealth = CharacterBaseData.MaxHealth;
        }
    }

    [Serializable]
    public class EnemyBaseData
    {
        public CharacterBaseData CharacterBaseData;
        public int KillPoint;
    }
}