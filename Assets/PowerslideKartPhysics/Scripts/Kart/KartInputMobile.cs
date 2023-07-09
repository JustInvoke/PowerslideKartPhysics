// Copyright (c) 2023 Justin Couch / JustInvoke

namespace PowerslideKartPhysics
{
    // This class controls a kart with UI buttons for mobile input
    public class KartInputMobile : KartInput
    {
        public override void Update() {
            // Set input based on static input values
            targetAccel = InputManager.MobileInput.accelInput;
            targetBrake = InputManager.MobileInput.brakeInput;
            targetSteer = InputManager.MobileInput.steerInput;
            drifting = InputManager.MobileInput.driftButton;
            boosting = InputManager.MobileInput.boostButton;

            if (InputManager.MobileInput.itemButtonDown) {
                PressItem();
            }

            base.Update();
        }
    }
}