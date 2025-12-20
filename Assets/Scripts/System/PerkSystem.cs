using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 天赋系统
/// </summary>
public class PerkSystem : MonoSingleton<PerkSystem>
{
    [SerializeField] private PerksUI perksUI;
    private readonly List<Perk> perks = new List<Perk>();

    public void AddPerk(Perk perk)
    {
        perks.Add(perk);
        perksUI.AddPerlUI(perk);
        perk.OnAdd();
        Debug.Log($"添加天赋：{perk.Image?.name ?? "未知图标"}");
    }

    public void RemovePerk(Perk perk)
    {
        perks.Remove(perk);
        perksUI.RemovePerlUI(perk);
        perk.OnRemove();
    }
}
