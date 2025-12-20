using UnityEngine;

public class HealGA : GameAction
{
    public int Amount { get; private set; }
    public CombatantView Target { get; private set; }

    public HealGA(int amount, CombatantView target)
    {
        Amount = amount;
        Target = target;
        Debug.Log($"创建治疗事件：{target.name} 恢复 {amount} 点生命");
    }
}
