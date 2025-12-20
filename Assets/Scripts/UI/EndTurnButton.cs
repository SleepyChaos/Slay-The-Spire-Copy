using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnButton : MonoBehaviour
{
    public void OnClick()
    {
        Debug.Log("结束回合：开始敌方回合");
        //点击结束回合按钮激活 敌人回合事件 查找敌人回合事件绑定的方法搜索EnemyTurnGA的用法，ActionSystem.SubscribeReaction<EnemyTurnGA>
        EnemyTurnGA enemyTurnGA = new EnemyTurnGA();
        ActionSystem.Instance.Perform(enemyTurnGA);
    }
}
