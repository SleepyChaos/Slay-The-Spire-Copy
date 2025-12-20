using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering; // 用于 SortingGroup
using System;

public class RewardCardAdapter : MonoBehaviour
{
    [Header("设置")]
    public float uiScaleFactor = 100f; // 放大倍数，通常 Sprite 在 UI 上需要放大 100 倍
    public int sortingOrder = 100;     // 确保卡牌显示在 UI 背景之上

    [Header("引用")]
    public Button clickButton;         // 透明的 UI 按钮
    public Transform cardSpawnPoint;   // 卡牌生成的父节点

    private CardData myCardData;
    private Action<CardData> onClickCallback;

    /// <summary>
    /// 初始化：生成真正的 CardView 并设置它
    /// </summary>
    public void Setup(CardView cardViewPrefab, CardData data, Action<CardData> callback)
    {
        myCardData = data;
        onClickCallback = callback;

        // 1. 清理旧的（如果有）
        foreach (Transform child in cardSpawnPoint) Destroy(child.gameObject);

        // 2. 实例化你的战斗卡牌预制体
        CardView cardInstance = Instantiate(cardViewPrefab, cardSpawnPoint);

        // 3. 【关键】解决坐标系差异
        // 调整层级：UI 里的物体 Z 轴通常是 0
        cardInstance.transform.localPosition = Vector3.zero;
        cardInstance.transform.localRotation = Quaternion.identity;
        // 放大：把世界坐标的物体放大到 UI 尺寸
        cardInstance.transform.localScale = new Vector3(uiScaleFactor, uiScaleFactor, uiScaleFactor);
        
        Debug.Log($"📐 CardView变换设置: Position={cardInstance.transform.localPosition}, Scale={cardInstance.transform.localScale}");

        // 4. 【关键】解决渲染层级问题
        // 尝试获取 SortingGroup，强制把它的层级提高，否则会被 Panel 遮挡
        SortingGroup sg = cardInstance.GetComponent<SortingGroup>();
        if (sg != null)
        {
            sg.sortingOrder = sortingOrder;
            Debug.Log($"✅ 设置SortingGroup Order为 {sortingOrder}");
        }
        else
        {
            // 如果没有 SortingGroup，就加一个
            sg = cardInstance.gameObject.AddComponent<SortingGroup>();
            sg.sortingOrder = sortingOrder;
            Debug.Log($"✅ 添加SortingGroup组件，Order设置为 {sortingOrder}");
        }
        
        // 【关键修复】确保CardView的SpriteRenderer能被Camera看到
        // 在UI Canvas中，SpriteRenderer需要正确的Layer和Camera设置
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            Debug.Log($"📷 Canvas RenderMode: {canvas.renderMode}, WorldCamera: {(canvas.worldCamera != null ? canvas.worldCamera.name : "NULL")}");
            
            // 设置CardView的Layer为Default层（Layer 0），确保Camera能看到
            // 注意：如果Canvas是Screen Space - Camera，需要确保Camera的Culling Mask包含这个Layer
            cardInstance.gameObject.layer = 0; // Default Layer
            foreach (Transform child in cardInstance.transform)
            {
                child.gameObject.layer = 0;
                child.gameObject.SetActive(true);
            }
            
            // 设置所有SpriteRenderer的sortingLayer和order
            SpriteRenderer[] spriteRenderers = cardInstance.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var sr in spriteRenderers)
            {
                if (sr != null)
                {
                    // 尝试设置Sorting Layer为"UI"或"Default"
                    try
                    {
                        sr.sortingLayerName = "Default";
                        sr.sortingOrder = sortingOrder;
                        Debug.Log($"✅ 设置SpriteRenderer {sr.name}: Layer={sr.sortingLayerName}, Order={sr.sortingOrder}, Enabled={sr.enabled}");
                    }
                    catch
                    {
                        // 如果Sorting Layer不存在，使用ID
                        sr.sortingLayerID = 0; // Default layer ID
                        sr.sortingOrder = sortingOrder;
                        Debug.Log($"✅ 设置SpriteRenderer {sr.name}: LayerID={sr.sortingLayerID}, Order={sr.sortingOrder}");
                    }
                    
                    // 确保SpriteRenderer启用
                    if (!sr.enabled) sr.enabled = true;
                }
            }
            
