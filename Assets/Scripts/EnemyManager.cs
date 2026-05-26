using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private List<Enemy> monsters;
    public List<Enemy> Monsters
    { get { return monsters; } }

    public static EnemyManager instance;

    void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("EnemyManager Start: monsters count = " + (monsters != null ? monsters.Count : -1));

        if (monsters == null)
        {
            Debug.LogError("EnemyManager: monsters list is null!");
            return;
        }

        foreach (Character m in monsters)
        {
            if (m == null) 
            {
                Debug.LogWarning("Found an empty monster slot in EnemyManager. Skipping...");
                continue; 
            }

            Debug.Log("CharInit for: " + m.name);
            m.CharInit(VFXManager.Instance, UIManager.instance, InventoryManager.instance, PartyManager.instance);
        }


        foreach (Enemy m in monsters)
        {
            if (m == null)
                continue;

            int itemCount = InventoryManager.instance.ItemData.Length;
            if (itemCount == 0)
                continue;

            // Random 1-3 items per enemy, can be duplicates or different items
            int itemsToGive = Random.Range(1, 4); // 1 to 3 items

            for (int i = 0; i < itemsToGive; i++)
            {
                int itemId = Random.Range(0, itemCount);
                int slot = InventoryManager.instance.AddItem(m, itemId);
                if (slot >= 0)
                {
                    Debug.Log($"Enemy {m.name} got item <color=green>{InventoryManager.instance.ItemData[itemId].itemName}</color> (id={itemId}) at slot <color=blue>{slot}</color>");
                }
            }
        }
    }
}