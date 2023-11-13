using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTrigger : MonoBehaviour
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
        player.animationTrigger = false;
    }

}
