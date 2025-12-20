using System;
using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

public class ManaSystem : MonoSingleton<ManaSystem>
{
    [SerializeField] private ManaUI manaUI;

    private const int MAX_MANA = 3;
    private int currentMana = MAX_MANA;


    private void OnEnable()
    {
        ActionSystem.AttachPerformer<SpendManaGA>(SpendManaPerformer);
        ActionSystem.AttachPerformer<RefillManaGA>(RefillManaPerformer);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction,ReactionTiming.POST);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<SpendManaGA>();
        ActionSystem.DetachPerformer<RefillManaGA>();
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }

    public bool HasEnoughMana(int mana)
    {
        if (currentMana >= mana)
        {
            return true;
        }
        else
        {
            //能量不足
            Debug.Log("Mana doesn't have enough mana");
            Tween.ShakeLocalPosition(manaUI.transform, new Vector3(20f, 20f, 0f), 0.5f);
            Tween.ShakeScale(manaUI.transform, new Vector3(0.2f, 0.2f, 0f), 0.5f);
            return false;
        }
        
        return currentMana >= mana;
    }

    private IEnumerator SpendManaPerformer(SpendManaGA spendManaGA)
    {
        Debug.Log($"能量系统：消耗能量 {spendManaGA.Amount}");
        currentMana -= spendManaGA.Amount;
        manaUI.UpdateMana(currentMana);
        yield return null;
    }

    private IEnumerator RefillManaPerformer(RefillManaGA refillManaGA)
    {
        currentMana = MAX_MANA;
        manaUI.UpdateMana(currentMana);
        yield return null;
    }

    /// <summary>
    /// 绑定重新恢复能量到敌人结束回合事件
    /// </summary>
    /// <param name="enemyTurnGA"></param>
    private void EnemyTurnPostReaction(EnemyTurnGA enemyTurnGA)
    {
        RefillManaGA refillManaGA = new RefillManaGA();
        ActionSystem.Instance.AddReaction(refillManaGA);
    }
}
