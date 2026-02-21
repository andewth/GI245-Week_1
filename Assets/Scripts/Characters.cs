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
    Die
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
    protected GameObject ringSelection;
    public GameObject RingSelection { get { return ringSelection; } }

    [SerializeField] protected int curHP = 10;
    public int CurHP { get { return curHP; } }



    [SerializeField] protected int attackDamage = 3;


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

    protected VFXManager vfxManager;
    protected UIManager uiManager;



    private void Awake() 
    {
        navAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }


    public void charInit(VFXManager vfxM, UIManager uiM)
    {
        vfxManager = vfxM;
        uiManager = uiM;
    }


    public void SetState(CharState newState)
    {

        if (state == newState)
            return;

        state = newState;

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
        if (curHP <= 0 || state == CharState.Die)
            return;

        curCharTarget = target;

        navAgent.SetDestination(target.transform.position);
        navAgent.isStopped = false;

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
        transform.LookAt(curCharTarget.transform);
        
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
            navAgent.SetDestination(curCharTarget.transform.position);
            navAgent.isStopped = false;
        }
    }


    protected virtual IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }


    protected void Die()
    {
        
        navAgent.isStopped = true;
        SetState(CharState.Die);

        anim.SetTrigger("Die");
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
        if (curHP <= 0 || state == CharState.Die)
            return;

        curHP -= damage;
        if (curHP <= 0)
        {
            curHP = 0;
            Die();
        }
    }


    protected void AttackLogic()
    {
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
        Character target = curCharTarget.GetComponent<Character>();

        if (target != null)
            target.ReceiveDamage(magic.Power);
    }


    private IEnumerator ShootMagicCast(Magic curMagicCast)
    {
        if (vfxManager != null)
            vfxManager.ShootMagic(curMagicCast.ShootID,
                                transform.position,
                                curCharTarget.transform.position,
                                curMagicCast.ShootTime);

        yield return new WaitForSeconds(curMagicCast.ShootTime);

        //cast logic
        MagicCastLogic(curMagicCast);
        isMagicMode = false;

        SetState(CharState.Idle);

        if (uiManager != null)
           uiManager.IsOnCurToggleMagic(false); 
    }


    private IEnumerator LoadMagicCast(Magic curMagicCast)
    {
        if (vfxManager != null)
            vfxManager.LoadMagic(curMagicCast.LoadID,
                                transform.position,
                                curMagicCast.LoadTime);

        yield return new WaitForSeconds(curMagicCast.LoadTime);

        StartCoroutine(ShootMagicCast(curMagicCast));
    }


    private void MagicCast(Magic curMagicCast)
    {
        transform.LookAt(curCharTarget.transform);
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

        navAgent.SetDestination(curCharTarget.transform.position);

        float distance = Vector3.Distance(transform.position,
                                        curCharTarget.transform.position);

        if (distance <= curMagicCast.Range)
        {
            navAgent.isStopped = true;
            SetState(CharState.MagicCast);

            MagicCast(curMagicCast);
        }
    }


}