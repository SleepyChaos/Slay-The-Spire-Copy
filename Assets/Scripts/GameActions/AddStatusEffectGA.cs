using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddStatusEffectGA : GameAction
{
    public StatusEffectType StatusEffectType{get; private set;}
    public int StackCount{get; private set;}
    public List<CombatantView>Targets{get; private set;}

    
    public AddStatusEffectGA(StatusEffectType statusEffectType, int stackCount, List<CombatantView> targets)
    {
        StatusEffectType = statusEffectType;
        StackCount = stackCount;
        Targets = targets;
        Debug.Log($"创建添加状态事件：{statusEffectType} 层数 {stackCount}，目标数：{targets.Count}");
    }
}
