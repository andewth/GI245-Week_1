using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] itemPrefabs;
    public GameObject[] ItemPrefabs
    { get { return itemPrefabs; } set { itemPrefabs = value; } }

    [SerializeField]
    private ItemData[] itemData;
    public ItemData[] ItemData
    { get { return itemData; } set { itemData = value; } }


    public const int MAXSLOT = 18;

    public static InventoryManager instance;

    void Awake()
    {
        instance = this;
    }


   


    public bool AddItem(Character character, int id)
    {
        if (character == null)
        {
            Debug.LogWarning("AddItem failed: character is null");
            return false;
        }
        if (id < 0 || id >= itemData.Length)
        {
            Debug.LogWarning($"AddItem failed: id {id} out of range (itemData length = {itemData.Length})");
            return false;
        }
        if (itemData[id] == null)
        {
            Debug.LogWarning($"AddItem failed: itemData[{id}] is null. Please assign ItemData in InventoryManager Inspector.");
            return false;
        }

        Item item = new Item(itemData[id]);

        for (int i = 0; i < character.InventoryItems.Length; i++)
        {
            if (character.InventoryItems[i] == null)
            {
                character.InventoryItems[i] = item;
                return true;
            }
        }
        Debug.Log("Inventory Full");
        return false;
    }


    public void SaveItemInBag(int index, Item item)
    {
        if (PartyManager.instance.SelectChars.Count == 0)
            return;

        if (index < 0 || index >= PartyManager.instance.SelectChars[0].InventoryItems.Length || item == null)
            return;
        
        PartyManager.instance.SelectChars[0].InventoryItems[index] = item;

        UnityEngine.Debug.Log("Save Item: " + item.ItemName + " in slot: " + index);

        switch(index)
        {
            case 16:
                PartyManager.instance.SelectChars[0].EquipShield(item);
                break;
            case 17:
                PartyManager.instance.SelectChars[0].EquipWeapon(item);
                break;
        }
    }


    public void RemoveItemInBag(int index)
    {
        if (PartyManager.instance.SelectChars.Count == 0)
            return;

        if (index < 0 || index >= PartyManager.instance.SelectChars[0].InventoryItems.Length)
            return;

        PartyManager.instance.SelectChars[0].InventoryItems[index] = null;

        switch(index)
        {
            case 16:
                PartyManager.instance.SelectChars[0].UnEquipShield(); 
                break;
            case 17:
                PartyManager.instance.SelectChars[0].UnEquipWeapon(); 
                break;
        }
    }


    private void SpawnDropItem(Item item, Vector3 pos)
    {
        if (item == null || ItemPrefabs == null || ItemPrefabs.Length == 0)
            return;

        int prefabId = item.PrefabID;

        if (prefabId < 0 || prefabId >= ItemPrefabs.Length)
        {
            switch (item.Type)
            {
                case ItemType.Consumable:
                    prefabId = 1;
                    break;
                default:
                    prefabId = 0;
                    break;
            }

            if (prefabId < 0 || prefabId >= ItemPrefabs.Length)
                prefabId = 0;
        }

        Vector3 spawnPos = pos + new Vector3(0f, 0.5f, 0f);
        GameObject itemObj = Instantiate(ItemPrefabs[prefabId], spawnPos, Quaternion.identity);

        ItemPick itemPick = itemObj.GetComponent<ItemPick>();
        if (itemPick == null)
        {
            itemPick = itemObj.AddComponent<ItemPick>();
        }

        itemPick.Init(item, instance, PartyManager.instance);
    }


    public float dropRadius = 2.0f;
    public void SpawnDropInventory(Item[] items, Vector3 pos)
    {
        if (items == null)
            return;

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null)
            {
                Vector2 randomOffset = Random.insideUnitCircle * dropRadius;
                Vector3 spawnPos;

                spawnPos = new Vector3(pos.x + randomOffset.x, pos.y, pos.z + randomOffset.y);
                SpawnDropItem(items[i], spawnPos);

                Debug.Log($"Spawned item: <color=green>{items[i].ItemName}</color> from inventory slot <color=blue>{i}</color>");
            }
        }
    }


    public void DrinkConsumableItem(Item item, int slotId)
    {
        if (item == null)
            return;

        string s = string.Format("Drink: {0}", item.ItemName);
        Debug.Log(s);

        if (PartyManager.instance.SelectChars.Count > 0)
        {
            PartyManager.instance.SelectChars[0].Recover(item.Power);
            RemoveItemInBag(slotId);
        }
    }


    public bool CheckPartyForItem(int id)
    {
        if (id < 0 || id >= itemData.Length)
            return false;

        Item item = new Item(itemData[id]);
        Debug.Log(item.ItemName);

        List<Character> party = PartyManager.instance.Members;

        foreach (Character hero in party)
        {
            if (hero == null)
                continue;

            for (int i = 0; i < hero.InventoryItems.Length; i++)
            {
                if (hero.InventoryItems[i] == null)
                    continue;

                Debug.Log(hero.InventoryItems[i].ItemName);
                if (hero.InventoryItems[i].ID == item.ID)
                    return true;
            }
        }
        return false;
    }



    public bool RemoveItemFromParty(int id)
    {
        if (id < 0 || id >= itemData.Length)
            return false;

        Item item = new Item(itemData[id]);
        Debug.Log($"Finding {item.ItemName}");

        List<Character> selectedHero = PartyManager.instance.SelectChars;

        foreach (Character hero in selectedHero)
        {
            if (hero == null)
                continue;

            for (int i = 0; i < hero.InventoryItems.Length; i++)
            {
                if (hero.InventoryItems[i] == null)
                    continue;

                if (hero.InventoryItems[i].ID == item.ID)
                {
                    Debug.Log($"Removing {hero.InventoryItems[i].ItemName}");
                    hero.InventoryItems[i] = null;
                    Debug.Log($"Removed {hero.InventoryItems[i]}");
                    return true;
                }
            }
        }
        return false;
    }


    private void AddItemShopToNPC(int npcId, int itemId)
    {
        Item item = new Item(itemData[itemId]);
        QuestManager.instance.NPCPerson[npcId].ShopItems.Add(item);
    }

}
