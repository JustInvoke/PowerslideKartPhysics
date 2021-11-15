// Copyright (c) 2022 Justin Couch / JustInvoke
using UnityEngine;
using System.Collections;

namespace PowerslideKartPhysics
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Kart))]
    // This class connects a kart to some kind of input, usually should be treated as an abstract class
    // The demo uses derived classes KartInputPlayer and BasicWaypointFollower as examples/implementations
    public class KartInput : MonoBehaviour
    {
        protected Kart theKart;
        protected ItemCaster caster;
        protected float targetAccel = 0.0f;
        protected float targetBrake = 0.0f;
        protected float targetSteer = 0.0f;
        protected bool drifting = false;
        protected bool boosting = false;

        public virtual void Awake() {
            theKart = GetComponent<Kart>();
            caster = GetComponent<ItemCaster>();
        }

        // Set kart input based on variables
        public virtual void Update() {
            if (theKart != null) {
                theKart.SetAccel(targetAccel);
                theKart.SetBrake(targetBrake);
                theKart.SetSteer(targetSteer);
                theKart.SetDrift(drifting);
                theKart.SetBoost(boosting);
            }
        }

        // Cast an equipped item upon input press
        protected void PressItem() {
            if (caster != null) {
                caster.Cast();
            }
        }
    }
}