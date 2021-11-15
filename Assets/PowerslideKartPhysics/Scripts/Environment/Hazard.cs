// Copyright (c) 2022 Justin Couch / JustInvoke
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerslideKartPhysics
{
    // Class for objects that cause karts to spin out upon collision
    public class Hazard : MonoBehaviour
    {
        public Kart.SpinAxis spinAxis = Kart.SpinAxis.Yaw;
        public int spinCount = 1;
    }
}
