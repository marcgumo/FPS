using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropTableController : MonoBehaviour
{
    [System.Serializable]
    
    public class LootDrop
    {
        public GameObject item;
        public int weight;
    }

    public List<LootDrop> loot;

    public GameObject SpawnDrop()
    {
        int roll = Random.Range(1, 100);
        int weightSum = 0;

        foreach (LootDrop item in loot)
        {
            weightSum += item.weight;

            if (roll < weightSum)
            {
                return item.item;
            }
        }

        return null;
    }
}
