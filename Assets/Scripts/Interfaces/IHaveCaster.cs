using UnityEngine;

/// <summary>
/// 施法者接口
/// </summary>
public interface IHaveCaster
{
    CombatantView Caster { get;  }
}
public static class IHaveCasterLogger
{
    static IHaveCasterLogger(){
        Debug.Log("IHaveCaster 接口已加载");
    }
}
