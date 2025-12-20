using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// 死亡界面：显示角色死亡画面，遮挡整个屏幕
/// </summary>
public class DeathUI : MonoBehaviour
{
    [Header("界面组件")]
    public GameObject content;              // 整个 Panel (用于遮挡全屏)
    public Image backgroundImage;           // 背景图片（半透明黑色或其他）
    public TextMeshProUGUI deathTitleText; // 死亡标题文本（如"你死了"）
    public TextMeshProUGUI deathMessageText; // 死亡信息文本（可选）
    public Button returnToMenuButton;      // 返回主菜单按钮
    public Button retryButton;             // 重试按钮（可选）

    [Header("设置")]
    [Tooltip("死亡界面显示后，延迟多少秒后允许点击按钮（用于显示动画）")]
    public float buttonEnableDelay = 1f;
    
    [Tooltip("是否在显示时播放淡入动画")]
    public bool useFadeInAnimation = true;
    
    [Tooltip("淡入动画时长")]
    public float fadeInDuration = 0.5f;

    private CanvasGroup canvasGroup;
    private bool isShowing = false;

    private void Start()
    {
        // 初始化时隐藏UI
        HideUI();
        
        // 获取或创建 CanvasGroup（用于淡入动画）
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        // 绑定按钮事件
        if (returnToMenuButton != null)
        {
            returnToMenuButton.onClick.RemoveAllListeners();
            returnToMenuButton.onClick.AddListener(OnReturnToMenuClicked);
        }
        
        if (retryButton != null)
        {
            retryButton.onClick.RemoveAllListeners();
            retryButton.onClick.AddListener(OnRetryClicked);
        }
    }

