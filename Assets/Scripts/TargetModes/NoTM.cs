using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoTM : TargetMode
{
    public override List<CombatantView> GetTargets()
    {
        Debug.Log("目标模式：无目标");
        return null;
    }
}
