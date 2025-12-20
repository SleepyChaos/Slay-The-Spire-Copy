using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;

/// <summary>
/// 角色敌人一类基类
/// </summary>
public class CombatantView : MonoBehaviour
{
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private StatusEffectsUI statusEffectsUI;
    public int MaxHealth{get; private set;}
    public int CurrentHealth{get; private set;}
    private Dictionary<StatusEffectType, int> statusEffects = new Dictionary<StatusEffectType, int>();



    public void SetupBase(int health, Sprite image)
    {
        MaxHealth = CurrentHealth = health;
        spriteRenderer.sprite = image;
        UpdateHealthText();
    }
    
    /// <summary>
    /// 直接设置当前血量（用于初始化或恢复血量，不会触发死亡检查）
    /// </summary>
    public void SetHealth(int health)
    {
        CurrentHealth = Mathf.Clamp(health, 0, MaxHealth);
        UpdateHealthText();
        Debug.Log($"{name} 血量已设置为: {CurrentHealth}/{MaxHealth}");
    }

    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = "HP:"  + CurrentHealth;
        }
        else
        {
            Debug.LogWarning($"⚠️ {name} 的 healthText 为空！无法更新血量显示。");
        }
    }

    /// <summary>
    /// 回复生命值
    /// </summary>
    public void Heal(int amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > MaxHealth) 
            CurrentHealth = MaxHealth;
            
        UpdateHealthText(); // 刷新头顶文字
        // TODO: 这里以后可以加个绿色数字飘字特效
    }

    /// <summary>
    /// 对单位造成伤害
    /// </summary>
    /// <param name="damageAmount"></param>
    public void Damage(int damageAmount)
    {
        Debug.Log($"{name} 受到伤害：{damageAmount}，当前血量: {CurrentHealth}/{MaxHealth}");
        
        //先算有没有护盾值
        int remainingDamage = damageAmount;
        int currentArmor = GetStatusEffectStacks(StatusEffectType.ARMOR);
        if (currentArmor > 0)
        {
            if (currentArmor >= damageAmount)
            {
                RemoveStatusEffect(StatusEffectType.ARMOR, remainingDamage);
                remainingDamage = 0;
                Debug.Log($"   - 护盾吸收了全部伤害，剩余护盾: {GetStatusEffectStacks(StatusEffectType.ARMOR)}");
            }
            else if (currentArmor < damageAmount)
            {
                RemoveStatusEffect(StatusEffectType.ARMOR, currentArmor);
                remainingDamage -= currentArmor;
                Debug.Log($"   - 护盾吸收了 {currentArmor} 点伤害，剩余伤害: {remainingDamage}");
            }
        }

        if (remainingDamage > 0)
        {
            int healthBefore = CurrentHealth;
            CurrentHealth -= remainingDamage;
            if (CurrentHealth < 0)
                CurrentHealth = 0;
            
            Debug.Log($"   - 实际扣除血量: {healthBefore - CurrentHealth}，剩余血量: {CurrentHealth}/{MaxHealth}");
            
            // 如果血量归零，记录日志（死亡检查由 DamageSystem 处理）
            if (CurrentHealth <= 0)
            {
                Debug.LogWarning($"⚠️ {name} 血量归零！");
            }
        }
        
        Tween.ShakeLocalPosition(transform, new Vector3(0.2f,0.2f,0), 0.5f);
        UpdateHealthText();
    }

    public void AddStatusEffect(StatusEffectType type, int stackCount)
    {
        if (statusEffects.ContainsKey(type))
        {
            statusEffects[type] += stackCount;
        }
        else
        {
            statusEffects.Add(type, stackCount);
        }
        statusEffectsUI.UpdateStatusEffectUI(type,GetStatusEffectStacks(type));
    }

    public void RemoveStatusEffect(StatusEffectType type, int stackCount)
    {
        if (statusEffects.ContainsKey(type))
        {
            statusEffects[type] -= stackCount;
            if (statusEffects[type] <= 0)
            {
                statusEffects.Remove(type);
            }
        }
        statusEffectsUI.UpdateStatusEffectUI(type, GetStatusEffectStacks(type));
    }

    public int GetStatusEffectStacks(StatusEffectType type)
    {
        if (statusEffects.ContainsKey(type))
        {
            return statusEffects[type];
        }
        else
        {
            return 0;
        }
    }
}
