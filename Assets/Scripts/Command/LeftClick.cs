using System.Numerics;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class LeftClick : MonoBehaviour
{
    private Camera cam;

    [SerializeField] private LayerMask layerMask;


    [SerializeField] private RectTransform boxSelection;
    private UnityEngine.Vector2 oldAnchoredPos;
    private UnityEngine.Vector2 startPos;


    public static LeftClick instance;

    void Start()
    {
        instance = this;
        cam = Camera.main;
        layerMask = LayerMask.GetMask("Ground", "Character", "Building", "Item");

        boxSelection = UIManager.instance.SelectionBox;
    }


    void Update()
    {
        // mouse down (เมื่อเริ่มคลิก)
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            startPos = Mouse.current.position.value;

            //if click UI, don't clear (ถ้าคลิกโดน UI ไม่ต้องทำอะไร)
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            // ClearEverything();
        }

        // mouse hold down (เมื่อกดค้างเพื่อลาก)
        if (Mouse.current.leftButton.isPressed)
        {
            //if click UI, don't check
            // if (EventSystem.current.IsPointerOverGameObject())
            //     return;

            UpdateSelectionBox(Mouse.current.position.value);
        }

        // mouse up (เมื่อปล่อยเมาส์)
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            bool selectedByBox = ReleaseSelectionBox(Mouse.current.position.value);

            if (!selectedByBox)
                TrySelect(Mouse.current.position.value);
        }
    }


    private int SelectCharacter(RaycastHit hit)
    {
        ClearEverything();

        Character hero = hit.collider.GetComponent<Character>();
        //Debug.Log("Selected Char: " + hit.collider.gameObject);

        int i = PartyManager.instance.FindIndexFromClass(hero);
        //Debug.Log($"Click Release: {i}");
        if (i == -1)
            return i;

        UIManager.instance.ToggleAvatar[i].isOn = true;
        PartyManager.instance.SelectSingleHero(i);
        return i;
    }

    
    private void TrySelect(UnityEngine.Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        RaycastHit hit;

        int i = -1; // เปลี่ยนเป็น -1 เพื่อให้รู้ว่า "ยังไม่ได้เลือกฮีโร่ตัวไหนเลย"

        if (Physics.Raycast(ray, out hit, 1000, layerMask))
        {
            switch (hit.collider.tag)
            {
                case "Player":
                case "Hero":
                    i = SelectCharacter(hit);
                    break;
                case "Item":
                    SelectItem(hit);
                    break;
            }
        }

        // ถ้าไม่มีฮีโร่ตัวไหนถูกเลือกอยู่เลยในตอนนี้
        if (PartyManager.instance.SelectChars.Count == 0)
        {
            // ถ้าคลิกโดนพื้น (i ยังเป็น -1) ให้ fallback ไปเลือกตัวที่ 0 เสมอ
            if (i == -1) 
            {
                i = 0;
            }

            // ดักเช็คก่อนเซ็ตค่า ป้องกัน error กรณี ToggleAvatar มีไม่ถึง index นั้น
            if (i >= 0 && i < UIManager.instance.ToggleAvatar.Length)
            {
                UIManager.instance.ToggleAvatar[i].isOn = true;
            }
        }
    }

    void ClearRingSelection()
    {
        foreach (Character hero in PartyManager.instance.SelectChars)
        {
            hero.ToggleRingSelection(false);
        }
    }


    private void ClearEverything()
    {
        foreach (Toggle t in UIManager.instance.ToggleAvatar)
        {
            t.isOn = false;
        }

        ClearRingSelection();
        PartyManager.instance.SelectChars.Clear();
    }


    private void UpdateSelectionBox(UnityEngine.Vector2 mousePos)
    {
        //Debug.Log("Mouse Pos - " + mousePos);
        if (!boxSelection.gameObject.activeInHierarchy)
            boxSelection.gameObject.SetActive(true);

        float width = mousePos.x - startPos.x;
        float height = mousePos.y - startPos.y;

        boxSelection.anchoredPosition = startPos + new UnityEngine.Vector2(width / 2, height / 2);

        width = Mathf.Abs(width);
        height = Mathf.Abs(height);

        boxSelection.sizeDelta = new UnityEngine.Vector2(width, height);

        //store old position for real unit selection
        oldAnchoredPos = boxSelection.anchoredPosition;
    }


    private bool ReleaseSelectionBox(UnityEngine.Vector2 mousePos)
    {
        //Debug.Log("Step 2 - " + Release Mouse);
        UnityEngine.Vector2 corner1; //down-left corner
        UnityEngine.Vector2 corner2; //top-right corner

        boxSelection.gameObject.SetActive(false);

        corner1 = oldAnchoredPos - (boxSelection.sizeDelta / 2);
        corner2 = oldAnchoredPos + (boxSelection.sizeDelta / 2);


        bool anyNewCharSelect = false;

        foreach (Character member in PartyManager.instance.Members)
        {
            UnityEngine.Vector2 unitPos = cam.WorldToScreenPoint(member.transform.position);

            if ((unitPos.x > corner1.x && unitPos.x < corner2.x)
                && (unitPos.y > corner1.y && unitPos.y < corner2.y))
            {
                if (!anyNewCharSelect) 
                {
                    anyNewCharSelect = true;
                    ClearEverything();
                }

                int i = PartyManager.instance.FindIndexFromClass(member);
                if (i == -1)
                    continue;

                UIManager.instance.ToggleAvatar[i].isOn = true;
                PartyManager.instance.SelectSingleHeroByToggle(i);

                // PartyManager.instance.SelectChars.Add(member);
                // member.ToggleRingSelection(true);
            }
        }
        boxSelection.sizeDelta = new UnityEngine.Vector2(0, 0); //clear Selection Box's size;
        return anyNewCharSelect;
    }


    private void SelectItem(RaycastHit hit)
    {
        ItemPick itemPick = hit.collider.GetComponent<ItemPick>();
        //Debug.Log("Pick Item: " + itemPick.Item.ItemName);

        if (PartyManager.instance.SelectChars.Count == 0)
            UIManager.instance.ToggleAvatar[0].isOn = true;

        if (itemPick != null)
        {
            itemPick.PickUpItem();
        }
    }
}
