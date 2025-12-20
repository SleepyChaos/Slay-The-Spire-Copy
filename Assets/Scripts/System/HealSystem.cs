using System.Collections;
using UnityEngine;

public class HealSystem : MonoBehaviour
{
    private void OnEnable()
    {
        // 只有这里注册了 Performer，HealGA 发出来才会有实际效果
        Debug.Log("✅ 【HealSystem】正在注册 HealGA 监听...");
        ActionSystem.AttachPerformer<HealGA>(HealPerformer);
        Debug.Log("✅ 【HealSystem】HealGA 监听已注册");
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<HealGA>();
        Debug.Log("❌ 【HealSystem】HealGA 监听已取消");
    }

    private IEnumerator HealPerformer(HealGA action)
    {
        Debug.Log("═══════════════════════════════════════════════════════════");
        Debug.Log($"💚 【治疗系统】开始执行治疗");
        Debug.Log("═══════════════════════════════════════════════════════════");
        
        if (action == null)
        {
            Debug.LogError("❌ HealGA 为空！无法执行治疗。");
            yield return null;
            yield break;
        }
        
        if (action.Target == null)
        {
            Debug.LogError("❌ 治疗目标为空！无法执行治疗。");
            yield return null;
            yield break;
        }
        
        int healthBefore = action.Target.CurrentHealth;
        int maxHealth = action.Target.MaxHealth;
        
        Debug.Log($"   - 目标: {action.Target.name}");
        Debug.Log($"   - 治疗量: +{action.Amount} HP");
        Debug.Log($"   - 治疗前血量: {healthBefore}/{maxHealth}");
        
        // 真正的回血逻辑在这里
        action.Target.Heal(action.Amount);
        
        int healthAfter = action.Target.CurrentHealth;
        int actualHeal = healthAfter - healthBefore;
        
        Debug.Log($"   - 治疗后血量: {healthAfter}/{maxHealth}");
        Debug.Log($"   - 实际恢复: +{actualHeal} HP");
        
        if (actualHeal != action.Amount)
        {
            Debug.LogWarning($"⚠️ 实际恢复量 ({actualHeal}) 与预期 ({action.Amount}) 不一致，可能是血量已满。");
        }
        
        Debug.Log("═══════════════════════════════════════════════════════════");
        yield return null;
    }
}
