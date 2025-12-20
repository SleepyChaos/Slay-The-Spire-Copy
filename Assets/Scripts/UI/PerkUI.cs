using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerkUI : MonoBehaviour
{
    [SerializeField] private Image image;
    public Perk Perk {get; private set; }


    public void Setup(Perk perk)
    {
        Perk = perk;
        image.sprite = perk.Image;
        Debug.Log("天赋UI：设置天赋图标");
    }
}
