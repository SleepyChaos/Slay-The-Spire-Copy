using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHeroGA : GameAction, IHaveCaster
{
    public EnemyView Attacker { get; private set; }
    public CombatantView Caster { get; set; }
    
    
    public AttackHeroGA(EnemyView attacker)
    {
        Attacker = attacker;
        Caster = Attacker;
        Debug.Log($"创建敌人攻击事件：{attacker.name}");
    }

}
