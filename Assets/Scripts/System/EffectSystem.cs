using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSystem : MonoBehaviour
{
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<PerformEffectGA>(PerformEffectPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<PerformEffectGA>();
    }

    private IEnumerator PerformEffectPerformer(PerformEffectGA performEffectGA)
    {
        Debug.Log($"效果系统执行：{performEffectGA.Effect.GetType().Name}，目标数：{performEffectGA.Targets?.Count ?? 0}");
        GameAction effectAction = performEffectGA.Effect.GetGameAction(performEffectGA.Targets, HeroSystem.Instance.HeroView);
        ActionSystem.Instance.AddReaction(effectAction);
        yield return null;
    }
}
