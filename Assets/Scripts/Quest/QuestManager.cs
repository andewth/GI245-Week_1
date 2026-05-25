using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField]
    private Npc[] npcPerson;
    public Npc[] NPCPerson { get { return npcPerson; } set { npcPerson = value; } }

    [SerializeField]
    private QuestData[] questData;
    public QuestData[] QuestData { get { return questData; } set { questData = value; } }

    [SerializeField]
    private Npc curNpc;
    public Npc CurNPC { get { return curNpc; } set { curNpc = value; } }

    [SerializeField]
    private Quest curQuest;
    public Quest CurQuest { get { return curQuest; } set { curQuest = value; } }

    public static QuestManager instance;

    void Awake()
    {
        instance = this;
    }

    private void AddQuestToNPC(Npc npc, QuestData questData)
    {
        if (npc == null || questData == null)
            return;

        Quest quest = new Quest(questData);
        npc.QuestToGive.Add(quest);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (npcPerson == null)
            return;

        foreach (Character npc in npcPerson)
        {
            if (npc == null)
                continue;

            npc.CharInit(VFXManager.Instance, UIManager.instance, InventoryManager.instance, PartyManager.instance);
        }

        if (npcPerson.Length > 0 && questData != null && questData.Length > 0)
            AddQuestToNPC(npcPerson[0], questData[0]); //Give Golem - Give Potion Quest
    }


    public Quest CheckForQuest(Npc npc, QuestStatus status)
    {
        if (npc == null)
            return null;

        curNpc = npc;

        Quest quest = npc.CheckQuestList(status);
        curQuest = quest;

        return quest;
    }

    private bool CheckItemToDelivery()
    {
        if (curQuest == null)
            return false;

        return InventoryManager.instance.CheckPartyForItem(curQuest.QuestItemId);
    }

    public bool CheckIfFinishQuest()
    {
        if (curQuest == null)
            return false;

        bool success = false;

        Debug.Log(curQuest.Type);

        switch(curQuest.Type)
        {
            case QuestType.Delivery:
                success = CheckItemToDelivery();
                break;
        }
        return success;
    }

    public bool CheckLastDialogue(int i)
    {
        if (curQuest == null || curQuest.QuestDialogue == null)
            return true;

        if (i == curQuest.QuestDialogue.Length - 1)
            return true;
        else
            return false;
    }

    public string NextDialogue(int i) //map with ButtonNext
    {
        if (curQuest == null || curQuest.QuestDialogue == null)
            return "";

        if (i < curQuest.QuestDialogue.Length)
            return curQuest.QuestDialogue[i];
        else
            return "";
    }

    public bool DeliverItem()
    {
        if (curQuest == null)
            return false;

        return InventoryManager.instance.RemoveItemFromParty(curQuest.QuestItemId);
    }


    public bool NpcGiveReward()
    {
        if (curQuest == null)
            return false;

        if (PartyManager.instance.SelectChars.Count == 0)
            return false;

        Character hero = PartyManager.instance.SelectChars[0];

        if (hero == null || curQuest.RewardItemId < 0 || curQuest.RewardItemId >= InventoryManager.instance.ItemData.Length)
            return false;

        Item item = new Item(InventoryManager.instance.ItemData[curQuest.RewardItemId]);

        int count = Mathf.Min(16, hero.InventoryItems.Length);

        for (int i = 0; i < count; i++)
        {
            if(hero.InventoryItems[i] == null)
            {
                hero.InventoryItems[i] = item;
                curQuest.Status = QuestStatus.Finish;
                return true;
            }
        }
        return false;
    }


    public void RejectQuest() //map with ButtonReject
    {
        if (curQuest == null)
            return;

        curQuest.Status = QuestStatus.Reject;
    }

    public void AcceptQuest() //map with ButtonAccept
    {
        if (curQuest == null)
            return;

        curQuest.Status = QuestStatus.InProgress;
        PartyManager.instance.QuestList.Add(curQuest);
    }

}