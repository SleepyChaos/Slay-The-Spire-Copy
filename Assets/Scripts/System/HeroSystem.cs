using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSystem : MonoSingleton<HeroSystem>
{
    [field: SerializeField] public HeroView HeroView { get; set; }

    private void OnEnable()
    {
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }

    private void OnDisable()
    {
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }

    public void Setup(HeroData heroData)
    {
        Debug.Log("英雄系统：初始化英雄与回合订阅");
        HeroView.Setup(heroData);
    }
    
    /// <summary>
    /// 敌人回合前事件
    /// </summary>
    /// <param name="enemyTurnGA"></param>
    private void EnemyTurnPreReaction(EnemyTurnGA enemyTurnGA)
    {
        //清空玩家手中所有手牌
        DiscardAllCardGA discardAllCardGA = new DiscardAllCardGA();
        ActionSystem.Instance.AddReaction(discardAllCardGA);
    }
    
    /// <summary>
    /// 敌人回合结束事件
    /// </summary>
    /// <param name="enemyTurnGA"></param>
    private void EnemyTurnPostReaction(EnemyTurnGA enemyTurnGA)
    {
        // 检查 HeroView 是否仍然有效（可能在场景切换时被销毁）
        if (HeroView == null)
        {
            Debug.LogWarning("⚠️ HeroView 已被销毁，跳过敌人回合结束事件（可能是场景正在切换）");
            return;
        }
        
        // 检查 HeroView 的 GameObject 是否仍然有效
        if (HeroView.gameObject == null)
        {
            Debug.LogWarning("⚠️ HeroView.GameObject 已被销毁，跳过敌人回合结束事件");
            return;
        }
        
        Debug.Log("✅ HeroView 仍然有效，继续处理敌人回合结束事件");
        
        //结算负面状态伤害 
        int burnStacks = HeroView.GetStatusEffectStacks(StatusEffectType.BURN);
        if (burnStacks > 0)
        {
            Debug.Log($"🔥 检测到 {burnStacks} 层燃烧，准备应用燃烧伤害");
            ApplyBurnGA applyBurnGA = new ApplyBurnGA(burnStacks, HeroView);
            ActionSystem.Instance.AddReaction(applyBurnGA);
        }
        else
        {
            Debug.Log("✅ 没有燃烧状态，跳过燃烧伤害");
        }
        
        //设置玩家可抽卡数量为5
        DrawCardsGA drawCardsGA = new DrawCardsGA(5);
        ActionSystem.Instance.AddReaction(drawCardsGA);
    }
}
