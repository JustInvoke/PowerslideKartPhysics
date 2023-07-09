// Copyright (c) 2023 Justin Couch / JustInvoke
using UnityEngine.InputSystem.Controls;
using static UnityEngine.InputSystem.InputAction;

namespace PowerslideKartPhysics
{
    // This class controls a kart using the Input System
    // These functions must be hooked up to a Player Input component
    public class KartInputSystem : KartInput
    {
        public void OnAccel(CallbackContext context) {
            targetAccel = context.ReadValue<float>();
        }

        public void OnBrake(CallbackContext context) {
            targetBrake = context.ReadValue<float>();
        }

        public void OnSteer(CallbackContext context) {
            targetSteer = context.ReadValue<float>();
        }

        public void OnDrift(CallbackContext context) {
            ButtonControl button = context.control as ButtonControl;
            if (button != null) {
                drifting = button.isPressed;
            }
            else {
                drifting = false;
            }
        }

        public void OnBoost(CallbackContext context) {
            ButtonControl button = context.control as ButtonControl;
            if (button != null) {
                boosting = button.isPressed;
            }
            else {
                boosting = false;
            }
        }

        public void OnItemPress(CallbackContext context) {
            ButtonControl button = context.control as ButtonControl;
            if (button != null && button.wasPressedThisFrame) {
                PressItem();
            }
        }
    }
}