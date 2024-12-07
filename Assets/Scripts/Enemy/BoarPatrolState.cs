using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class BoarPatrolState : BaseState
{


    public override void OnEnter(Enemy enemy)
    {
        //����� currentEnemy �����ڸ���
        currentEnemy = enemy;

        currentEnemy.currentSpeed = currentEnemy.normalSpeed;
    }
    public override void LogicUpdate()
    {
        if (currentEnemy.FoundPlayer())
        {
            currentEnemy.SwitchState(NPCState.Chase);
        }



        //ת����ʱ�� �ᴥ������һ��ǽ�ļ�� ,����Ҫ���ϳ����ж�����
        if (!currentEnemy.physicsCheck.isGround||(currentEnemy.physicsCheck.touchLeftWall && currentEnemy.faceDir.x < 0) || (currentEnemy.physicsCheck.touchRightWall && currentEnemy.faceDir.x > 0))
        {
            currentEnemy.wait = true;
            currentEnemy.anim.SetBool("walk", false);
        }
        else
        {
            currentEnemy.anim.SetBool("walk", true);
        }

    }


    public override void PhysicsUpdate()
    {
        throw new System.NotImplementedException();
    }


    public override void OnExit()
    {
        currentEnemy.anim.SetBool("walk", false);
        Debug.Log("Exit");
    }

}
