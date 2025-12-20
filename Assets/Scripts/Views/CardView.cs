using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardView : MonoBehaviour
{
    public Card Card { get; private set; }
    
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text mana;
    [SerializeField] private SpriteRenderer imageSR;
    [SerializeField] private GameObject wrapper;
    [SerializeField] private LayerMask dropLayer;//卡牌使用区域的层级
    
    private Vector3 dragStartPosition;
    private Quaternion dragStartRotation;


    /// <summary>
    /// 初始化这张卡牌的信息
    /// </summary>
    /// <param name="card">card类</param>
    public void Setup(Card card)
    {
        Card = card;
        title.text = card.Title;
        description.text = card.Description;
        mana.text = card.Mana.ToString();
        imageSR.sprite = card.Image;
    }

    /// <summary>
    /// [重载] 仅通过数据表来初始化显示（用于奖励界面预览）
    /// 不需要创建真正的运行时 Card 对象
    /// </summary>
    /// <param name="data">CardData数据表</param>
    public void Setup(CardData data)
    {
        // 注意：这里我们不赋值 Card 属性，因为这是一个纯展示用的 View
        // Card = null; 
        
        Debug.Log($"🎴 CardView.Setup(CardData): 设置卡牌 {data.name}");
        
        // 直接从 data 读取数据显示
        if(title != null) 
        {
            title.text = data.name;
            Debug.Log($"  - Title设置: {title.text}");
        }
        else
        {
            Debug.LogWarning("⚠️ CardView: title 引用为空！");
        }
        
        if(description != null) 
        {
            description.text = data.Description;
            Debug.Log($"  - Description设置: {description.text}");
        }
        else
        {
            Debug.LogWarning("⚠️ CardView: description 引用为空！");
        }
        
        if(mana != null) 
        {
            mana.text = data.Mana.ToString();
            Debug.Log($"  - Mana设置: {mana.text}");
        }
        else
        {
            Debug.LogWarning("⚠️ CardView: mana 引用为空！");
        }
        
        if(imageSR != null) 
        {
            imageSR.sprite = data.Image;
            Debug.Log($"  - Image设置: {(data.Image != null ? data.Image.name : "NULL")}");
            // 确保SpriteRenderer可见
            if (!imageSR.enabled) imageSR.enabled = true;
        }
        else
        {
            Debug.LogWarning("⚠️ CardView: imageSR 引用为空！");
        }
        
        // 确保文本组件正确显示
        if (title != null)
        {
            if (!title.enabled) title.enabled = true;
            if (!title.gameObject.activeSelf) title.gameObject.SetActive(true);
            // 确保文本颜色不透明
            if (title.color.a < 0.1f)
            {
                Color titleColor = title.color;
                titleColor.a = 1f;
                title.color = titleColor;
                Debug.Log($"  - 修复Title透明度: {title.color}");
            }
        }
        
        if (description != null)
        {
            if (!description.enabled) description.enabled = true;
            if (!description.gameObject.activeSelf) description.gameObject.SetActive(true);
            // 确保文本颜色不透明
            if (description.color.a < 0.1f)
            {
                Color descColor = description.color;
                descColor.a = 1f;
                description.color = descColor;
                Debug.Log($"  - 修复Description透明度: {description.color}");
            }
        }
        
        if (mana != null)
        {
            if (!mana.enabled) mana.enabled = true;
            if (!mana.gameObject.activeSelf) mana.gameObject.SetActive(true);
            // 确保文本颜色不透明
            if (mana.color.a < 0.1f)
            {
                Color manaColor = mana.color;
                manaColor.a = 1f;
                mana.color = manaColor;
                Debug.Log($"  - 修复Mana透明度: {mana.color}");
            }
        }
        
        // 确保wrapper可见（如果有）
        if (wrapper != null)
        {
            wrapper.SetActive(true);
        }
        
        // 如果你的 CardView 还有其他视觉元素（比如边框颜色），也要在这里设置
    }
    
    /// <summary>
    /// 选中卡牌放大效果
    /// </summary>
    private void OnMouseEnter()
    {
        // 如果 Card 为 null（奖励界面），不触发悬停效果
        if (Card == null)
            return;
            
        if (!Interactions.Instance.PlayerCanHover())
            return;
        
        wrapper.SetActive(false);
        Vector3 pos = new Vector3(transform.position.x, -2, 0);
        CardViewHoverSystem.Instance.Show(Card, pos);
    }

    /// <summary>
    /// 鼠标离开卡牌复原
    /// </summary>
    private void OnMouseExit()
    {
        // 如果 Card 为 null（奖励界面），不触发悬停效果
        if (Card == null)
            return;
            
        if (!Interactions.Instance.PlayerCanHover())
            return;
        
        CardViewHoverSystem.Instance.Hide();
        wrapper.SetActive(true);
    }
    
    /// <summary>
    /// 鼠标按下卡牌 进行移动前的准备
    /// </summary>
    private void OnMouseDown()
    {
        // 如果 Card 为 null（奖励界面），不触发战斗交互
        if (Card == null)
            return;
            
        if (!Interactions.Instance.PlayerCanInteract())
            return;

        Debug.Log($"尝试选择卡牌：{Card.Title}");
        if (Card.ManualTargetEffect != null)
        {
            ManualTargetSystem.Instance.StartTargeting(transform.position);
        }
        else
        {
            Interactions.Instance.PlayerIsDragging = true;
            wrapper.SetActive(true);
            CardViewHoverSystem.Instance.Hide();
            dragStartPosition = transform.position;
            dragStartRotation = transform.rotation;
            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.position = MouseUtil.GetMousePositionInWorldSpace(-1);
        }
    }

    /// <summary>
    /// 拖动当前选中卡牌
    /// </summary>
    private void OnMouseDrag()
    {
        // 如果 Card 为 null（奖励界面），不触发拖拽
        if (Card == null)
            return;
            
        if (!Interactions.Instance.PlayerCanInteract())
            return;

        if (Card.ManualTargetEffect != null)
            return;
        
        transform.position = MouseUtil.GetMousePositionInWorldSpace(-1);
    }

    

    /// <summary>
    /// 对选中的卡牌进行落子判断
    /// </summary>
    private void OnMouseUp()
    {
        // 如果 Card 为 null（奖励界面），不触发战斗交互
        if (Card == null)
            return;
            
        if (!Interactions.Instance.PlayerCanInteract())
            return;

        if (Card.ManualTargetEffect != null)
        {
            EnemyView target = ManualTargetSystem.Instance.EnemyTargeting(MouseUtil.GetMousePositionInWorldSpace(-1));
            if (target != null && ManaSystem.Instance.HasEnoughMana(Card.Mana))
            {
                PlayCardGA playCardGA = new PlayCardGA(Card, target);
                ActionSystem.Instance.Perform(playCardGA);
            }
        }
        else
        {
            if (ManaSystem.Instance.HasEnoughMana(Card.Mana) 
                && Physics.Raycast(transform.position, Vector3.forward, out RaycastHit hit, 10f, dropLayer))
            {
                PlayCardGA playCardGA = new PlayCardGA(Card);//以显示卡牌作为中间数据 读取出去新的卡牌数据 获取到当前选中卡牌数据
                ActionSystem.Instance.Perform(playCardGA);
            }
            else//没放在有效区域回到手牌区域
            {
                transform.position = dragStartPosition;
                transform.rotation = dragStartRotation;
            }
            Interactions.Instance.PlayerIsDragging = false;
        }
    }

    
}
