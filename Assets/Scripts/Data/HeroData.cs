using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Hero")]
public class HeroData : ScriptableObject
{
    [field: SerializeField] public Sprite Image { get; private set; }
    [field: SerializeField] public int Health { get; private set; }
    [field: SerializeField] public List<CardData> Deck { get; private set; }//不同英雄持有的不同牌组
    [field: SerializeField] public List<PerkData> StartingPerks { get; private set; }
    static HeroData(){
        Debug.Log("HeroData 脚本已加载");
    }
}
