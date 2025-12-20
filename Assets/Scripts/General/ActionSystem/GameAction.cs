using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 注册游戏事件
/// </summary>
public abstract class GameAction 
{
    public List<GameAction> PreReactions {get; private set;} = new List<GameAction>();
    public List<GameAction> PerformReactions {get; private set;} = new List<GameAction>();
    public List<GameAction> PostReactions {get; private set;} = new List<GameAction>();
    static GameAction(){
        Debug.Log("GameAction 基类已加载");
    }
}
