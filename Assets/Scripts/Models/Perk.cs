using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Perk
{
    public Sprite Image => data.Image;
    private readonly PerkData data;
    private readonly PerkCondition condition;
    private readonly AutoTargetEffect effect;
    
    public Perk(PerkData perkData)
    {
        data = perkData;
        condition = data.Condition;
        effect = data.AutoTargetEffect;
    }

    public void OnAdd()
    {
        condition.SubscribeCondition(Reaction);    
        Debug.Log("天赋激活：开始监听条件");
    }

    public void OnRemove()
    {
        condition.UnsubscribeCondition(Reaction);
    }

    private void Reaction(GameAction gameAction)
    {
        if (condition.SubConditionIsMet(gameAction))
        {
            List<CombatantView> targets = new List<CombatantView>();
            if (data.UseActionCasterAsTarget && gameAction is IHaveCaster hasCaster)
            {
                targets.Add(hasCaster.Caster);
            }
            if (data.UseAutoTarget)
            {
                targets.AddRange(effect.TargetMode.GetTargets());
            }

            GameAction perkEffectAction = effect.Effect.GetGameAction(targets, HeroSystem.Instance.HeroView);
            ActionSystem.Instance.AddReaction(perkEffectAction);
        }
    }
}
