using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ManaUI : MonoBehaviour
{
    [SerializeField] private TMP_Text mana;

    public void UpdateMana(int currentMana)
    {
        Debug.Log($"更新能量UI：{currentMana}");
        mana.text = currentMana.ToString(); 
    }
}