    /// <summary>
    /// 显示死亡界面
    /// </summary>
    public void Show()
    {
        if (isShowing)
        {
            Debug.LogWarning("⚠️ DeathUI 已经在显示中，跳过重复显示");
            return;
        }
        
        Debug.Log("═══════════════════════════════════════════════════════════");
        Debug.Log("💀 【死亡界面】开始显示死亡画面");
        Debug.Log("═══════════════════════════════════════════════════════════");
        
        isShowing = true;
        
        // 1. 确保父对象（Canvas）也是激活的
        Transform parent = transform.parent;
        while (parent != null)
        {
            if (!parent.gameObject.activeSelf)
            {
                Debug.LogWarning($"⚠️ 父对象 {parent.name} 未激活，正在激活...");
                parent.gameObject.SetActive(true);
            }
            parent = parent.parent;
        }
        
        // 2. 检查Canvas组件
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            Debug.Log($"✅ 找到父Canvas: {canvas.name}, Active={canvas.gameObject.activeSelf}, SortOrder={canvas.sortingOrder}");
            if (!canvas.gameObject.activeSelf)
            {
                canvas.gameObject.SetActive(true);
            }
            
            // 确保Canvas的Sort Order足够高，显示在最上层
            if (canvas.sortingOrder < 200)
            {
                canvas.sortingOrder = 200; // 死亡界面应该显示在所有其他UI之上
                Debug.Log($"✅ 已设置Canvas Sort Order为 {canvas.sortingOrder}");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ 未找到父Canvas组件！");
        }
        
        // 3. 显示UI
        gameObject.SetActive(true);
        if (content != null)
        {
            content.SetActive(true);
        }
        
        // 4. 设置背景（如果未设置，使用默认半透明黑色）
        if (backgroundImage != null)
        {
            if (backgroundImage.color.a < 0.1f)
            {
                // 如果背景图片是透明的，设置为半透明黑色
                backgroundImage.color = new Color(0, 0, 0, 0.8f);
            }
            backgroundImage.gameObject.SetActive(true);
        }
        
        // 5. 设置文本内容
        if (deathTitleText != null)
        {
            if (string.IsNullOrEmpty(deathTitleText.text))
            {
                deathTitleText.text = "你死了";
            }
            deathTitleText.gameObject.SetActive(true);
        }
        
        if (deathMessageText != null)
        {
            if (string.IsNullOrEmpty(deathMessageText.text))
            {
                deathMessageText.text = "游戏结束";
            }
            deathMessageText.gameObject.SetActive(true);
        }
        
        // 6. 初始禁用按钮（等待动画完成）
        if (returnToMenuButton != null)
        {
            returnToMenuButton.interactable = false;
        }
        if (retryButton != null)
        {
            retryButton.interactable = false;
        }
        
        // 7. 播放淡入动画
        if (useFadeInAnimation && canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            StartCoroutine(FadeInCoroutine());
        }
        else
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }
            // 延迟启用按钮
            Invoke(nameof(EnableButtons), buttonEnableDelay);
        }
        
        // 8. 标记死亡界面已打开，禁用战斗交互
        if (Interactions.Instance != null)
        {
            Interactions.Instance.IsDeathUIOpen = true;
            Debug.Log("✅ 已设置 IsDeathUIOpen = true，战斗交互已禁用");
        }
        else
        {
            Debug.LogWarning("⚠️ Interactions.Instance 为空！无法禁用战斗交互。");
        }
        
        // 9. 强制刷新Canvas
        Canvas.ForceUpdateCanvases();
        
        Debug.Log("✅ 死亡界面显示完成！");
        Debug.Log("═══════════════════════════════════════════════════════════");
    }
    
    /// <summary>
    /// 淡入动画协程
    /// </summary>
    private System.Collections.IEnumerator FadeInCoroutine()
    {
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            }
            yield return null;
        }
        
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }
        
        // 淡入完成后启用按钮
        EnableButtons();
    }
    
    /// <summary>
    /// 启用按钮
    /// </summary>
    private void EnableButtons()
    {
        if (returnToMenuButton != null)
        {
            returnToMenuButton.interactable = true;
            Debug.Log("✅ 返回主菜单按钮已启用");
        }
        if (retryButton != null)
        {
            retryButton.interactable = true;
            Debug.Log("✅ 重试按钮已启用");
        }
    }
    
    /// <summary>
    /// 隐藏死亡界面
    /// </summary>
    public void HideUI()
    {
        isShowing = false;
        
        // 恢复战斗交互
        if (Interactions.Instance != null)
        {
            Interactions.Instance.IsDeathUIOpen = false;
            Debug.Log("✅ 已设置 IsDeathUIOpen = false，战斗交互已恢复");
        }
        
        if (content != null)
        {
            content.SetActive(false);
        }
        
        gameObject.SetActive(false);
        
        Debug.Log("💀 死亡界面已隐藏");
    }
    
    /// <summary>
    /// 返回主菜单按钮点击事件
    /// </summary>
    private void OnReturnToMenuClicked()
    {
        Debug.Log("═══════════════════════════════════════════════════════════");
        Debug.Log("🏠 【返回主菜单】玩家点击返回主菜单按钮");
        Debug.Log("═══════════════════════════════════════════════════════════");
        
        // 隐藏界面
        HideUI();
        
        // 加载主菜单场景
        string startSceneName = "StartScene";
        if (BattleFlowManager.Instance != null && !string.IsNullOrEmpty(BattleFlowManager.Instance.startSceneName))
        {
            startSceneName = BattleFlowManager.Instance.startSceneName;
        }
        
        Debug.Log($"🚀 正在加载主菜单场景: {startSceneName}");
        SceneManager.LoadScene(startSceneName);
    }
    
    /// <summary>
    /// 重试按钮点击事件（重新开始游戏）
    /// </summary>
    private void OnRetryClicked()
    {
        Debug.Log("═══════════════════════════════════════════════════════════");
        Debug.Log("🔄 【重试游戏】玩家点击重试按钮");
        Debug.Log("═══════════════════════════════════════════════════════════");
        
        // 隐藏界面
        HideUI();
        
        // 重新开始游戏（需要从 StartScene 开始，或者直接调用 GameManager.StartNewRun）
        // 这里我们返回主菜单，让玩家重新开始
        string startSceneName = "StartScene";
        if (BattleFlowManager.Instance != null && !string.IsNullOrEmpty(BattleFlowManager.Instance.startSceneName))
        {
            startSceneName = BattleFlowManager.Instance.startSceneName;
        }
        
        Debug.Log($"🚀 正在加载主菜单场景: {startSceneName}");
        SceneManager.LoadScene(startSceneName);
    }
}


