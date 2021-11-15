// Copyright (c) 2022 Justin Couch / JustInvoke
using UnityEngine;
using System.Collections;

namespace PowerslideKartPhysics
{
    // This class controls a kart with standard input (Unity input manager)
    public class KartInputPlayer : KartInput
    {
        public override void Update() {
            // Set input based on static input values
            targetAccel = InputManager.accelInput;
            targetBrake = InputManager.brakeInput;
            targetSteer = InputManager.steerInput;
            drifting = InputManager.driftButton;
            boosting = InputManager.boostButton;

            if (InputManager.itemButtonDown) {
                PressItem();
            }

            base.Update();
        }
    }
}