using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnSystem : MonoBehaviour
{
    [SerializeField] private GameObject burnFX;

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<ApplyBurnGA>(ApplyBurnPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<ApplyBurnGA>();
    }

    private IEnumerator ApplyBurnPerformer(ApplyBurnGA applyBurnGA)
    {
        // 检查目标是否有效
        if (applyBurnGA == null)
        {
            Debug.LogError("❌ ApplyBurnGA 为空！无法执行燃烧伤害。");
            yield break;
        }
        
        CombatantView target = applyBurnGA.Target;
        
        // 检查目标是否仍然有效（可能在场景切换时被销毁）
        if (target == null)
        {
            Debug.LogWarning("⚠️ 燃烧目标已被销毁，跳过燃烧伤害（可能是场景正在切换）");
            yield break;
        }
        
        if (target.gameObject == null)
        {
            Debug.LogWarning("⚠️ 燃烧目标的 GameObject 已被销毁，跳过燃烧伤害");
            yield break;
        }
        
        Debug.Log($"🔥 燃烧系统：对 {target.name} 造成燃烧伤害 {applyBurnGA.BurnDamage}");
        Debug.Log($"   - 目标当前血量: {target.CurrentHealth}/{target.MaxHealth}");
        
        // 生成燃烧特效
        if (burnFX != null && target.transform != null)
        {
            Instantiate(burnFX, target.transform.position, Quaternion.identity);
        }
        
        // 造成燃烧伤害
        int healthBefore = target.CurrentHealth;
        target.Damage(applyBurnGA.BurnDamage);
        int healthAfter = target.CurrentHealth;
        Debug.Log($"   - 燃烧伤害后血量: {healthAfter}/{target.MaxHealth} (损失 {healthBefore - healthAfter} 点)");
        
        // 移除一层燃烧状态
        target.RemoveStatusEffect(StatusEffectType.BURN, 1);
        
        // 等待一下，让伤害动画播放
        yield return new WaitForSeconds(0.15f);
        
        // 再次检查目标是否仍然有效（可能在等待期间被销毁）
        if (target == null || target.gameObject == null)
        {
            Debug.LogWarning("⚠️ 目标在燃烧伤害处理期间被销毁，跳过死亡检查");
            yield break;
        }
        
        // 【关键修复】检查目标是否死亡（血量 <= 0）
        if (healthAfter <= 0)
        {
            Debug.Log($"💀 燃烧伤害导致目标血量归零：{target.name}，当前血量: {healthAfter}");
            
            // 敌人死亡
            if (target is EnemyView enemyView)
            {
                Debug.Log($"   - 敌人因燃烧死亡，发送 KillEnemyGA");
                KillEnemyGA killEnemyGA = new KillEnemyGA(enemyView);
                ActionSystem.Instance.AddReaction(killEnemyGA);
            }
            // 玩家死亡
            else if (target is HeroView)
            {
                Debug.LogWarning("💀 玩家因燃烧伤害死亡！触发战斗失败");
                Debug.LogWarning("   - 将显示死亡界面");
                // 使用 AddReaction 而不是 Perform，避免立即执行导致场景切换
                ActionSystem.Instance.AddReaction(new BattleResultGA(false));
            }
        }
        
        yield return new WaitForSeconds(0.85f); // 剩余等待时间（总共1秒）
    }
}
