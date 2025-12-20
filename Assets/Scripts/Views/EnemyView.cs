using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyView : CombatantView
{
    [SerializeField] private TMP_Text attackText;
    public int AttackPower { get; set; }


    public void Setup(EnemyData enemyData)
    {
        Debug.Log($"生成敌人：{enemyData.name} HP:{enemyData.Health} ATK:{enemyData.AttackPower}");
        AttackPower = enemyData.AttackPower;
        UpdateAttackText();
        SetupBase(enemyData.Health, enemyData.Image);
    }

    private void UpdateAttackText()
    {
        attackText.text = "ATK:" + AttackPower;
    }
}
