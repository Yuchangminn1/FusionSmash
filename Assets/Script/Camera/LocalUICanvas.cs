using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LocalUICanvas : MonoBehaviour
{
    Slider hpSlider;
    void Awake()
    {
        hpSlider = gameObject.GetComponentInChildren<Slider>();
    }

    public void ChangeHPBar(int _hp, int _maxHP, Transform transform )
    {
        hpSlider.value = (float)_hp / (float)_maxHP;
        Debug.Log($"{transform.name} : MaxHp{_maxHP} , _currentHp{_hp}  hpBar.value = {hpSlider.value}");

    }
} 
