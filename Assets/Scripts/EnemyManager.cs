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


    }
}