using UnityEngine;

namespace Jiang.Games
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "Character Stats/PlayerData", order = 0)]
    public class PlayerData_SO : ScriptableObject
    {
        public PlayerBaseData PlayerBaseData;
    }
}