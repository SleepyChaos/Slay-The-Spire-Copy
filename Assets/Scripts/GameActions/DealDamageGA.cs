using System.Collections.Generic;
using UnityEngine;

public class DealDamageGA : GameAction, IHaveCaster
{
    public int Amount{get;set;}
    public List<CombatantView> Targets{get;set;}
    public CombatantView Caster { get; private set; }

    public DealDamageGA(int amount, List<CombatantView> targets, CombatantView caster)  
    {
        Amount = amount;
        Targets = new List<CombatantView>(targets);
        Caster = caster;
        Debug.Log($"创建伤害事件：对{Targets.Count}个目标造成{Amount}点伤害，施法者：{caster.name}");
    }

}
