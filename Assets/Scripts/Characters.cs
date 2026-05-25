using System.Numerics;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using Vector3 = UnityEngine.Vector3;
using UVector3 = UnityEngine.Vector3;
using SVector3 = System.Numerics.Vector3;

public enum CharState
{
    Idle,
    Walk,
    WalkToEnemy,
    Attack,
    WalkToMagicCast,
    MagicCast,
    Hit,
    Die,
    WalkToNPC
}


public abstract class Character : MonoBehaviour
{
    protected NavMeshAgent navAgent;

    protected Animator anim;
    public Animator Anim { get { return anim; } }

    [SerializeField]
    protected CharState state;
    public CharState State { get { return state; } }


    [SerializeField]
    protected Sprite avatarPic;
    public Sprite AvatarPic { get { return avatarPic; } }

    [SerializeField]
    protected string charName;
    public string CharName { get { return charName; } }


    [SerializeField]
    protected GameObject ringSelection;
    public GameObject RingSelection { get { return ringSelection; } }

    [SerializeField] protected int curHP = 10;
    public int CurHP { get { return curHP; } set { curHP = value; } } 

    [SerializeField] protected int maxHp = 100;
    public int MaxHP { get { return maxHp; } }



    [SerializeField] protected int attackDamage = 3;
    public int AttackDamage { get { return attackDamage; } set { attackDamage = value; } }


    [SerializeField] protected float attackRange = 2f;


    [SerializeField] protected float attackCooldown = 2f;



    [SerializeField] protected float attackTimer = 0f;


    [SerializeField] protected float findingRange = 20f;
    public float FindingRange { get { return findingRange; } }


    [SerializeField]
    protected Character curCharTarget;
    public Character CurCharTarget { get { return curCharTarget; } set { curCharTarget = value; } }


    [SerializeField]
    protected List<Magic> magicSkills = new List<Magic>();
    public List<Magic> MagicSkills
    { get { return magicSkills; } set { magicSkills = value; } }

    [SerializeField]
    protected Magic curMagicCast = null;
    public Magic CurMagicCast
    { get { return curMagicCast; } set { curMagicCast = value; } }

    [SerializeField]
    protected bool isMagicMode = false;
    public bool IsMagicMode
    { get { return isMagicMode; } set { isMagicMode = value; } }


    [Header("Inventory")]
    [SerializeField] protected Item[] inventoryItems;
    public Item[] InventoryItems
    { get { return inventoryItems; } set { inventoryItems = value; } }


    [SerializeField] protected Item mainWeapon;
    public Item MainWeapon => mainWeapon;


    [SerializeField] protected Item shield;
    public Item Shield => shield;


    protected VFXManager vfxManager;
    protected UIManager uiManager;

    protected InventoryManager invManager;


    [SerializeField] protected Transform shieldHand;

    [SerializeField] protected GameObject shieldObj;


    [SerializeField] protected Transform weaponHand;

    [SerializeField] protected GameObject weaponObj;

    protected PartyManager partyManager;



    [SerializeField] protected int defensePower = 0;
    public int DefensePower { get { return defensePower; } set { defensePower = value; } }



    private void Awake() 
    {
        navAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }


    public void CharInit(VFXManager vfxM, UIManager uiM, InventoryManager invM, PartyManager partyM)
    {
        vfxManager = vfxM;
        uiManager = uiM;
        invManager = invM;
        partyManager = partyM;

        if (inventoryItems == null || inventoryItems.Length == 0)
            inventoryItems = new Item[InventoryManager.MAXSLOT];
    }


    public void SetState(CharState newState)
    {

        if (state == newState)
            return;

        state = newState;

        if (navAgent == null)
            return;

        if (state == CharState.Idle)
        {
            navAgent.isStopped = true;
            navAgent.ResetPath();
        }
        else if (state == CharState.Walk)
        {
            navAgent.isStopped = false;
        }
    }


    public void WalkPosition(UnityEngine.Vector3 dest) // ระบุชื่อเต็มหน้า Vector3
    {
        if (navAgent != null)
        {
            navAgent.isStopped = false;
            navAgent.SetDestination(dest);
        }

        SetState(CharState.Walk);
    }


