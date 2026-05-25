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

            Debug.Log("Adding items to: " + m.name);

            bool added0 = InventoryManager.instance.AddItem(m, 0); //Health Potion
            bool added1 = InventoryManager.instance.AddItem(m, 1); //Sword
            bool added2 = InventoryManager.instance.AddItem(m, 2); //Shield

            Debug.Log($"Items added to {m.name}: potion={added0}, sword={added1}, shield={added2}");
        }
    }
}