using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardViewHoverSystem : MonoSingleton<CardViewHoverSystem>
{
    //起显示作用的卡牌
    [SerializeField] private CardView cardViewHover;

    /// <summary>
    /// 放大选中卡牌
    /// </summary>
    /// <param name="card">传入选中的卡牌所持的card类信息</param>
    /// <param name="position">选中卡牌的位置</param>
    public void Show(Card card, Vector3 position)
    {
        Debug.Log($"卡牌悬浮显示：{card.Title}");
        //把起显示作用的卡牌激活
        cardViewHover.gameObject.SetActive(true);
        //并更新卡牌信息为选中的卡牌信息
        cardViewHover.Setup(card);
        //并设置位置达成原本卡牌的放大的效果
        cardViewHover.transform.position = position;
    }

    public void Hide()
    {
        Debug.Log("卡牌悬浮隐藏");
        cardViewHover.gameObject.SetActive(false);
    }    
}
