using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PrimeTween;
using UnityEngine;
using UnityEngine.Splines;

[HelpURL("https://www.bilibili.com/video/BV1CoKPzHEyV")]
public class HandView : MonoBehaviour
{
    [Tooltip("最多可持有手牌")] 
    [SerializeField] private float maxHandleSize = 10f;
    [SerializeField] private SplineContainer splineContainer;
    private readonly List<CardView> cards = new List<CardView>();//当前持有手牌


    /// <summary>
    /// 向手牌区添加卡牌
    /// </summary>
    /// <param name="cardView"></param>
    /// <returns></returns>
    public IEnumerator AddCard(CardView cardView)
    {
        Debug.Log($"手牌区添加卡牌：{cardView.Card.Title}");
        cards.Add(cardView);
        yield return UpdateCardPositions(0.15f);
    }
    
    /// <summary>
    /// 从手牌中丢弃卡牌
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    public CardView RemoveCard(Card card)
    {
        CardView cardView = GetCardView(card);
        if (cardView == null)
            return null;
        
        cards.Remove(cardView);
        StartCoroutine(UpdateCardPositions(0.15f));
        return cardView;
    }

    /// <summary>
    /// 根据传入卡牌信息寻找卡牌位置
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    private CardView GetCardView(Card card)
    {
        return cards.Where(cardView => cardView.Card == card).FirstOrDefault();
    }

    /// <summary>
    /// 更新手牌在手牌曲线上的位置
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    private IEnumerator UpdateCardPositions(float duration)
    {
        if (cards.Count == 0)
            yield break;

        //整个曲线的最大长度是1，里面卡牌放置的位置就是在0~1之间，所以用1/最大手牌数 得到装满时每张卡牌的间隔作为卡牌间隔
        float cardSpacing = 1f / maxHandleSize;
        //手上没有卡牌时 添加第一张卡牌 牌是从中间开始排列，所以只有一张牌时的位置应该是0.5
        //卡牌每次增多就需要将第一张牌往左移一点达成整副牌组居中的效果
        //居中是整个中心点居中 所以从0.5往左减去间隔/2，不/2相当于每次新增加的牌排列在中间，/2的话就是第一张牌的位置是0.5-0.025，第二张牌是0.5+0.025，这样就能够居中
        float firstCardPosition = 0.5f - (cards.Count - 1) * cardSpacing / 2;
        Spline spline = splineContainer.Spline;
        //插进一张牌更新所有牌的位置
        for (int i = 0; i < cards.Count; i++)
        {
            //理解了上面就是正常在起始点往后 根据间隔增加位移
            float p = firstCardPosition + i * cardSpacing;
            //上面说了曲线的长度是1，卡牌的位置是0~1之间，这个坐标不是世界坐标
            //EvaluatePosition将曲线上的坐标 返回成一个Vector3
            Vector3 splinePosition = spline.EvaluatePosition(p);
            //EvaluateTangent根据曲线上的坐标返回 相当于曲线在世界坐标上的切线向量
            Vector3 forward = spline.EvaluateTangent(p);
            //EvaluateTangent根据曲线上的坐标 返回世界坐标的z轴的方向，指向的是卡牌的背面
            Vector3 up = spline.EvaluateUpVector(p);
            //将卡牌最终的旋转四元数计算出来，向前是朝着摄像机 也就是z轴的反方向，向上是曲线的法线向量，但是好像没有直接拿曲线法线的方法，就把朝向反向和切线做了叉乘
            Quaternion rotation = Quaternion.LookRotation(-up, Vector3.Cross(-up, forward).normalized);
            //动画效果
            //他这里最后在z轴朝着视野每次推进了0.01可以避免渲染的层级错误了
            Tween.Position(cards[i].transform, splinePosition + transform.position + 0.01f * i * Vector3.back, duration);
            Tween.Rotation(cards[i].transform, rotation.eulerAngles, duration);
        }
        yield return new WaitForSeconds(duration);
    }
}
