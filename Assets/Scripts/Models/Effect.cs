
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Effect 
{
    public abstract GameAction GetGameAction(List<CombatantView> targets, CombatantView caster);
    static Effect(){
        Debug.Log("Effect 基类已加载");
    }
}
