using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEnemyAttackCondition : PerkCondition
{
    public override void SubscribeCondition(Action<GameAction> reaction)
    {
        Debug.Log("天赋条件：订阅敌人攻击事件");
        ActionSystem.SubscribeReaction<AttackHeroGA>(reaction, reactionTiming);   
    }

    public override void UnsubscribeCondition(Action<GameAction> reaction)
    {
        ActionSystem.UnsubscribeReaction<AttackHeroGA>(reaction, reactionTiming);
    }

    public override bool SubConditionIsMet(GameAction gameAction)
    {
        return true;
    }
}
