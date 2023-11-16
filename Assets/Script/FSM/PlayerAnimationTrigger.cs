using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTrigger : NetworkBehaviour
{
    PlayerStateHandler player;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponentInParent<PlayerStateHandler>();
        animator = transform.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void AnimationTrigger()
    {
        if (Object.HasInputAuthority)
        {
            player.animationTrigger = false;
        }
    }

}
