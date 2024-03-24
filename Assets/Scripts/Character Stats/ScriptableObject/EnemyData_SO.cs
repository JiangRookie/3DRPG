using UnityEngine;

namespace Jiang.Games
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Character Stats/EnemyData", order = 1)]
    public class EnemyData_SO : ScriptableObject
    {
        public EnemyBaseData EnemyBaseData;
    }
}