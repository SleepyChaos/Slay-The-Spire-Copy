using System;
using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

public class DamageSystem : MonoBehaviour
{
    [SerializeField] private GameObject damageVFX;
    
    
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<DealDamageGA>(DealDamagePerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<DealDamageGA>();
    }

    /// <summary>
    /// 对单位造成伤害 玩家 敌人 共用受击系统
    /// </summary>
    /// <param name="dealDamageGA"></param>
    /// <returns></returns>
    private IEnumerator DealDamagePerformer(DealDamageGA dealDamageGA)
    {
        Debug.Log($"伤害系统开始处理：{dealDamageGA.Amount} 点伤害，目标数：{dealDamageGA.Targets.Count}");
        foreach (var target in dealDamageGA.Targets)
        {
            // 检查目标是否仍然有效
            if (target == null || target.gameObject == null)
            {
                Debug.LogWarning("⚠️ 目标已被销毁，跳过伤害处理");
                continue;
            }
            
            int healthBefore = target.CurrentHealth;
            target.Damage(dealDamageGA.Amount);
            int healthAfter = target.CurrentHealth;
            
            //生成受击特效
            if (damageVFX != null && target.transform != null)
            {
                Instantiate(damageVFX, target.transform.position, Quaternion.identity);
            }
            
            yield return new WaitForSeconds(0.15f);
            
            // 再次检查目标是否仍然有效（可能在等待期间被销毁）
            if (target == null || target.gameObject == null)
            {
                Debug.LogWarning("⚠️ 目标在伤害处理期间被销毁，跳过死亡检查");
                continue;
            }
            
            if (healthAfter <= 0)
            {
                Debug.Log($"💀 目标血量归零：{target.name}，当前血量: {healthAfter}");
                
                //敌人血量被打的小于0
                if (target is EnemyView enemyView)
                {
                    Debug.Log($"   - 敌人死亡，发送 KillEnemyGA");
                    KillEnemyGA killEnemyGA = new KillEnemyGA(enemyView);
                    ActionSystem.Instance.AddReaction(killEnemyGA);
                }
                else if (target is HeroView)//玩家血量被打的小于0
                {
                    Debug.LogWarning("💀 玩家血量归零！触发战斗失败");
                    Debug.LogWarning("   - 将加载 StartScene，场景中的对象将被销毁");
                    // 使用 AddReaction 而不是 Perform，避免立即执行导致场景切换
                    ActionSystem.Instance.AddReaction(new BattleResultGA(false));
                }
            }
        }
    }
}
