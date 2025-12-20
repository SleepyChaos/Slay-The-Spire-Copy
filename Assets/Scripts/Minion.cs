using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : MonoBehaviour
{
    private int attack;
    private int health;
    
    private void OnEnable()
    {
        ActionSystem.SubscribeReaction<DealDamageGA>(DealDamageReaction, ReactionTiming.POST);
    }

    private void OnDisable()
    {
        ActionSystem.UnsubscribeReaction<DealDamageGA>(DealDamageReaction, ReactionTiming.POST);
    }

    private void DealDamageReaction(DealDamageGA dealDamageGA)
    {
        Debug.Log($"随从被动：受到伤害事件后触发加成 +{dealDamageGA.Amount}");
        IncreaseStatsGA increaseStatsGA = new IncreaseStatsGA(this, dealDamageGA.Amount, dealDamageGA.Amount);
        ActionSystem.Instance.AddReaction(increaseStatsGA);
    }

    public IEnumerator IncreaseAttackAndHealth(int attack, int health)
    {
        Debug.Log($"随从属性提升：攻击 +{attack} 生命 +{health}");
        this.attack += attack;
        this.health += health;
        //attackText.text = this.attack.ToString();
        yield return null;
    }
}
