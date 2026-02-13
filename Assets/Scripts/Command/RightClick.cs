using System.IO.Compression;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using UVector3 = UnityEngine.Vector3;
using SVector3 = System.Numerics.Vector3;



public class RightClick : MonoBehaviour
{
    public static RightClick instance;
    public LayerMask layerMask;

    private Camera cam;


    private LeftClick leftClick;


    void Awake() {
        leftClick = GetComponent<LeftClick>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        cam = Camera.main;
        layerMask = LayerMask.GetMask("Ground", "Building", "Character");
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            TryCommand(Input.mousePosition);
        }
    }


    void CommandToWalk(RaycastHit hit, Character curChar)
    {
        if (curChar != null)
        {
            curChar.WalkPosition(hit.point);
        }

        CreateVFX(hit.point, VFXManager.Instance.DoubleRingMarker);
    }


    void TryCommand(UnityEngine.Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, layerMask))
        {
            switch (hit.collider.tag)
            {
                case "Ground":
                    CommandToWalk(hit, leftClick.CurChar);
                    break;

                case "Enemy":
                    CommandToAttack(hit, leftClick.CurChar);
                    break;
            }
        }
    }


    private void CreateVFX(UVector3 pos, GameObject vfxPrefab)
    {
        if (vfxPrefab != null)
        {
            Instantiate(vfxPrefab, pos, Quaternion.identity);
        }
    }

    
    private void CommandToAttack(RaycastHit hit, Character c)
    {
        if (c == null)
            return;

        Character target = hit.collider.GetComponent<Character>();
        Debug.Log("Attack: " + target);

        if (target != null)
            c.ToAttackCharacter(target);
    }
    

}
