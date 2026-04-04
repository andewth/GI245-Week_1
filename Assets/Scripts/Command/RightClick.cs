using System.IO.Compression;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using UVector3 = UnityEngine.Vector3;
using SVector3 = System.Numerics.Vector3;
using System.Collections.Generic;



public class RightClick : MonoBehaviour
{
    public static RightClick instance;
    public LayerMask layerMask;

    private Camera cam;



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


    void CommandToWalk(RaycastHit hit, List<Character> heros)
    {
        foreach (Character c in heros)
        {
            c.WalkPosition(hit.point);
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
                    CommandToWalk(hit, PartyManager.instance.SelectChars);
                    break;

                case "Enemy":
                    CommandToAttack(hit, PartyManager.instance.SelectChars);
                    break;
                
                case "NPC":
                    CommandTalkToNPC(hit, PartyManager.instance.SelectChars);
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

    
    private void CommandToAttack(RaycastHit hit, List<Character> heros)
    {
        Character target = hit.collider.GetComponent<Character>();
        Debug.Log("Attack: " + target);

        foreach (Character h in heros)
        {
            h.ToAttackCharacter(target);
        }
    }


    private void CommandTalkToNPC(RaycastHit hit, List<Character> heroes)
    {
        Character npc = hit.collider.GetComponent<Character>();
        Debug.Log("Talk to NPC: " + npc);

        if (heroes.Count <= 0)
            return;

        heroes[0].ToTalkToNPC(npc);
    }
    

}
