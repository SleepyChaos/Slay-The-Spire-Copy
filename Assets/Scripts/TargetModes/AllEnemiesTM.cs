using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllEnemiesTM : TargetMode
{
    public override List<CombatantView> GetTargets()
    {
        Debug.Log("目标模式：所有敌人");
        return new List<CombatantView>(EnemySystem.Instance.Enemies);
    }
}
