// Copyright (c) 2022 Justin Couch / JustInvoke
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerslideKartPhysics
{
    // Class for triggers that respawn karts when out of bounds
    [DisallowMultipleComponent]
    public class OutOfBounds : MonoBehaviour
    {
        // A kart will respawn if it touches this trigger
        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.layer == LayerInfo.KartLayer) {
                RaceAgent kart = F.GetTopmostParentComponent<RaceAgent>(other.transform);
                if (kart != null) {
                    kart.Respawn();
                }
            }
        }
    }
}
