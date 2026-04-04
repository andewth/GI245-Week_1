using UnityEngine;
using System.Collections.Generic; // List [Need]

public class PartyManager : MonoBehaviour
{
    [SerializeField] private List<Character> members = new List<Character>();
    public List<Character> Members { get { return members; } }


    [SerializeField] private List<Character> selectChars = new List<Character>();
    public List<Character> SelectChars { get { return selectChars; } }

    [SerializeField]
    private List<Quest> questList = new List<Quest>();
    public List<Quest> QuestList { get { return questList; } }

    public static PartyManager instance;


    void Awake() 
    {
        instance = this;
    }


    void Start()
    {
        foreach (Character c in members)
        {
            c.CharInit(VFXManager.Instance, UIManager.instance, InventoryManager.instance);
            // c.MagicSkills.Add(new Magic(0, "Fireball", 10f, 30, 3f, 1f, 0, 1));  // Skill 1
        }

        SelectSingleHero(0);

        // members[0].MagicSkills.Add(new Magic(0, "Power Glow", 10f, 20, 3f, 1f, 2, 2));  // Skill 2
        // members[0].MagicSkills.Add(new Magic(1, "Fire Explosion", 10f, 20, 3f, 1f, 1, 3));   // Skill 3
        // members[0].MagicSkills.Add(new Magic(2, "Experien Gain", 10f, 20, 3f, 1f, 2, 4));  // Skill 4

        // members[1].MagicSkills.Add(new Magic(0, "Power Glow", 10f, 20, 3f, 1f, 2, 2));   // Skill 2
        // members[1].MagicSkills.Add(new Magic(1, "Electric", 10f, 20, 3f, 1f, 0, 5));    // Skill 3
        // members[1].MagicSkills.Add(new Magic(2, "Firework", 10f, 20, 3f, 1f, 0, 6));  // Skill 4


        members[0].MagicSkills.Add(new Magic(VFXManager.Instance.MagicData[0]));
        members[1].MagicSkills.Add(new Magic(VFXManager.Instance.MagicData[1]));

        InventoryManager.instance.AddItem(members[0], 0);   // Heal potion
        InventoryManager.instance.AddItem(members[0], 1);   // Sword
        InventoryManager.instance.AddItem(members[0], 2);   // Sword
        InventoryManager.instance.AddItem(members[0], 3);   // Sword
        InventoryManager.instance.AddItem(members[0], 4);   // Sword
        InventoryManager.instance.AddItem(members[0], 5);   // Sword
        InventoryManager.instance.AddItem(members[0], 6);   // Sword
        InventoryManager.instance.AddItem(members[0], 7);   // Sword
        InventoryManager.instance.AddItem(members[0], 8);   // Sword
        InventoryManager.instance.AddItem(members[0], 9);   // Sword

        InventoryManager.instance.AddItem(members[1], 0);   // Heal potion
        InventoryManager.instance.AddItem(members[1], 1);   // Sword
        InventoryManager.instance.AddItem(members[1], 2);   // Shield
        InventoryManager.instance.AddItem(members[1], 3);   // Shield


        UIManager.instance.ShowMagicToggles();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (selectChars.Count > 0)
            {
                selectChars[0].IsMagicMode = true;
                selectChars[0].CurMagicCast = selectChars[0].MagicSkills[0];
            }
        }
    }


    public void SelectSingleHero(int i)
    {
        foreach (Character c in selectChars)
            c.ToggleRingSelection(false);

        selectChars.Clear();

        selectChars.Add(members[i]);
        selectChars[0].ToggleRingSelection(true);
    }


    public void HeroSelectMagicSkill(int i)
    {
        if (selectChars.Count <= 0)
            return;

        selectChars[0].IsMagicMode = true;
        selectChars[0].CurMagicCast = selectChars[0].MagicSkills[i];
    }

    



}
