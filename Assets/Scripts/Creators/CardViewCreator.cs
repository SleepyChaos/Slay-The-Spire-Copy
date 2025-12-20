using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

public class CardViewCreator : MonoSingleton<CardViewCreator>
{
    [SerializeField] private CardView cardViewPrefab;

    /// <summary>
    /// 生成新的卡牌
    /// </summary>
    /// <param name="card">卡牌信息card类</param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns>生成的卡牌实例</returns>
    public CardView CreateCardView(Card card, Vector3 position, Quaternion rotation)
    {
        // 参数验证
        if (card == null)
        {
            Debug.LogError("❌ CardViewCreator: 卡牌数据为空！无法创建卡牌视图。");
            return null;
        }
        
        if (cardViewPrefab == null)
        {
            Debug.LogError("❌ CardViewCreator: cardViewPrefab 为空！请在 Inspector 中设置 CardView 预制体。");
            return null;
        }
        
        Debug.Log($"创建卡牌视图：{card.Title}");
        
        //简单实例化
        CardView cardView = Instantiate(cardViewPrefab, position, rotation);
        if (cardView == null)
        {
            Debug.LogError("❌ CardViewCreator: 实例化 CardView 失败！");
            return null;
        }
        
        cardView.transform.localScale = Vector3.zero;
        //从0到1的动画效果
        Tween.Scale(cardView.transform, Vector3.one, 0.15f);
        //给实例化出来的卡牌赋值
        cardView.Setup(card);
        //返回使用信息创造的实例卡牌
        return cardView;
    }
}