    protected void WalkUpdate()
    {
        if (navAgent == null)
            return;

        float distance = UnityEngine.Vector3.Distance(transform.position, navAgent.destination);
        if (distance <= navAgent.stoppingDistance)
        {
            SetState(CharState.Idle);
        }
    }


    public void ToggleRingSelection(bool isActive)
    {
        if (ringSelection != null)
        {
            ringSelection.SetActive(isActive);
        }
    }


    public void ToAttackCharacter(Character target)
    {
        if (curHP <= 0 || state == CharState.Die || target == null)
            return;

        curCharTarget = target;

        if (navAgent != null)
        {
            navAgent.SetDestination(target.transform.position);
            navAgent.isStopped = false;
        }

        if (isMagicMode)
        {
            SetState(CharState.WalkToMagicCast);
        } else {
            SetState(CharState.WalkToEnemy);
        }
    }


    protected void WalkToEnemyUpdate()
    {
        if (curCharTarget == null)
        {
            SetState(CharState.Idle);
            return;
        }

        if (navAgent != null)
            navAgent.SetDestination(curCharTarget.transform.position);

        float distance = Vector3.Distance(transform.position, 
                                        curCharTarget.transform.position);

        if (distance <= attackRange)
        {
            SetState(CharState.Attack);
            Attack();
            // [ส่วนที่ถูกขีดฆ่า] //First Attack
        }
    }

    protected void Attack()
    {
        if (curCharTarget == null)
            return;

        transform.LookAt(curCharTarget.transform);
        
        if (anim != null)
            anim.SetTrigger("Attack");

        AttackLogic();
    }


   protected void AttackUpdate()
    {
        if (curCharTarget == null)
            return;

        if (curCharTarget.CurHP <= 0)
        {
            SetState(CharState.Idle);
            return;
        }
        if (navAgent != null)
            navAgent.isStopped = true;

        attackTimer += Time.deltaTime;
        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;
            Attack();
        }

        float distance = Vector3.Distance(transform.position, 
                                        curCharTarget.transform.position);

