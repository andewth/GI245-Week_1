using UnityEngine;

public class ItemPick : MonoBehaviour
{
    [SerializeField]
    private Item item;
    public Item Item
    { get { return item; } }

    private InventoryManager inventoryManager;
    private PartyManager partyManager;

    private void Start()
    {
        if (partyManager == null)
        {
            partyManager = FindObjectOfType<PartyManager>();
        }

        if (inventoryManager == null)
        {
            inventoryManager = FindObjectOfType<InventoryManager>();
        }
    }


    public void Init(Item item, InventoryManager invManager, PartyManager ptyManager)
    {
        this.item = item;
        inventoryManager = invManager;
        partyManager = ptyManager;
    }

    public void PickUpItem()
    {
        if (inventoryManager == null || partyManager == null || item == null)
            return;

        if (partyManager.SelectChars.Count == 0)
            return;

        if (inventoryManager.AddItem(partyManager.SelectChars[0], item.ID))
        {
            Destroy(gameObject);
        }
    }

    // private void OnMouseDown()
    // {
    //     Debug.Log("Pick Up");

    //     if (partyManager == null)
    //     {
    //         Debug.LogError("PartyManager is null on " + gameObject.name + ". Was Init() called?");
    //         return; 
    //     }

    //     if (partyManager.SelectChars == null)
    //     {
    //         Debug.LogError("SelectChars list in PartyManager is null. Ensure it is initialized.");
    //         return;
    //     }

    //     if (partyManager.SelectChars.Count > 0)
    //     {
    //         PickUpItem(partyManager.SelectChars[0]);
    //     }
    // }

}


