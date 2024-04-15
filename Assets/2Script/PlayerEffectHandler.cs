using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectHandler : MonoBehaviour
{


    public ParticleSystem flyParticle;
    
    public void PlayParticle()
    {
        flyParticle.Play();

    }
    public void StopParticle()
    {
        flyParticle.Stop();
    }
}
