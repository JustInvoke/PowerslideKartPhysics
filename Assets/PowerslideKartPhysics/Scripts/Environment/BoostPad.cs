// Copyright (c) 2022 Justin Couch / JustInvoke
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerslideKartPhysics
{
    // Class for boost pads
    [DisallowMultipleComponent]
    public class BoostPad : MonoBehaviour
    {
        public float boostAmount = 1.0f;
        public float boostForce = 1.0f;
        public float delayInterval = 1.0f;
        public bool continuous = false;
    }
}
