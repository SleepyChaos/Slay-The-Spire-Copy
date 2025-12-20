using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// 胜利界面：显示Boss战斗胜利画面，遮挡整个屏幕
/// </summary>
public class VictoryUI : MonoBehaviour
{
    [Header("界面组件")]
    public GameObject content;              // 整个 Panel (用于遮挡全屏)
    public Image backgroundImage;           // 背景图片（半透明黑色或其他）
    public TextMeshProUGUI victoryTitleText; // 胜利标题文本（如"胜利"）
    public TextMeshProUGUI victoryMessageText; // 胜利信息文本（可选）
    public Button returnToMenuButton;      // 返回主菜单按钮

    [Header("设置")]
    [Tooltip("胜利界面显示后，延迟多少秒后允许点击按钮（用于显示动画）")]
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
    }

    /// <summary>
    /// 显示胜利界面
    /// </summary>
    public void Show()
    {
        if (isShowing)
        {
            Debug.LogWarning("⚠️ VictoryUI 已经在显示中，跳过重复显示");
            return;
        }
        
        Debug.Log("═══════════════════════════════════════════════════════════");
        Debug.Log("🎉 【胜利界面】开始显示胜利画面");
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
                canvas.sortingOrder = 200; // 胜利界面应该显示在所有其他UI之上
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
        if (victoryTitleText != null)
        {
            if (string.IsNullOrEmpty(victoryTitleText.text))
            {
                victoryTitleText.text = "胜利！";
            }
            victoryTitleText.gameObject.SetActive(true);
        }
        
        if (victoryMessageText != null)
        {
            if (string.IsNullOrEmpty(victoryMessageText.text))
            {
                victoryMessageText.text = "恭喜你完成了所有关卡！";
            }
            victoryMessageText.gameObject.SetActive(true);
        }
        
        // 6. 初始禁用按钮（等待动画完成）
        if (returnToMenuButton != null)
        {
            returnToMenuButton.interactable = false;
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
        
        // 8. 标记胜利界面已打开，禁用战斗交互
        if (Interactions.Instance != null)
        {
            Interactions.Instance.IsVictoryUIOpen = true;
            Debug.Log("✅ 已设置 IsVictoryUIOpen = true，战斗交互已禁用");
        }
        else
        {
            Debug.LogWarning("⚠️ Interactions.Instance 为空！无法禁用战斗交互。");
        }
        
        // 9. 强制刷新Canvas
        Canvas.ForceUpdateCanvases();
        
        Debug.Log("✅ 胜利界面显示完成！");
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
    }
    
    /// <summary>
    /// 隐藏胜利界面
    /// </summary>
    public void HideUI()
    {
        isShowing = false;
        
        // 恢复战斗交互
        if (Interactions.Instance != null)
        {
            Interactions.Instance.IsVictoryUIOpen = false;
            Debug.Log("✅ 已设置 IsVictoryUIOpen = false，战斗交互已恢复");
        }
        
        if (content != null)
        {
            content.SetActive(false);
        }
        
        gameObject.SetActive(false);
        
        Debug.Log("🎉 胜利界面已隐藏");
    }
    
    /// <summary>
    /// 返回主菜单按钮点击事件
    /// </summary>
    private void OnReturnToMenuClicked()
    {
        Debug.Log("═══════════════════════════════════════════════════════════");
        Debug.Log("🏠 【返回主菜单】玩家点击返回主菜单按钮");
        Debug.Log("═══════════════════════════════════════════════════════════");
        
        // 禁用按钮，防止重复点击
        if (returnToMenuButton != null)
        {
            returnToMenuButton.interactable = false;
        }
        
        // 隐藏界面
        HideUI();
        
        // 【关键修复】重置 GameManager 状态（清除游戏进度）
        if (GameManager.Instance != null)
        {
            Debug.Log("🔄 重置 GameManager 状态...");
            GameManager.Instance.CurrentStageIndex = 0;
            GameManager.Instance.CurrentHealth = 0;
            GameManager.Instance.MaxHealth = 0;
            GameManager.Instance.MasterDeck.Clear();
            GameManager.Instance.Relics.Clear();
            GameManager.Instance.NextEnemies = null;
            GameManager.Instance.IsEliteBattle = false;
            Debug.Log("✅ GameManager 状态已重置");
        }
        
        // 【关键修复】始终加载 StartScene，不依赖 BattleFlowManager 的配置
        string startSceneName = "StartScene";
        
        // 验证场景是否存在
        bool sceneExists = false;
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneName == startSceneName)
            {
                sceneExists = true;
                break;
            }
        }
        
        if (!sceneExists)
        {
            Debug.LogError($"❌ 场景 '{startSceneName}' 不存在于 Build Settings 中！");
            Debug.LogError("💡 请确保 StartScene 已添加到 Build Settings");
            // 如果 StartScene 不存在，尝试使用 BattleFlowManager 的配置作为后备
            if (BattleFlowManager.Instance != null && !string.IsNullOrEmpty(BattleFlowManager.Instance.startSceneName))
            {
                startSceneName = BattleFlowManager.Instance.startSceneName;
                Debug.LogWarning($"⚠️ 使用 BattleFlowManager 的配置作为后备: {startSceneName}");
            }
        }
        
        Debug.Log($"🚀 正在加载主菜单场景: {startSceneName}");
        Debug.Log("   - 这将卸载当前战斗场景，清除所有战斗状态");
        Debug.Log("   - 注意：始终使用 'StartScene'，不依赖 BattleFlowManager 配置");
        
        // 使用 LoadSceneMode.Single 确保完全卸载当前场景
        SceneManager.LoadScene(startSceneName, LoadSceneMode.Single);
        
        Debug.Log("═══════════════════════════════════════════════════════════");
    }
}


