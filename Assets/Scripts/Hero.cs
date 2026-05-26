using UnityEngine;

public class Hero : Character
{
    [SerializeField]
    private int exp;
    public int Exp
    { get { return exp; } set { exp = value; } }

    [SerializeField]
    private int level;
    public int Level
    { get { return level; } set { level = value; } }

    [SerializeField]
    private int strength;
    public int Strength
    { get { return strength; } set { strength = value; } }

    [SerializeField]
    private int dexterity;
    public int Dexterity
    { get { return dexterity; } set { dexterity = value; } }

    [SerializeField]
    private int constitution;
    public int Constitution
    { get { return constitution; } set { constitution = value; } }

    [SerializeField]
    private int intelligence;
    public int Intelligence
    { get { return intelligence; } set { intelligence = value; } }

    [SerializeField]
    private int wisdom;
    public int Wisdom
    { get { return wisdom; } set { wisdom = value; } }

    [SerializeField]
    private int charisma;
    public int Charisma
    { get { return charisma; } set { charisma = value; } }


    [SerializeField] private int nextExp;
    public int NextExp
    { get { return nextExp; } set { nextExp = value; } }


    [SerializeField] private int prefabId;
    public int PrefabID
    { get { return prefabId; } set { prefabId = value; } }





    void Start()
    {
        if (invManager == null)
        {
            CharInit(VFXManager.Instance, UIManager.instance, InventoryManager.instance, PartyManager.instance);
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

            case CharState.Attack:
                AttackUpdate();
                break;

            case CharState.WalkToMagicCast:
                WalkToMagicCastUpdate();
                break;

            case CharState.WalkToNPC:
                WalkToNPCUpdate();
                break;
        }
    }


    protected void WalkToNPCUpdate()
    {
        if (curCharTarget == null)
        {
            SetState(CharState.Idle);
            return;
        }

        float distance = Vector3.Distance(transform.position,
            curCharTarget.transform.position);

        if (distance <= 2f)
        {
            if (navAgent != null)
                navAgent.isStopped = true;
            SetState(CharState.Idle);

            Npc npc = curCharTarget.GetComponent<Npc>();

            if (uiManager != null)
            {
                if (npc != null)
                {
                    if (npc.IsShopKeeper)
                        uiManager.PrepareShopPanel(npc, this);
                    else
                        uiManager.PrepareDialogueBox(npc);
                }
                else
                {
                    Hero hero = curCharTarget.GetComponent<Hero>();
                    uiManager.PrepareHeroJoinParty(hero);
                }
            }
        }
    }


    public void SaveItemInInventory(Item item)
    {
        for (int i = 0; i < 16; i++)
        {
            if (InventoryItems[i] == null)
            {
                InventoryItems[i] = item;
                return;
            }
        }
    }


    public void ReceiveExp(int n)
    {
        exp += n;
        CheckLevel(exp);
    }

    
    private void UpdateStat()
    {
        attackDamage++;
        defensePower++;
        maxHp++;

        //bonus
        if (strength >= Random.Range(1, 20))
            attackDamage++;

        if (dexterity >= Random.Range(1, 20))
            defensePower++;

        if (constitution >= Random.Range(1, 20))
            maxHp++;
    }


    private void CheckLevel(int exp)
    {
        nextExp = level * 30;

        if (exp >= nextExp)
        {
            level++;
            nextExp = level * 30;
            UpdateStat();

            if (VFXManager.Instance != null && VFXManager.Instance.MagicData != null)
            {
                switch (level)
                {
                    case 3:
                        if (VFXManager.Instance.MagicData.Length > 0)
                            magicSkills.Add(new Magic(VFXManager.Instance.MagicData[0]));
                        if (uiManager != null)
                            uiManager.ShowMagicToggles();
                        break;

                    case 5:
                        if (VFXManager.Instance.MagicData.Length > 1)
                            magicSkills.Add(new Magic(VFXManager.Instance.MagicData[1]));
                        if (uiManager != null)
                            uiManager.ShowMagicToggles();
                        break;

                    case 7:
                        if (VFXManager.Instance.MagicData.Length > 2)
                            magicSkills.Add(new Magic(VFXManager.Instance.MagicData[2]));
                        if (uiManager != null)
                            uiManager.ShowMagicToggles();
                        break;

                    case 10:
                        if (VFXManager.Instance.MagicData.Length > 3)
                            magicSkills.Add(new Magic(VFXManager.Instance.MagicData[3]));
                        if (uiManager != null)
                            uiManager.ShowMagicToggles();
                        break;
                }
            }
        }
    }

}
