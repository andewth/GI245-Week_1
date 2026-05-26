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

        if (InventoryManager.instance == null || InventoryManager.instance.ItemData == null || InventoryManager.instance.ItemData.Length == 0)
            return;

        if (inventoryItems == null || inventoryItems.Length == 0)
            CharInit(VFXManager.Instance, UIManager.instance, InventoryManager.instance, PartyManager.instance);

        inventoryItems = new Item[InventoryManager.MAXSLOT];

        for (int i = 0; i < 3; i++)
        {
            int itemId = Random.Range(0, InventoryManager.instance.ItemData.Length);
            int slot = InventoryManager.instance.AddItem(this, itemId);
            if (slot >= 0)
            {
                Debug.Log($"Enemy {name} got item <color=green>{InventoryManager.instance.ItemData[itemId].itemName}</color> (id={itemId}) at slot <color=blue>{slot}</color>");
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
