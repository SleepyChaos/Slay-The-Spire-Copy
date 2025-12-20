using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifestealSystem : MonoBehaviour
{
    [SerializeField] private GameObject damageVFX; // 可以拖入通用的受击特效

    private void OnEnable()
    {
        Debug.Log("✅ 【System层】正在注册 LifestealGA 监听...");
        ActionSystem.AttachPerformer<LifestealGA>(LifestealPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<LifestealGA>();
    }

    private IEnumerator LifestealPerformer(LifestealGA action)
    {
        int totalHealthRestored = 0;
        Debug.Log($"⚡️ 【System层】接收到吸血指令！目标数: {action.Targets?.Count ?? 0}");
        foreach (var target in action.Targets)
        {

            // 1. 记录被打之前的血量
            int hpBefore = target.CurrentHealth;

            // 2. 直接调用 View 层的伤害方法 (复用现有的伤害逻辑)
            target.Damage(action.DamageAmount);
            
            // 可选：播放特效
            if(damageVFX != null) 
                Instantiate(damageVFX, target.transform.position, Quaternion.identity);

            // 3. 计算实际造成了多少血量损失 (这就是我们要吸的量)
            int hpAfter = target.CurrentHealth;
            int damageDealt = hpBefore - hpAfter;

            // 如果打在护盾上没掉血，就不吸血；如果掉血了，就累加
            Debug.Log($"⚔️ 造成实际伤害: {damageDealt}");

            if (damageDealt > 0)
            {
                totalHealthRestored += damageDealt;
            }
            // 4. 检查目标是否死亡（血量 <= 0）
            if (hpAfter <= 0)
            {
                Debug.Log($"💀 吸血伤害导致目标血量归零：{target.name}，当前血量: {hpAfter}");
                
                // 敌人死亡
                if (target is EnemyView enemy)
                {
                    Debug.Log($"   - 敌人因吸血伤害死亡，发送 KillEnemyGA");
                    ActionSystem.Instance.AddReaction(new KillEnemyGA(enemy));
                }
                // 玩家死亡
                else if (target is HeroView)
                {
                    Debug.LogWarning("💀 玩家因吸血伤害死亡！触发战斗失败");
                    Debug.LogWarning("   - 将显示死亡界面");
                    // 使用 AddReaction 而不是 Perform，避免立即执行导致场景切换
                    ActionSystem.Instance.AddReaction(new BattleResultGA(false));
                }
            }

        }
        // 5. 只有造成了伤害才回血
        if (totalHealthRestored > 0)
        {
            Debug.Log("═══════════════════════════════════════════════════════════");
            Debug.Log($"💚 【吸血系统】准备回血");
            Debug.Log("═══════════════════════════════════════════════════════════");
            Debug.Log($"   - 总恢复量: {totalHealthRestored} HP");
            
            if (action.Caster == null)
            {
                Debug.LogError("❌ 吸血施法者为空！无法回血。");
            }
            else
            {
                int casterHealthBefore = action.Caster.CurrentHealth;
                Debug.Log($"   - 施法者: {action.Caster.name}");
                Debug.Log($"   - 施法者当前血量: {casterHealthBefore}/{action.Caster.MaxHealth}");
                
                HealGA healGA = new HealGA(totalHealthRestored, action.Caster);
                Debug.Log($"✅ 已创建 HealGA: 恢复 {totalHealthRestored} HP 给 {action.Caster.name}");
                
                // 发送治疗事件
                ActionSystem.Instance.AddReaction(healGA);
                Debug.Log($"✅ 已发送 HealGA 到 ActionSystem");
            }
            Debug.Log("═══════════════════════════════════════════════════════════");
        }
        else
        {
            Debug.Log("⚠️ 【吸血系统】没有造成实际伤害，无法回血。");
        }

        yield return new WaitForSeconds(0.2f); // 稍微停顿一下展示效果
    }
}