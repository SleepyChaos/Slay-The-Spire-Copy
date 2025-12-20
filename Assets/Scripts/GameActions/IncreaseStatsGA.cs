using UnityEngine;

public class IncreaseStatsGA : GameAction
{
    public Minion Target;
    public int AttackIncreaseAmount;
    public int HealthIncreaseAmount;

    public IncreaseStatsGA(Minion target, int attackIncreaseAmount, int healthIncreaseAmount)
    {
        Target = target;
        AttackIncreaseAmount = attackIncreaseAmount;
        HealthIncreaseAmount = healthIncreaseAmount;
        Debug.Log($"创建提升属性事件：目标 {target.name} 攻击 +{attackIncreaseAmount} 生命 +{healthIncreaseAmount}");
    }
}
