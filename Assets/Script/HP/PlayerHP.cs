using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHP : NetworkBehaviour
{
    [Networked]
    public int HP { get; set; }

    const int startingHP = 5;
    private void Start()
    {
        HP = startingHP;
    }
    public void OnTakeDamage()
    {
        if (!Object.HasStateAuthority)
            return;

        HP -= 1;
    }
}