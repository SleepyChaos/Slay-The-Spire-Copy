using System.Collections.Generic;
using UnityEngine;

public class LifestealEffect : Effect
{
    [SerializeField] private int damageAmount;

    public override GameAction GetGameAction(List<CombatantView> targets, CombatantView caster)
    {
        Debug.Log($"effect传入数据{caster}使用吸血技能对目标{targets.Count}造成{damageAmount}点伤害");
        return new LifestealGA(damageAmount, targets, caster);
    }
}