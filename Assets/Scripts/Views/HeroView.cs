using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeroView : CombatantView
{


    public void Setup(HeroData heroData)
    {
        Debug.Log($"初始化英雄：{heroData.name}");
        SetupBase(heroData.Health, heroData.Image);
    }
}
