using System;
using System.Collections;
using System.Collections.Generic;
using DeepGame.Quota;
using UnityEngine;

public class BubbleParticleTrigger : MonoBehaviour
{
    [SerializeField] private float _bubbleAir;
   
    private void OnParticleCollision(GameObject other)
    {
        ShipAir shipAir = other.GetComponent<ShipAir>();
        if (shipAir != null)
        {
            shipAir.AddAir(_bubbleAir);
        }
    }
}