        if (distance > attackRange)
        {
            SetState(CharState.WalkToEnemy);
            if (navAgent != null)
            {
                navAgent.SetDestination(curCharTarget.transform.position);
                navAgent.isStopped = false;
            }
        }
    }


    protected virtual IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }


    protected virtual void Die()
    {
        if (navAgent != null)
            navAgent.isStopped = true;

        SetState(CharState.Die);

        if (anim != null)
            anim.SetTrigger("Die");

        if (inventoryItems != null)
        {
            InventoryManager manager = invManager ?? InventoryManager.instance;
            if (manager != null)
                manager.SpawnDropInventory(inventoryItems, transform.position);
        }

        StartCoroutine(DestroyObject());
    }

    // public void ReceiveDamage(Character enemy)
    // {
    //     if (curHP <= 0 || state == CharState.Die)
    //         return;

    //     curHP -= enemy.attackDamage;
    //     if (curHP <= 0)
    //     {
    //         curHP = 0;
    //         Die();
    //     }
    // }

    public void ReceiveDamage(int damage)
    {
        if (CurHP <= 0 || state == CharState.Die)
            return;

        int damageAfter = damage - defensePower;

        if (damageAfter < 0)
            damageAfter = 0;

        curHP -= damageAfter;

        if (curHP <= 0)
        {
            curHP = 0;
            Die();
        }
    }

    protected void AttackLogic()
    {
        if (curCharTarget == null)
            return;

        Character target = curCharTarget.GetComponent<Character>();
        
        if (target != null)
        {
            target.ReceiveDamage(attackDamage);
        }
    }


    public bool IsMyEnemy(string targetTag)
    {
        string myTag = gameObject.tag;

        if ((myTag == "Hero" || myTag == "Player") && targetTag == "Enemy")
            return true;

        if (myTag == "Enemy" && (targetTag == "Hero" || targetTag == "Player"))
            return true;

        return false;
    }


    protected void MagicCastLogic(Magic magic)
    {
        if (curCharTarget == null)
            return;

        Character target = curCharTarget.GetComponent<Character>();

        if (target != null)
            target.ReceiveDamage(magic.Power);
    }


    private IEnumerator ShootMagicCast(Magic curMagicCast)
    {
        if (vfxManager != null && curCharTarget != null)
        {
            Vector3 chestPosition = curCharTarget.transform.position + new Vector3(0, 1.2f, 0);

            vfxManager.ShootMagic(curMagicCast.ShootID,
                                    transform.position,
                                    chestPosition,
                                    curMagicCast.ShootTime);
        }

        if (curMagicCast != null)
            yield return new WaitForSeconds(curMagicCast.ShootTime);

        //cast logic
        if (curMagicCast != null)
            MagicCastLogic(curMagicCast);
        isMagicMode = false;

        SetState(CharState.Idle);

        if (uiManager != null)
           uiManager.IsOnCurToggleMagic(false); 
    }


    private IEnumerator LoadMagicCast(Magic curMagicCast)
    {
        if (curMagicCast == null)
            yield break;

        if (vfxManager != null)
            vfxManager.LoadMagic(curMagicCast.LoadID,
                                transform.position,
                                curMagicCast.LoadTime);

        yield return new WaitForSeconds(curMagicCast.LoadTime);

        StartCoroutine(ShootMagicCast(curMagicCast));
    }


    private void MagicCast(Magic curMagicCast)
    {
        if (curCharTarget != null)
            transform.LookAt(curCharTarget.transform);

        if (anim != null)
            anim.SetTrigger("MagicAttack");

        StartCoroutine(LoadMagicCast(curMagicCast));
    }


    protected void WalkToMagicCastUpdate()
    {
        if (curCharTarget == null || curMagicCast == null)
        {
            SetState(CharState.Idle);
            return;
        }

        if (navAgent != null)
            navAgent.SetDestination(curCharTarget.transform.position);

        float distance = Vector3.Distance(transform.position,
                                        curCharTarget.transform.position);

        if (distance <= curMagicCast.Range)
        {
            if (navAgent != null)
                navAgent.isStopped = true;
            SetState(CharState.MagicCast);

            MagicCast(curMagicCast);
        }
    }


    public void Recover(int n)
    {
        curHP += n;

        if (curHP > MaxHP)
        {
            curHP = MaxHP;
        }
    }


    public void EquipShield(Item item)
    {
        if (invManager == null || item == null || shieldHand == null)
            return;

        if (item.PrefabID < 0 || item.PrefabID >= invManager.ItemPrefabs.Length)
            return;

        shieldObj = Instantiate(invManager.ItemPrefabs[item.PrefabID], shieldHand);

        if (shieldObj != null)
        {
            shieldObj.transform.localPosition = new Vector3(0.23f, -0.004f, -0.013f);
            shieldObj.transform.Rotate(-90f, 0f, 180f, Space.Self);
        }

        defensePower += item.Power;
        shield = item;

        Debug.Log("Equip Shield!!");
    }

    public void UnEquipShield()
    {
        if (shield != null)
        {
            defensePower -= shield.Power;
            shield = null;
            Destroy(shieldObj);

            Debug.Log("Exit Shield!!");
        }
    }

    public void EquipWeapon(Item item)
    {
        if (invManager == null || item == null || weaponHand == null)
            return;

        if (item.PrefabID < 0 || item.PrefabID >= invManager.ItemPrefabs.Length)
            return;

        weaponObj = Instantiate(invManager.ItemPrefabs[item.PrefabID], weaponHand);

        if (weaponObj != null)
        {
            weaponObj.transform.localPosition = new Vector3(0.13f, 0.052f, -0.013f);
            weaponObj.transform.Rotate(-10.06f, 86.763f, -90f, Space.Self);
        }

        defensePower += item.Power;
        mainWeapon = item;

        Debug.Log("Equip Weapon!!");
    }

    public void UnEquipWeapon()
    {
        if (mainWeapon != null)
        {
            defensePower -= mainWeapon.Power;
            mainWeapon = null;
            Destroy(weaponObj);

            Debug.Log("Exit Weapon!!");
        }
    }


    // move to NPC
    public void ToTalkToNPC(Character npc)
    {
        if (curHP <= 0 || state == CharState.Die || npc == null)
            return;

        //lock target
        curCharTarget = npc;

        //start walking to enemy
        if (navAgent != null)
        {
            navAgent.SetDestination(npc.transform.position);
            navAgent.isStopped = false;
        }

        SetState(CharState.WalkToNPC);
    }

}