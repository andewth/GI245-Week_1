using UnityEngine;

public class Enemy : Character
{

    [SerializeField] private int expDrop;
    public int ExpDrop { get { return expDrop; } set { expDrop = value; } }


    void Start()
    {
        if (invManager == null)
        {
            CharInit(VFXManager.Instance, UIManager.instance, InventoryManager.instance, PartyManager.instance);
        }

        // Fallback: if this enemy is NOT in EnemyManager list, add random items
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
                int itemCount = InventoryManager.instance.ItemData.Length;
                if (itemCount > 0)
                {
                    int itemsToGive = Random.Range(1, 4); // 1 to 3 items
                    for (int i = 0; i < itemsToGive; i++)
                    {
                        int itemId = Random.Range(0, itemCount);
                        InventoryManager.instance.AddItem(this, itemId);
                    }
                }
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
