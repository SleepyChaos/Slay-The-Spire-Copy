using System;
using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

public class EnemySystem : MonoSingleton<EnemySystem>
{
    [SerializeField] private EnemyBoardView enemyBoardView;
    public List<EnemyView> Enemies => enemyBoardView.EnemyViews;
    
    
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<EnemyTurnGA>(EnemyTurnPerformer);
        ActionSystem.AttachPerformer<AttackHeroGA>(AttackHeroPerformer);
        ActionSystem.AttachPerformer<KillEnemyGA>(KillEnemyPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<EnemyTurnGA>();
        ActionSystem.DetachPerformer<AttackHeroGA>();
        ActionSystem.DetachPerformer<KillEnemyGA>();
    }

    /// <summary>
    /// 初始化本关敌人 
    /// </summary>
    /// <param name="enemyDatas"></param>
    public void Setup(List<EnemyData> enemyDatas)
    {
        Debug.Log("敌人系统：初始化敌人列表");
        foreach (var enemyData in enemyDatas)
        {
            enemyBoardView.AddEnemy(enemyData);
        }
    }

    /// <summary>
    /// 轮到敌方回合
    /// </summary>
    /// <param name="enemyTurnGA"></param>
    /// <returns></returns>
    private IEnumerator EnemyTurnPerformer(EnemyTurnGA enemyTurnGA)
    {
        Debug.Log("敌方回合");
        Debug.Log($"   - 当前敌人数量: {enemyBoardView.EnemyViews.Count}");

        // 【关键修复】创建敌人列表的副本，避免在遍历时列表被修改
        List<EnemyView> enemiesToProcess = new List<EnemyView>(enemyBoardView.EnemyViews);
        
        foreach (var enemy in enemiesToProcess)
        {
            // 检查敌人是否仍然有效（可能在之前的处理中被销毁）
            if (enemy == null || enemy.gameObject == null)
            {
                Debug.LogWarning("⚠️ 敌人已被销毁，跳过处理");
                continue;
            }
            
            Debug.Log($"   - 处理敌人: {enemy.name}");
            
            //可能有的敌人会造成燃烧伤害
            int burnStacks = enemy.GetStatusEffectStacks(StatusEffectType.BURN);
            if (burnStacks > 0)
            {
                Debug.Log($"     - 敌人有 {burnStacks} 层燃烧，创建燃烧事件");
                ApplyBurnGA applyBurnGA = new ApplyBurnGA(burnStacks, enemy);
                ActionSystem.Instance.AddReaction(applyBurnGA);
            }
            
            // 【关键修复】只有在敌人仍然有效时才创建攻击事件
            // 注意：即使现在有效，在执行时可能已经被销毁，所以 AttackHeroPerformer 也需要检查
            if (enemy != null && enemy.gameObject != null)
            {
                Debug.Log($"     - 创建敌人攻击事件");
                AttackHeroGA attackHeroGA = new AttackHeroGA(enemy);
                ActionSystem.Instance.AddReaction(attackHeroGA);
            }
            else
            {
                Debug.LogWarning($"     - 敌人在创建攻击事件前被销毁，跳过攻击");
            }
        }
        
        yield return null;
        Debug.Log("敌方回合结束");
    }

