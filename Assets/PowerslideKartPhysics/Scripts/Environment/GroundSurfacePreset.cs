// Copyright (c) 2022 Justin Couch / JustInvoke
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerslideKartPhysics
{
    // Class for storing ground surface information
    [CreateAssetMenu(fileName = "Ground Surface Preset", menuName = "Powerslide Kart Physics/Ground Surface Preset")]
    public class GroundSurfacePreset : ScriptableObject
    {
        public float friction = 1.0f;
        public bool useColliderFriction = false;
        public float speed = 1.0f;
        public Material tireMarkMaterial;
        public bool alwaysSlide = false;
        public AudioClip tireSnd;
    }
}
