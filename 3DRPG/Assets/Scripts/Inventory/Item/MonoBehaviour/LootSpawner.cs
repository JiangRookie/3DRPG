using UnityEngine;

public class LootSpawner : MonoBehaviour
{
    [System.Serializable]
    public class LootItem
    {
        public GameObject item;
        [Range(0, 1)] public float weight;
    }

    public LootItem[] lootItems;

    public void SpawnLoot()
    {
        var currentValue = Random.value;
        for (int i = 0; i < lootItems.Length; i++)
        {
            if (currentValue <= lootItems[i].weight)
            {
                GameObject obj = Instantiate(lootItems[i].item);
                obj.transform.position = transform.position + Vector3.up * 2;
                break; // 只要有一个物品掉落了就退出循环
            }
        }
    }
}