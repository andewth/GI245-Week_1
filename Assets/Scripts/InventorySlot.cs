using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{

    [SerializeField] 
    private int id;
    public int ID { get { return id; } set { id = value; } }


    [SerializeField] 
    private ItemType itemType;
    public ItemType ItemType
    { get {return itemType;} set { itemType = value; } }

    [SerializeField]
    InventoryManager inventoryManager;

    void Start()
    {
        inventoryManager = InventoryManager.instance;
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject objA = eventData.pointerDrag;
        if (objA == null)
            return;

        ItemDrag itemDragA = objA.GetComponent<ItemDrag>();
        if (itemDragA == null || itemDragA.IconParent == null || itemDragA.Item == null)
            return;

        InventorySlot slotA = itemDragA.IconParent.GetComponent<InventorySlot>();
        if (slotA == null)
            return;

        if (itemType == ItemType.Shield)
        {
            if (itemDragA.Item.Type != itemType)
                return;
        }

        if (transform.childCount > 0)
        {
            GameObject objB = transform.GetChild(0).gameObject;
            ItemDrag itemDragB = objB.GetComponent<ItemDrag>();
            if (itemDragB == null || itemDragB.Item == null)
                return;

            if (slotA.ItemType == ItemType.Shield)
            {
                if (itemDragB.Item.Type != slotA.ItemType)
                    return;
            }

            inventoryManager.RemoveItemInBag(slotA.ID);

            itemDragB.transform.SetParent(itemDragA.IconParent);
            itemDragB.IconParent = itemDragA.IconParent;
            inventoryManager.SaveItemInBag(slotA.ID, itemDragB.Item);

            inventoryManager.RemoveItemInBag(id);
        }
        else
        {
            inventoryManager.RemoveItemInBag(slotA.ID);
        }

        itemDragA.IconParent = transform;
        inventoryManager.SaveItemInBag(id, itemDragA.Item);
    }
}
