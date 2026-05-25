using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ItemDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [SerializeField]
    private Item item;
    public Item Item
    { get { return item; } set { item = value; } }

    [SerializeField]
    private Transform iconParent;
    public Transform IconParent
    { get { return iconParent; } set { iconParent = value; } }

    [SerializeField]
    private Image image;
    public Image Image
    { get { return image; } set { image = value; } }

    private UIManager uiManager;
    public UIManager UIManager
    { get { return uiManager; } set { uiManager = value; } } 


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (image == null)
            return;

        Debug.Log("BeginDrag");
        iconParent = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Mouse.current == null)
            return;

        Debug.Log("Dragging");
        transform.position = Mouse.current.position.ReadValue();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (image == null || iconParent == null)
            return;

        Debug.Log("EndDrag");
        transform.SetParent(iconParent);
        image.raycastTarget = true;
    }


    private int FindIndexOfSlotParent()
    {
        if (iconParent == null)
            return -1;

        InventorySlot slot = iconParent.GetComponent<InventorySlot>();
        if (slot == null)
            return -1;

        int id = slot.ID;
        return id;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("Right Click on Item");
            if (item != null && uiManager != null && item.Type == ItemType.Consumable)
            {
                int slotId = FindIndexOfSlotParent();
                if (slotId < 0)
                    return;

                uiManager.SetCurItemInUse(this, slotId);
                uiManager.ToggleItemDialog(true);
            }
        }
    }
}