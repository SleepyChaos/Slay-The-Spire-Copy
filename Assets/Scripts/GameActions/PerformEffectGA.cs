using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformEffectGA : GameAction
{
    public Effect Effect { get; set; }
    public List<CombatantView> Targets { get; set; }
    
    public PerformEffectGA(Effect effect, List<CombatantView> targets)
    {
        Effect = effect;
        Targets = targets == null ? null : targets;
        Debug.Log($"创建执行效果事件：{effect.GetType().Name}，目标数：{(targets == null ? 0 : targets.Count)}");
    }
}
