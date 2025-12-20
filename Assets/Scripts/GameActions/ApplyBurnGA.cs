using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyBurnGA : GameAction
{
    public int BurnDamage { get; private set; }
    public CombatantView Target { get; private set; }


    public ApplyBurnGA(int burnDamage, CombatantView target)
    {
        BurnDamage = burnDamage;
        Target = target;
        
        // 检查目标是否有效
        if (target == null)
        {
            Debug.LogError("❌ ApplyBurnGA: 目标为空！无法创建燃烧事件。");
            return;
        }
        
        // 检查目标的 GameObject 是否仍然有效
        if (target.gameObject == null)
        {
            Debug.LogError("❌ ApplyBurnGA: 目标的 GameObject 已被销毁！无法创建燃烧事件。");
            return;
        }
        
        string targetName = target.name;
        Debug.Log($"创建燃烧事件：{targetName} 受到 {burnDamage} 点燃烧伤害");
    }
}
