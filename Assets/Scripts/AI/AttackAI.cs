using System.Dynamic;
using UnityEngine;

public class AttackAI : MonoBehaviour
{

    Character myChar;

    [SerializeField]
    protected Character curEnemy;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myChar = GetComponent<Character>();

        if (myChar != null)
        {
            InvokeRepeating("FindAndAttackEnemy", 0f, 1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    private void FindAndAttackEnemy()
    {
        if (myChar.CurCharTarget == null)
        {
            // หมายเหตุ: ต้องประกาศตัวแปร curEnemy ไว้ก่อนหน้านี้ หรือใช้ var curEnemy = ...
            curEnemy = Formula.FindClosestEnemyChar(myChar);
            if (curEnemy == null)
                return;

            if (myChar.IsMyEnemy(curEnemy.gameObject.tag))
                myChar.ToAttackCharacter(curEnemy);
        }
    }
}
