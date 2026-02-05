using System.Numerics;
using UnityEngine;
using UnityEngine.AI;

public enum CharState
{
    Idle,
    Walk,
    Attack,
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


    private void Awake() {
        navAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
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


    public void WalkPosition(Vector3 position)
    {
        if (navAgent == null)
            return;

        navAgent.isStopped = false;
        navAgent.SetDestination(position);
        SetState(CharState.Walk);
    }


    protected void WalkUpdate()
    {
        float distance = Vector3.Distance(transform.position, navAgent.destination);
        if (distance <= navAgent.stoppingDistance)
        {
            SetState(CharState.Idle);
        }
    }
    
}