using System.Collections.Generic;
using UnityEngine;

public class LifestealGA : GameAction, IHaveCaster
{
    public int DamageAmount { get; private set; }
    public List<CombatantView> Targets { get; private set; }
    public CombatantView Caster { get; private set; } // 谁吸血？

    public LifestealGA(int damageAmount, List<CombatantView> targets, CombatantView caster)
    {
        Debug.Log($"GA接收到{caster} 使用了吸血效果，对{targets}造成 {damageAmount} 点伤害");
        DamageAmount = damageAmount;
        Targets = targets;
        Caster = caster;
    }
}