using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class TargetMode
{
    public abstract List<CombatantView> GetTargets();
    static TargetMode(){
        Debug.Log("TargetMode 基类已加载");
    }
}
