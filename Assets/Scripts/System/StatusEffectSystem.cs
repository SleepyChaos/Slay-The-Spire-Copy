using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectSystem : MonoBehaviour
{
    [SerializeField] private GameObject armorEffectFX;
    
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<AddStatusEffectGA>(AddStatusEffectPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<AddStatusEffectGA>();
    }

    /// <summary>
    /// 生成护甲值
    /// </summary>
    /// <param name="addStatusEffectGA"></param>
    /// <returns></returns>
    private IEnumerator AddStatusEffectPerformer(AddStatusEffectGA addStatusEffectGA)
    {
        Debug.Log($"状态系统：添加 {addStatusEffectGA.StatusEffectType} 层数 {addStatusEffectGA.StackCount} 到 {addStatusEffectGA.Targets.Count} 个目标");
        foreach (var target in addStatusEffectGA.Targets)
        {
            Instantiate(armorEffectFX, target.transform);
            target.AddStatusEffect(addStatusEffectGA.StatusEffectType, addStatusEffectGA.StackCount);
            yield return null;
        }
    }
}
