using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class RewardUI : MonoBehaviour
{
    [Header("界面组件")]
    public GameObject content;        // 整个 Panel (用于遮挡)
    public Transform cardContainer;   // 卡牌排列的父节点
    public Button skipButton;         // [新增] 下一关/跳过按钮

    [Header("预制体引用")]
    public RewardCardAdapter containerPrefab; // 透明壳子 (RewardCardContainer)
    public CardView cardViewPrefab;           // 战斗用的卡牌预制体 (CardView)

    private void Start()
    {
        // 初始化时隐藏UI
        HideUI();
        
        // 绑定跳过按钮事件
        if (skipButton != null)
        {
            skipButton.onClick.RemoveAllListeners();
            skipButton.onClick.AddListener(OnSkipClicked);
        }
    }

    public void Show()
    {
        Debug.Log("🎁 【RewardUI】Show 被调用！准备显示奖励...");
        Debug.Log($"📍 RewardUI GameObject: {gameObject.name}, 当前状态: Active={gameObject.activeSelf}");
        
        // 0. 确保父对象（Canvas）也是激活的
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
        
        // 检查Canvas组件
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            Debug.Log($"✅ 找到父Canvas: {canvas.name}, Active={canvas.gameObject.activeSelf}, SortOrder={canvas.sortingOrder}, RenderMode={canvas.renderMode}");
            if (!canvas.gameObject.activeSelf)
            {
                canvas.gameObject.SetActive(true);
                Debug.Log("✅ 已激活父Canvas");
            }
            
            // 【关键修复】如果Canvas是Screen Space - Overlay，需要改为Screen Space - Camera才能显示SpriteRenderer
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                Debug.LogWarning("⚠️ Canvas使用Screen Space - Overlay模式，SpriteRenderer无法显示！");
                Debug.Log("🔧 尝试将Canvas改为Screen Space - Camera模式...");
                
                // 查找Main Camera
                Camera mainCamera = Camera.main;
                if (mainCamera == null)
                {
                    mainCamera = FindObjectOfType<Camera>();
                }
                
                if (mainCamera != null)
                {
                    canvas.renderMode = RenderMode.ScreenSpaceCamera;
                    canvas.worldCamera = mainCamera;
                    canvas.planeDistance = 10f; // 设置合适的距离
                    Debug.Log($"✅ 已将Canvas改为Screen Space - Camera模式，使用Camera: {mainCamera.name}");
                }
                else
                {
                    Debug.LogError("❌ 找不到Camera！无法将Canvas改为Screen Space - Camera模式。");
                }
            }
            
            // 确保Canvas的Sort Order足够高，显示在最上层
            if (canvas.sortingOrder < 100)
            {
                canvas.sortingOrder = 100;
                Debug.Log($"✅ 已设置Canvas Sort Order为 {canvas.sortingOrder}");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ 未找到父Canvas组件！");
        }
        
        // 1. 标记奖励界面已打开，禁用战斗交互
        if (Interactions.Instance != null)
        {
            Interactions.Instance.IsRewardUIOpen = true;
            Debug.Log("✅ 已设置 IsRewardUIOpen = true");
        }
        else
        {
            Debug.LogWarning("⚠️ Interactions.Instance 为空！");
        }
        
        // 2. 显示遮挡层和整个UI
        gameObject.SetActive(true);
        Debug.Log($"✅ 已激活 GameObject: {gameObject.name}, 激活后状态: {gameObject.activeSelf}");
        
        // 检查RectTransform，确保大小和位置正确
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            Debug.Log($"📐 RectTransform: SizeDelta={rectTransform.sizeDelta}, AnchoredPosition={rectTransform.anchoredPosition}");
            // 如果SizeDelta为0，说明可能没有正确设置大小
            if (rectTransform.sizeDelta.x < 10 || rectTransform.sizeDelta.y < 10)
            {
                Debug.LogWarning("⚠️ RectTransform大小异常，可能需要检查Anchor设置");
            }
        }
        
        // 检查Image组件，确保可见
        UnityEngine.UI.Image image = GetComponent<UnityEngine.UI.Image>();
        if (image != null)
        {
            Color imgColor = image.color;
            Debug.Log($"📸 Image组件: Color={imgColor}, Alpha={imgColor.a}, Enabled={image.enabled}");
            if (imgColor.a < 0.1f)
            {
                imgColor.a = 1f;
                image.color = imgColor;
                Debug.Log("✅ 已修复Image透明度");
            }
            if (!image.enabled)
            {
                image.enabled = true;
                Debug.Log("✅ 已启用Image组件");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ RewardPanel 上没有Image组件！");
        }
        
        if (content != null)
        {
            content.SetActive(true);
            Debug.Log($"✅ 已激活 Content: {content.name}, 激活后状态: {content.activeSelf}");
        }
        else
        {
            Debug.LogWarning("⚠️ Content 引用为空！将使用 gameObject 作为内容");
            // 如果content为空，说明content就是gameObject本身，已经激活了
        }
        
        // 强制刷新Canvas
        Canvas.ForceUpdateCanvases();
        Debug.Log("🔄 已强制刷新Canvas");
        
        // 3. 确保跳过按钮可见
        if (skipButton != null)
        {
            skipButton.gameObject.SetActive(true);
            Debug.Log($"✅ 已激活 SkipButton: {skipButton.gameObject.name}");
        }
        else
        {
            Debug.LogWarning("⚠️ SkipButton 引用为空！");
        }
        
        // 4. 检查必要组件
        if (cardContainer == null)
        {
            Debug.LogError("❌ CardContainer 引用为空！无法生成卡牌奖励。");
            Debug.LogError("💡 请在 RewardUI 的 Inspector 中设置 Card Container 引用。");
            return;
        }
        
        if (containerPrefab == null)
        {
            Debug.LogError("❌ ContainerPrefab 引用为空！无法生成卡牌奖励。");
            Debug.LogError("💡 请在 RewardUI 的 Inspector 中设置 Container Prefab 引用。");
            return;
        }
        
        if (cardViewPrefab == null)
        {
            Debug.LogError("❌ CardViewPrefab 引用为空！无法生成卡牌奖励。");
            Debug.LogError("💡 请在 RewardUI 的 Inspector 中设置 Card View Prefab 引用。");
            return;
        }

        // 5. 生成卡牌
        Debug.Log("🎴 开始生成奖励卡牌...");
        GenerateCardRewards();
        
        // 6. 延迟一帧后再次检查状态（确保Unity已更新）
        StartCoroutine(VerifyUIState());
        
        Debug.Log("✅ 奖励UI显示完成！");
    }
    
    /// <summary>
    /// 验证UI状态，确保所有组件都正确显示
    /// </summary>
    private System.Collections.IEnumerator VerifyUIState()
    {
        yield return null; // 等待一帧
        
        Debug.Log("🔍 验证UI状态...");
        Debug.Log($"  - GameObject Active: {gameObject.activeSelf}");
        Debug.Log($"  - GameObject ActiveInHierarchy: {gameObject.activeInHierarchy}");
        
        if (content != null)
        {
            Debug.Log($"  - Content Active: {content.activeSelf}");
            Debug.Log($"  - Content ActiveInHierarchy: {content.activeInHierarchy}");
        }
        
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            Debug.Log($"  - Canvas Active: {canvas.gameObject.activeSelf}");
            Debug.Log($"  - Canvas SortOrder: {canvas.sortingOrder}");
        }
        
        UnityEngine.UI.Image image = GetComponent<UnityEngine.UI.Image>();
        if (image != null)
        {
            Debug.Log($"  - Image Enabled: {image.enabled}");
            Debug.Log($"  - Image Color Alpha: {image.color.a}");
        }
        
        // 如果GameObject未激活，再次尝试激活
        if (!gameObject.activeInHierarchy)
        {
            Debug.LogWarning("⚠️ GameObject未在层级中激活，尝试再次激活...");
            gameObject.SetActive(false);
            yield return null;
            gameObject.SetActive(true);
            Canvas.ForceUpdateCanvases();
            Debug.Log("✅ 已重新激活GameObject");
        }
    }
    
    private void HideUI()
    {
        if (content != null) content.SetActive(false);
        else gameObject.SetActive(false);
    }

    private void GenerateCardRewards()
    {
        Debug.Log($"🎴 GenerateCardRewards 开始，CardContainer: {(cardContainer != null ? cardContainer.name : "NULL")}");
        
        // 清理旧卡牌
        if (cardContainer != null)
        {
            int childCount = cardContainer.childCount;
            Debug.Log($"🧹 清理旧卡牌，当前子对象数量: {childCount}");
            foreach(Transform child in cardContainer) 
            {
                if (child != null)
                    Destroy(child.gameObject);
            }
        }

        // 检查卡池
        if (GameManager.Instance == null)
        {
            Debug.LogError("❌ 【RewardUI】GameManager.Instance 为空！");
            return;
        }
        
        if (GameManager.Instance.AllCardsLibrary == null || GameManager.Instance.AllCardsLibrary.Count == 0)
        {
            Debug.LogError("❌ 【RewardUI】卡池为空！无法生成奖励。");
            Debug.LogError($"💡 请在 GameManager 的 Inspector 中设置 AllCardsLibrary，当前数量: {(GameManager.Instance.AllCardsLibrary == null ? "NULL" : GameManager.Instance.AllCardsLibrary.Count.ToString())}");
            return;
        }

        List<CardData> library = GameManager.Instance.AllCardsLibrary;
        Debug.Log($"✅ 卡池中有 {library.Count} 张卡牌");
        
        // 随机抽3张
        for (int i = 0; i < 3; i++)
        {
            CardData randomCard = library[Random.Range(0, library.Count)];
            Debug.Log($"🎴 生成第 {i + 1} 张奖励卡: {randomCard.name}");
            
            // 实例化适配器容器
            if(containerPrefab == null) 
            { 
                Debug.LogError("❌ ContainerPrefab 为空！无法生成奖励卡牌。");
                return; 
            }
            
            RewardCardAdapter adapter = Instantiate(containerPrefab, cardContainer);
            Debug.Log($"✅ 已实例化 RewardCardAdapter: {adapter.name}");
            
            // 初始化：传入 CardView 预制体 和 数据
            if (cardViewPrefab == null)
            {
                Debug.LogError("❌ CardViewPrefab 为空！无法设置奖励卡牌。");
                Destroy(adapter.gameObject);
                continue;
            }
            
            adapter.Setup(cardViewPrefab, randomCard, PickCard);
            Debug.Log($"✅ 已设置奖励卡牌: {randomCard.name}");
        }
        
        Debug.Log("🎉 所有奖励卡牌生成完成！");
        Debug.Log("💡 提示：点击卡牌即可选择，或点击跳过按钮跳过奖励");
    }

    // 玩家点击了某张卡
    private void PickCard(CardData card)
    {
        Debug.Log("═══════════════════════════════════════════════════════════");
        Debug.Log("🎴 【选择奖励卡牌】玩家选择了奖励卡牌");
        Debug.Log("═══════════════════════════════════════════════════════════");
        
        if (card == null)
        {
            Debug.LogError("❌ 卡牌数据为空！无法添加到牌堆。");
            return;
        }
        
        Debug.Log($"📋 选择的卡牌信息:");
        Debug.Log($"   - 卡牌名称: {card.name}");
        Debug.Log($"   - 卡牌描述: {card.Description}");
        Debug.Log($"   - 卡牌费用: {card.Mana}");
        
        if (GameManager.Instance == null)
        {
            Debug.LogError("❌ GameManager.Instance 为空！无法添加卡牌到牌堆。");
            return;
        }
        
        // 记录添加前的牌堆数量
        int deckCountBefore = GameManager.Instance.MasterDeck.Count;
        Debug.Log($"   - 添加前牌堆数量: {deckCountBefore}");
        
        // 添加卡牌到牌堆
        GameManager.Instance.MasterDeck.Add(card);
        
        // 验证是否成功添加
        int deckCountAfter = GameManager.Instance.MasterDeck.Count;
        Debug.Log($"   - 添加后牌堆数量: {deckCountAfter}");
        
        if (deckCountAfter > deckCountBefore)
        {
            Debug.Log($"✅ 卡牌 '{card.name}' 已成功添加到牌堆！");
            
            // 验证卡牌是否真的在牌堆中
            bool cardInDeck = GameManager.Instance.MasterDeck.Contains(card);
            if (cardInDeck)
            {
                Debug.Log($"✅ 验证成功：卡牌 '{card.name}' 确实在牌堆中");
            }
            else
            {
                Debug.LogError($"❌ 验证失败：卡牌 '{card.name}' 不在牌堆中！");
            }
        }
        else
        {
            Debug.LogError($"❌ 卡牌添加失败！牌堆数量没有增加。");
        }
        
        Debug.Log("═══════════════════════════════════════════════════════════");
        
        // 进入下一关
        GoToNextLevel();
    }

    // 玩家点击了“下一关” (不选卡)
    private void OnSkipClicked()
    {
        Debug.Log("⏩ 玩家选择跳过奖励");
        GoToNextLevel();
    }

    private void GoToNextLevel()
    {
        Debug.Log("═══════════════════════════════════════════════════════════");
        Debug.Log("🚀 【进入下一关】RewardUI 处理进入下一关");
        Debug.Log("═══════════════════════════════════════════════════════════");
        
        if (GameManager.Instance != null)
        {
            Debug.Log($"   - 当前关卡索引: {GameManager.Instance.CurrentStageIndex}");
            Debug.Log($"   - 玩家血量: {GameManager.Instance.CurrentHealth}/{GameManager.Instance.MaxHealth}");
        }
        
        // 1. 恢复战斗交互
        if (Interactions.Instance != null)
        {
            Interactions.Instance.IsRewardUIOpen = false;
            Debug.Log("   - 已恢复战斗交互");
        }
        
        // 2. 隐藏界面
        HideUI();
        Debug.Log("   - 已隐藏奖励界面");

        // 3. 通知 GameManager 进入下一关
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AdvanceStage();
        }
        else
        {
            Debug.LogError("❌ GameManager.Instance 为空！无法进入下一关。");
            return;
        }
        
        // 4. 检查是否使用固定顺序流程
        if (EncounterManager.Instance != null && EncounterManager.Instance.useFixedSequence)
        {
            // 固定顺序流程：直接进入下一关战斗
            int nextStage = GameManager.Instance.CurrentStageIndex;
            Debug.Log($"   - 固定顺序流程: 准备进入关卡 {nextStage}");
            
            // 检查是否还有下一关
            if (EncounterManager.Instance.HasNextEncounter(nextStage - 1))
            {
                EncounterData nextEncounter = EncounterManager.Instance.GetEncounterByIndex(nextStage);
                if (nextEncounter != null && BattleFlowManager.Instance != null)
                {
                    Debug.Log($"   - 下一关遭遇: {nextEncounter.encounterName} ({nextEncounter.encounterType})");
                    Debug.Log("🎯 固定顺序流程：直接进入下一关战斗");
                    BattleFlowManager.Instance.StartBattle(nextEncounter);
                    return;
                }
                else
                {
                    Debug.LogError($"❌ 无法获取关卡 {nextStage} 的遭遇数据！");
                }
            }
            else
            {
                Debug.Log("═══════════════════════════════════════════════════════════");
                Debug.Log("🎉 【游戏完成】所有关卡已完成！");
                Debug.Log("═══════════════════════════════════════════════════════════");
                // 可以在这里添加胜利逻辑，比如返回主菜单或显示胜利界面
                if (BattleFlowManager.Instance != null)
                {
                    BattleFlowManager.Instance.GoToStartScene();
                }
                else
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene("StartScene");
                }
                return;
            }
        }
        
        // 5. 随机选关模式：返回关卡选择界面
        Debug.Log("   - 随机选关模式: 返回选关界面");
        if (BattleFlowManager.Instance != null)
        {
            BattleFlowManager.Instance.ReturnToLevelSelect();
        }
        else
        {
            // 如果没有 BattleFlowManager，直接加载场景
            UnityEngine.SceneManagement.SceneManager.LoadScene("LevelSelectScene");
        }
    }
}