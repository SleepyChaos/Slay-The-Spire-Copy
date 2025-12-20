using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroTM : TargetMode
{
    public override List<CombatantView> GetTargets()
    {
        Debug.Log("目标模式：英雄");
        List<CombatantView> targets = new List<CombatantView>()
        {
            HeroSystem.Instance.HeroView
        };
        
        return targets;
    }
}
