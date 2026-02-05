using System.Numerics;
using UnityEngine;

public class LeftClick : MonoBehaviour
{
    private Camera cam;


    [SerializeField] private Character curChar;
    public Character CurChar { get { return curChar; } }


    [SerializeField] private LayerMask layerMask;

    public static LeftClick instance;

    void Start()
    {
        instance = this;
        cam = Camera.main;
        layerMask = LayerMask.GetMask("Ground", "Character", "Building", "Item");
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TrySelect(Input.mousePosition);
        }
    }


    void SelectCharacter(RaycastHit hit)
    {
        curChar = hit.collider.GetComponent<Character>();   
    }

    void TrySelect(UnityEngine.Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, layerMask))
        {
            switch (hit.collider.tag)
            {
                case "Player":
                case "Hero":
                    SelectCharacter(hit);
                    break;
            }
        }
    }
}
