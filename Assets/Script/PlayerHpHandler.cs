using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpHandler : NetworkBehaviour
{
    [Networked(OnChanged = nameof(PlayerHit))]
    int Hp { get; set; }
    int MaxHp { get; set; }

    [SerializeField] Slider hpBar;
    [SerializeField] Image hpBarImage;
    [SerializeField] Image hpHealFillImage;
    [SerializeField] ParticleSystem playerParticle;
    // Start is called before the first frame update
    void Start()
    {
        hpBar = GetComponentInChildren<Slider>();
        hpBarImage = GetComponentInChildren<Image>();
        hpHealFillImage = GetComponentInChildren<Image>();
        playerParticle = GetComponentInChildren<ParticleSystem>();

        if (MaxHp == 0)
        {
            MaxHp = 200;
            HpReset();
        } 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    static void PlayerHit(Changed<PlayerHpHandler> changed)
    {
        int isFiringCurrent = changed.Behaviour.Hp;

        //Load the old value
        changed.LoadOld();

        int isFiringOld = changed.Behaviour.Hp;

        //if (isFiringCurrent != isFiringOld)
        //    changed.Behaviour.HpBarSet();
    }

    //void HpBarSet()
    //{

    //}

    void HpReset()
    {
        Hp = MaxHp;
    }
}
