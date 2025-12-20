using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSystem : MonoSingleton<ActionSystem>
{
    //当前阶段反应列表的引用
    private List<GameAction> reactions = null;
    //正在执行 属性
    public bool IsPerforming { get; private set; } = false;
    
    /// <summary>
    /// 强制重置 IsPerforming 状态（用于紧急情况）
    /// </summary>
    public void ForceResetPerforming()
    {
        Debug.LogWarning("⚠️ 强制重置 IsPerforming 状态！");
        IsPerforming = false;
    }
    // 事件订阅系统（按事件类型分类存储）
    private static Dictionary<Type, List<Action<GameAction>>> preSubs = new Dictionary<Type, List<Action<GameAction>>>();
    private static Dictionary<Type, List<Action<GameAction>>> postSubs = new Dictionary<Type, List<Action<GameAction>>>();
    // 事件执行器注册表（每个GameAction子类对应的执行逻辑）
    private static Dictionary<Type, Func<GameAction, IEnumerator>> performers = new Dictionary<Type, Func<GameAction, IEnumerator>>();

    
    /// <summary>
    /// 执行游戏事件（入口方法）
    /// </summary>
    /// <param name="action">继承了GameAction的游戏事件</param>
    /// <param name="OnPerformFinished">完成事件后的回调函数</param>
    public void Perform(GameAction action, Action OnPerformFinished = null)
    {
        if (IsPerforming)
        {
            Debug.LogWarning($"⚠️ 尝试执行事件 {action.GetType().Name}，但 ActionSystem 正在执行中，事件被忽略");
            return;
        }
        IsPerforming = true;
        Debug.Log($"开始执行事件：{action.GetType().Name}");
        StartCoroutine(Flow(action, () =>
        {
            IsPerforming = false;
            Debug.Log($"✅ 事件 {action.GetType().Name} 执行完成，IsPerforming 已重置为 false");
            OnPerformFinished?.Invoke();
        }));
    }

    /// <summary>
    /// 添加当前阶段的游戏事件以备激活
    /// </summary>
    /// <param name="gameAction">继承GameAction的游戏事件</param>
    public void AddReaction(GameAction gameAction)
    {
        reactions?.Add(gameAction);
    }

    /// <summary>
    /// 事件处理主流程（三阶段协程）
    /// </summary>
    /// <param name="action"></param>
    /// <param name="OnFlowFinished"></param>
    /// <returns></returns>
    private IEnumerator Flow(GameAction action, Action OnFlowFinished = null)
    {
        // ===== 阶段1：前置处理 =====
        reactions = action.PreReactions;
        PerformSubscribers(action, preSubs);
        yield return PerformReactions();
    
        // ===== 阶段2：主执行 =====
        // 执行核心逻辑前重置反应列表
        reactions = action.PerformReactions; // 使用专门的PerformReactions列表
        yield return PerformPerformer(action);
        yield return PerformReactions();
    
        // ===== 阶段3：后置处理 =====
        reactions = action.PostReactions; // 使用独立的PostReactions列表
        PerformSubscribers(action, postSubs);
        yield return PerformReactions();
    
        OnFlowFinished?.Invoke();
    }

    /// <summary>
    /// 执行事件对应的核心逻辑
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    private IEnumerator PerformPerformer(GameAction action)
    {
        Type type = action.GetType();
        if (performers.ContainsKey(type))
        {
            yield return performers[type](action); // 执行注册的协程逻辑
        }
    }

    /// <summary>
    /// 触发订阅者回调（Pre/POST阶段）
    /// </summary>
    /// <param name="action">事件</param>
    /// <param name="subs">事件链表</param>
    private void PerformSubscribers(GameAction action, Dictionary<Type, List<Action<GameAction>>> subs)
    {
        Type type = action.GetType();
        if (subs.ContainsKey(type))
        {
            foreach (var sub in subs[type])
            {
                sub(action);// 触发所有订阅者
            }
        }
    }
    
    /// <summary>
    /// 执行当前阶段的所有反应事件（递归处理）
    /// </summary>
    /// <returns></returns>
    private IEnumerator PerformReactions()
    {
        foreach (var reaction in reactions)
        {
            yield return Flow(reaction);// 递归执行子事件
        }
    }

    /// <summary>
    /// 注册事件执行器（核心逻辑实现）
    /// </summary>
    /// <typeparam name="T">GameAction子类型</typeparam>
    /// <param name="performer">协程执行函数</param>
    public static void AttachPerformer<T>(Func<T, IEnumerator> performer) where T : GameAction
    {
        Type type = typeof(T);
        // 包装泛型参数适配字典存储
        IEnumerator wrappedPerformer(GameAction action) => performer((T)action);
        if (performers.ContainsKey(type))
            performers[type] = wrappedPerformer;
        else
            performers.Add(type, wrappedPerformer);// 注册或覆盖
    }

    /// <summary>
    /// 注册对应的移除
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static void DetachPerformer<T>() where T : GameAction
    {
        Type type = typeof(T);
        if (performers.ContainsKey(type))
            performers.Remove(type);
    }

    /// <summary>
    /// 订阅事件响应（Pre/POST阶段）
    /// </summary>
    /// <param name="timing">订阅阶段（前置/后置）</param>
    public static void SubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
    {
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubs : postSubs;
        // 创建类型安全的包装回调
        void wrappedReaction(GameAction action) => reaction((T)action);
        if (subs.ContainsKey(typeof(T)))
        {
            subs[typeof(T)].Add(wrappedReaction);
        }
        else
        {
            subs.Add(typeof(T), new List<Action<GameAction>>());
            subs[typeof(T)].Add(wrappedReaction);// 添加订阅
        }
    }

    /// <summary>
    /// 订阅对应的移除
    /// </summary>
    /// <param name="reaction"></param>
    /// <param name="timing"></param>
    /// <typeparam name="T"></typeparam>
    public static void UnsubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
    {
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubs : postSubs;
        if (subs.ContainsKey(typeof(T)))
        {
            void wrappedReaction(GameAction action) => reaction((T)action);
            subs[typeof(T)].Remove(wrappedReaction);
        }
    }
    
}