    /// <summary>
    /// 敌人攻击Performer
    /// </summary>
    /// <param name="attackHeroGA"></param>
    /// <returns></returns>
    private IEnumerator AttackHeroPerformer(AttackHeroGA attackHeroGA)
    {
        // 【关键修复】检查攻击者是否仍然有效（可能在燃烧伤害时被销毁）
        if (attackHeroGA == null)
        {
            Debug.LogError("❌ AttackHeroGA 为空！无法执行攻击。");
            yield break;
        }
        
        EnemyView attacker = attackHeroGA.Attacker;
        
        if (attacker == null)
        {
            Debug.LogWarning("⚠️ 攻击者已被销毁，跳过攻击（可能是被燃烧伤害击杀）");
            yield break;
        }
        
        if (attacker.gameObject == null)
        {
            Debug.LogWarning("⚠️ 攻击者的 GameObject 已被销毁，跳过攻击");
            yield break;
        }
        
        // 检查攻击者是否还在敌人列表中（可能已经被移除）
        if (!enemyBoardView.EnemyViews.Contains(attacker))
        {
            Debug.LogWarning($"⚠️ 攻击者 {attacker.name} 不在敌人列表中，可能已被击杀，跳过攻击");
            yield break;
        }
        
        Debug.Log($"⚔️ 敌人攻击：{attacker.name} 攻击英雄，攻击力 {attacker.AttackPower}");
        
        //攻击动画效果
        if (attacker.transform != null)
        {
            Tween tween = Tween.PositionX(attacker.transform, attacker.transform.position.x - 1f, 0.15f);
            yield return tween;
            
            // 再次检查攻击者是否仍然有效（可能在动画期间被销毁）
            if (attacker == null || attacker.gameObject == null)
            {
                Debug.LogWarning("⚠️ 攻击者在动画期间被销毁，跳过剩余攻击流程");
                yield break;
            }
            
            Tween.PositionX(attacker.transform, attacker.transform.position.x + 1f, 0.25f);
        }
        
        // 再次检查攻击者是否仍然有效
        if (attacker == null || attacker.gameObject == null)
        {
            Debug.LogWarning("⚠️ 攻击者在攻击流程中被销毁，跳过伤害");
            yield break;
        }
        
        //造成伤害
        if (HeroSystem.Instance != null && HeroSystem.Instance.HeroView != null)
        {
            DealDamageGA dealDamageGA = new DealDamageGA(attacker.AttackPower,
                new List<CombatantView>() { HeroSystem.Instance.HeroView }, attackHeroGA.Caster);
            ActionSystem.Instance.AddReaction(dealDamageGA);
            Debug.Log($"✅ 已创建伤害事件：{attacker.AttackPower} 点伤害");
        }
        else
        {
            Debug.LogError("❌ HeroSystem 或 HeroView 为空！无法造成伤害。");
        }
    }

    private IEnumerator KillEnemyPerformer(KillEnemyGA killEnemyGA)
    {
        Debug.Log($"敌人被击杀：{killEnemyGA.EnemyView.name}");
        yield return enemyBoardView.RemoveEnemy(killEnemyGA.EnemyView);
        // [新增] 检查是否还有活着的敌人
        if (enemyBoardView.EnemyViews.Count == 0){
            Debug.Log("🎉 所有敌人已被击败！触发战斗胜利事件");
            // 使用 StartCoroutine 在下一帧执行，避免阻塞当前事件流程
            // 这样 BattleResultGA 会在当前事件完全结束后执行
            BattleResultGA battleResultGA = new BattleResultGA(true);
            StartCoroutine(DelayedBattleResult(battleResultGA));
        }
    }
    
    /// <summary>
    /// 延迟执行战斗结果，确保当前事件流程完全完成
    /// </summary>
    private IEnumerator DelayedBattleResult(BattleResultGA battleResultGA)
    {
        // 等待当前帧结束
        yield return new WaitForEndOfFrame();
        // 等待一帧，确保所有反应事件都处理完成
        yield return null;
        
        // 如果 ActionSystem 还在执行，等待它完成
        int waitCount = 0;
        while (ActionSystem.Instance.IsPerforming)
        {
            waitCount++;
            if (waitCount > 300) // 最多等待300帧（约5秒）
            {
                Debug.LogError("❌ 等待 ActionSystem 完成超时！可能存在死锁。强制重置 IsPerforming。");
                ActionSystem.Instance.ForceResetPerforming();
                break;
            }
            yield return null;
        }
        
        if (waitCount > 0)
        {
            Debug.Log($"⏳ 等待了 {waitCount} 帧后，ActionSystem 执行完成");
        }
        
        // 现在执行战斗结果事件
        Debug.Log("🚀 开始执行战斗结果事件");
        ActionSystem.Instance.Perform(battleResultGA);
    }
}
