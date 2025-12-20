using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactions : MonoSingleton<Interactions>
{
    public bool PlayerIsDragging { get; set; } = false;
    public bool IsRewardUIOpen { get; set; } = false; // 奖励界面是否打开
    public bool IsDeathUIOpen { get; set; } = false;  // 死亡界面是否打开


    public bool PlayerCanInteract()
    {
        // 如果奖励界面打开，禁止战斗交互
        if (IsRewardUIOpen)
            return false;
            
        // 如果死亡界面打开，禁止战斗交互
        if (IsDeathUIOpen)
            return false;
            
        if(!ActionSystem.Instance.IsPerforming)
        {
            Debug.Log("交互检查：可以交互");
            return true;
        }
        else
        {
            Debug.Log("交互检查：事件执行中，禁止交互");
            return false;
        }
    }

    public bool PlayerCanHover()
    {
        // 如果奖励界面打开，禁止悬停效果
        if (IsRewardUIOpen)
            return false;
            
        // 如果死亡界面打开，禁止悬停效果
        if (IsDeathUIOpen)
            return false;
            
        if (PlayerIsDragging)
            return false;
        else
            return true;
    }
}
