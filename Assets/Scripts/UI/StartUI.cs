using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// 开始界面：游戏启动和死亡后返回的界面
/// </summary>
public class StartUI : MonoBehaviour
{
    [Header("界面组件")]
    public GameObject content;              // 整个 Panel
    public Button startGameButton;          // 开始游戏按钮
    public TextMeshProUGUI titleText;       // 标题文本（可选）
    public TextMeshProUGUI subtitleText;    // 副标题文本（可选）

    [Header("游戏配置")]
    [Tooltip("默认英雄数据（用于开始新游戏）")]
    public HeroData defaultHeroData;

    private void Start()
    {
        Debug.Log("═══════════════════════════════════════════════════════════");
        Debug.Log("🏠 【开始界面】StartUI 初始化");
        Debug.Log("═══════════════════════════════════════════════════════════");
        
        // 确保界面显示
        if (content != null)
        {
            content.SetActive(true);
        }
        
        gameObject.SetActive(true);
        
        // 绑定开始游戏按钮事件
        if (startGameButton != null)
        {
            startGameButton.onClick.RemoveAllListeners();
            startGameButton.onClick.AddListener(OnStartGameClicked);
            Debug.Log("✅ 开始游戏按钮事件已绑定");
        }
        else
        {
            Debug.LogError("❌ StartUI: startGameButton 未设置！无法开始游戏。");
            Debug.LogError("💡 请在 StartUI 的 Inspector 中设置 Start Game Button 引用。");
        }
        
        // 检查 Canvas
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            Debug.Log($"✅ 找到父Canvas: {canvas.name}, SortOrder: {canvas.sortingOrder}");
        }
        else
        {
            Debug.LogWarning("⚠️ 未找到父Canvas组件！");
        }
        
        Debug.Log("═══════════════════════════════════════════════════════════");
    }

    /// <summary>
    /// 开始游戏按钮点击事件
    /// </summary>
    private void OnStartGameClicked()
    {
        Debug.Log("═══════════════════════════════════════════════════════════");
        Debug.Log("🎮 【开始游戏】玩家点击开始游戏按钮");
        Debug.Log("═══════════════════════════════════════════════════════════");
        
        // 检查 HeroData 是否设置
        if (defaultHeroData == null)
        {
            Debug.LogError("❌ StartUI: defaultHeroData 未设置！无法开始游戏。");
            Debug.LogError("💡 请在 StartUI 的 Inspector 中设置 Default Hero Data。");
            return;
        }
        
        Debug.Log($"📋 英雄数据:");
        Debug.Log($"   - 英雄名称: {defaultHeroData.name}");
        Debug.Log($"   - 初始血量: {defaultHeroData.Health}");
        Debug.Log($"   - 初始牌库: {defaultHeroData.Deck?.Count ?? 0} 张卡牌");
        
        // 检查 GameManager 是否存在
        if (GameManager.Instance == null)
        {
            Debug.LogError("❌ GameManager.Instance 为空！无法开始游戏。");
            Debug.LogError("💡 请确保场景中有 GameManager 组件，或检查 MonoSingleton 是否正确初始化。");
            return;
        }
        
        // 禁用按钮，防止重复点击
        if (startGameButton != null)
        {
            startGameButton.interactable = false;
            Debug.Log("   - 已禁用开始游戏按钮，防止重复点击");
        }
        
        // 开始新游戏
        Debug.Log("🚀 调用 GameManager.StartNewRun() 开始新游戏...");
        GameManager.Instance.StartNewRun(defaultHeroData);
        
        Debug.Log("═══════════════════════════════════════════════════════════");
    }
}


