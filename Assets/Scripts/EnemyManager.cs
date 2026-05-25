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
        foreach (Character m in monsters)
        {
            if (m == null) 
            {
                Debug.LogWarning("Found an empty monster slot in EnemyManager. Skipping...");
                continue; 
            }

            m.CharInit(VFXManager.Instance, UIManager.instance, InventoryManager.instance, PartyManager.instance);
        }


        if (monsters.Count > 0 && monsters[0] != null)
        {
            InventoryManager.instance.AddItem(monsters[0], 0); //Health Potion
            InventoryManager.instance.AddItem(monsters[0], 1); //Sword
            InventoryManager.instance.AddItem(monsters[0], 2); //Shield
        }
    }
}