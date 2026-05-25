using UnityEngine;

public class Enemy : Character
{

    [SerializeField] private int expDrop;
    public int ExpDrop { get { return expDrop; } set { expDrop = value; } }


    void Start()
    {
        Debug.Log("Enemy Start: " + name + ", invManager=" + (invManager != null));

        if (invManager == null)
        {
            Debug.Log("Enemy " + name + " calling CharInit...");
            CharInit(VFXManager.Instance, UIManager.instance, InventoryManager.instance, PartyManager.instance);
        }

        // Fallback: if this enemy is NOT in EnemyManager list, add items here
        if (InventoryManager.instance != null && inventoryItems != null)
        {
            bool hasAnyItem = false;
            for (int i = 0; i < inventoryItems.Length; i++)
            {
                if (inventoryItems[i] != null)
                {
                    hasAnyItem = true;
                    break;
                }
            }

            if (!hasAnyItem)
            {
                Debug.Log("Enemy " + name + " has no items, adding default items...");
                InventoryManager.instance.AddItem(this, 0); //Health Potion
                InventoryManager.instance.AddItem(this, 1); //Sword
                InventoryManager.instance.AddItem(this, 2); //Shield
            }
        }
    }

    private void Update() 
    {
        switch (state)
        {
            case CharState.Walk:
                WalkUpdate();
                break;
            
            case CharState.WalkToEnemy:
                WalkToEnemyUpdate();
                break;
        }
    }


    protected override void Die()
    {
        base.Die();
        PartyManager pm = partyManager ?? PartyManager.instance;
        if (pm != null)
            pm.DistributeTotalExp(expDrop);
    }
}
