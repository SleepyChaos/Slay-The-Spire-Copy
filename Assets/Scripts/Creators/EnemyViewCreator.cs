using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyViewCreator : MonoSingleton<EnemyViewCreator>
{
    [SerializeField] private EnemyView enemyViewPrefab;

    /// <summary>
    /// 生成新的敌人
    /// </summary>
    /// <param name="enemyData"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public EnemyView CreateEnemyView(EnemyData enemyData, Vector3 position, Quaternion rotation)
    {
        Debug.Log($"创建敌人视图：{enemyData.name}");
        EnemyView enemyView = Instantiate(enemyViewPrefab, position, rotation);
        enemyView.Setup(enemyData);
        return enemyView;
    }
}
