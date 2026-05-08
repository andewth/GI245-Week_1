using UnityEngine;

public class Enemy : Character
{

    [SerializeField] private int expDrop;
    public int ExpDrop { get { return expDrop; } set { expDrop = value; } }


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
        }
    }


    protected override void Die()
    {
        base.Die();
        partyManager.DistributeTotalExp(expDrop);
    }
}
