// Copyright (c) 2022 Justin Couch / JustInvoke
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerslideKartPhysics
{
    // Class for the finish line trigger
    [DisallowMultipleComponent]
    public class FinishLine : MonoBehaviour
    {
        public float minLapCompletion = 0.9f; // Minimum percentage of a lap that must be completed for the lap to count
        [System.NonSerialized]
        public Track track; // The track associated with the finish line trigger

        // When a kart touches the trigger, this class evaluates whether it has completed a valid lap
        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.layer == LayerMask.NameToLayer("Karts")) {
                RaceAgent finishedKart = F.GetTopmostParentComponent<RaceAgent>(other.transform);
                if (finishedKart != null) {
                    if (finishedKart.GetLapProgress() >= minLapCompletion) {
                        finishedKart.IncrementLap(track != null ? track.startPoint : null);
                    }
                }
            }
        }
    }
}