            // 如果Canvas是Screen Space - Camera，确保Camera能看到这些对象
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera != null)
            {
                Camera cam = canvas.worldCamera;
                // 确保Camera的Culling Mask包含Default层
                if ((cam.cullingMask & (1 << 0)) == 0)
                {
                    Debug.LogWarning("⚠️ Camera的Culling Mask不包含Default层，可能无法看到CardView！");
                }
                Debug.Log($"📷 Camera设置: CullingMask={cam.cullingMask}, 包含Layer 0: {((cam.cullingMask & (1 << 0)) != 0)}");
            }
        }
        
        // 确保CardView的所有子对象都可见
        foreach (Transform child in cardInstance.transform)
        {
            child.gameObject.SetActive(true);
        }

        // 5. 禁用战斗相关组件 (防止选卡时触发拖拽或物理碰撞)
        BoxCollider collider = cardInstance.GetComponent<BoxCollider>();
        if (collider != null)
        {
            collider.enabled = false;
            Debug.Log($"   - 已禁用 CardView 的 BoxCollider，防止拦截按钮点击");
        }
        
        // 禁用 CardView 的所有 Collider（包括子对象）
        Collider[] allColliders = cardInstance.GetComponentsInChildren<Collider>();
        foreach (var col in allColliders)
        {
            if (col != null)
            {
                col.enabled = false;
                Debug.Log($"   - 已禁用 Collider: {col.name}");
            }
        }
        
        // 如果 CardView 上有拖拽脚本，最好也禁用，例如：
        // if (cardInstance.GetComponent<Draggable>() != null) cardInstance.GetComponent<Draggable>().enabled = false;

        // 6. 填充数据
        Debug.Log($"📝 准备设置CardView数据: {data.name}");
        cardInstance.Setup(data);
        
        // 7. 隐藏RewardCardContainer上的占位文本（在设置数据后）
        // 先隐藏RewardCardAdapter自己的占位文本
        TMPro.TextMeshProUGUI[] adapterTexts = GetComponentsInChildren<TMPro.TextMeshProUGUI>(true);
        foreach (var text in adapterTexts)
        {
            // 如果文本包含占位符内容，且不是CardView的文本，隐藏它
            if (text != null && !text.transform.IsChildOf(cardInstance.transform) &&
                (text.text.Contains("here") || text.text.Contains("Button") || 
                 text.text == "技能名称" || text.text == "技能描述" || text.text.Contains("技能")))
            {
                Debug.Log($"🔇 隐藏占位文本: '{text.text}' (来自 {text.gameObject.name})");
                text.gameObject.SetActive(false);
            }
        }
        
        // 8. 确保文本组件正确显示
        Debug.Log($"✅ CardView数据设置完成，验证并修复文本组件...");
        TMPro.TMP_Text[] cardTexts = cardInstance.GetComponentsInChildren<TMPro.TMP_Text>(true);
        foreach (var text in cardTexts)
        {
            if (text != null)
            {
                // 确保文本组件已启用
                if (!text.enabled) text.enabled = true;
                if (!text.gameObject.activeSelf) text.gameObject.SetActive(true);
                
                // 确保文本颜色不透明
                if (text.color.a < 0.1f)
                {
                    Color textColor = text.color;
                    textColor.a = 1f;
                    text.color = textColor;
                    Debug.Log($"  - 修复文本透明度: {text.name}, 新颜色: {text.color}");
                }
                
                // 确保文本可见
                CanvasRenderer canvasRenderer = text.GetComponent<CanvasRenderer>();
                if (canvasRenderer != null)
                {
                    if (canvasRenderer.GetAlpha() < 0.1f)
                    {
                        canvasRenderer.SetAlpha(1f);
                        Debug.Log($"  - 修复CanvasRenderer透明度: {text.name}");
                    }
                }
                
                // 设置文本的 Sorting Order（如果是3D文本）
                if (text is TMPro.TextMeshPro) // 3D文本
                {
                    TMPro.TextMeshPro tmp3D = text as TMPro.TextMeshPro;
                    Renderer textRenderer = tmp3D.GetComponent<Renderer>();
                    if (textRenderer != null)
                    {
                        textRenderer.sortingLayerName = "Default";
                        textRenderer.sortingOrder = sortingOrder + 10; // 文本应该在卡牌图片之上
                        Debug.Log($"  - 设置3D文本Sorting Order: {text.name}, Order={textRenderer.sortingOrder}");
                    }
                }
                
                Debug.Log($"  - 文本组件: {text.name}, 内容: '{text.text}', 激活: {text.gameObject.activeSelf}, 颜色: {text.color}, Alpha: {text.color.a}");
            }
        }
        
        // 9. 检查SpriteRenderer（使用不同的变量名避免冲突）
        SpriteRenderer[] allSpriteRenderers = cardInstance.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sr in allSpriteRenderers)
        {
            if (sr != null)
            {
                Debug.Log($"  - SpriteRenderer: {sr.name}, Sprite: {(sr.sprite != null ? sr.sprite.name : "NULL")}, Enabled: {sr.enabled}, Visible: {sr.isVisible}");
            }
        }
        
        // 7. 绑定 UI 点击事件
        if (clickButton != null)
        {
            clickButton.onClick.RemoveAllListeners();
            clickButton.onClick.AddListener(OnClick);
            clickButton.gameObject.SetActive(true); // 确保按钮可见
            
            // 确保按钮可以接收点击事件
            // 设置按钮的 Raycast Target 和 Interactable
            clickButton.interactable = true;
            
            // 确保按钮在最上层（通过设置 RectTransform 的 sibling index）
            RectTransform buttonRect = clickButton.GetComponent<RectTransform>();
            if (buttonRect != null)
            {
                buttonRect.SetAsLastSibling(); // 将按钮移到最上层
                Debug.Log($"   - 已将按钮移到最上层 (SiblingIndex: {buttonRect.GetSiblingIndex()})");
            }
            
            Image buttonImage = clickButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.raycastTarget = true;
                // 如果按钮图片是透明的，设置一个非常小的 alpha 值以确保可以接收点击
                if (buttonImage.color.a < 0.01f)
                {
                    Color c = buttonImage.color;
                    c.a = 0.01f; // 几乎透明但可以接收点击
                    buttonImage.color = c;
                    Debug.Log($"   - 按钮图片几乎透明，已设置最小 alpha 值以接收点击");
                }
                Debug.Log($"✅ 按钮已启用，RaycastTarget: {buttonImage.raycastTarget}, Interactable: {clickButton.interactable}, Alpha: {buttonImage.color.a}");
            }
            else
            {
                Debug.LogWarning("⚠️ 按钮上没有 Image 组件！可能无法接收点击事件。");
            }
            
            // 检查 Canvas Group（如果有的话）
            CanvasGroup canvasGroup = clickButton.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.blocksRaycasts = true;
                canvasGroup.interactable = true;
                Debug.Log($"   - CanvasGroup 已启用: blocksRaycasts={canvasGroup.blocksRaycasts}, interactable={canvasGroup.interactable}");
            }
            
            Debug.Log($"✅ 已绑定按钮点击事件: {clickButton.gameObject.name}");
        }
        else
        {
            Debug.LogError("❌ RewardCardAdapter: clickButton 未设置！无法点击卡牌。");
            Debug.LogError("💡 请在 RewardCardAdapter 的 Inspector 中设置 Click Button 引用。");
        }
    }

    private void OnClick()
    {
        Debug.Log("═══════════════════════════════════════════════════════════");
        Debug.Log("🃏 【按钮点击】RewardCardAdapter 按钮被点击");
        Debug.Log("═══════════════════════════════════════════════════════════");
        
        if (myCardData == null)
        {
            Debug.LogError("❌ 卡牌数据为空！无法选择。");
            return;
        }
        
        Debug.Log($"📋 点击的卡牌: {myCardData.name}");
        Debug.Log($"   - 回调函数: {(onClickCallback != null ? "已设置" : "未设置")}");
        
        if (onClickCallback != null)
        {
            Debug.Log("✅ 正在调用回调函数...");
            try
            {
                onClickCallback.Invoke(myCardData);
                Debug.Log("✅ 回调函数调用成功");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ 回调函数调用失败: {e.Message}");
                Debug.LogError($"   堆栈跟踪: {e.StackTrace}");
            }
        }
        else
        {
            Debug.LogError("❌ 回调函数为空！无法选择卡牌。");
            Debug.LogError("💡 请检查 RewardUI.PickCard 是否正确传递给 RewardCardAdapter.Setup");
        }
        
        Debug.Log("═══════════════════════════════════════════════════════════");
    }
}