using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text stackCountText;


    public void Set(Sprite sprite, int stackCount)
    {
        Debug.Log($"设置状态图标：{sprite?.name ?? "无"} 层数 {stackCount}");
        image.sprite = sprite;
        stackCountText.text = stackCount.ToString();
    }
}
