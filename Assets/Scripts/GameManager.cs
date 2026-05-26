using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] heroPrefabs;
    public GameObject[] HeroPrefabs { get { return heroPrefabs; } }

    public static GameManager instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (Settings.isNewGame)
        {
            Settings.isNewGame = false;
            GeneratePlayerHero();
            AudioManager.instance.PlayBGM(1);
        }

        if (Settings.isWarping)
        {
            Settings.isWarping = false;
            WarpPlayers();
        }
    }

    private void GeneratePlayerHero()
    {
        int i = Settings.playerPrefabId;

        if (i < 0 || i >= heroPrefabs.Length)
            i = 0;

        GameObject heroObj = Instantiate(heroPrefabs[i], 
            new Vector3(46f, 10f, 38f), Quaternion.identity);

        heroObj.tag = "Player";

        Character hero = heroObj.GetComponent<Character>();
        Hero playerHero = heroObj.GetComponent<Hero>();
        playerHero.PrefabID = i;
        PartyManager.instance.Members.Add(hero);

        hero.CharInit(VFXManager.Instance, UIManager.instance, InventoryManager.instance, PartyManager.instance);
        
        if (InventoryManager.instance != null && InventoryManager.instance.ItemData != null && InventoryManager.instance.ItemData.Length > 0)
        {
            int slot0 = InventoryManager.instance.AddItem(hero, 0);
            int slot2 = InventoryManager.instance.AddItem(hero, 2);
            Debug.Log($"Player item 0 added at slot: <color=blue>{slot0}</color>, item 2 at slot: <color=blue>{slot2}</color>");
        }
        else
        {
            Debug.LogError("InventoryManager or ItemData is not set up! Cannot add starting items.");
        }

        UIManager.instance.MapToggleAvatar();
        PartyManager.instance.SelectSingleHero(0);

    }


    void WarpPlayers()
    {
        PartyManager.instance.LoadAllHeroData();
        Settings.nextWarpTime = Time.time + 1f;
        Settings.isChangingMap = false;
    }
}