// Copyright (c) 2022 Justin Couch / JustInvoke
using UnityEngine;
using System.Collections;

namespace PowerslideKartPhysics
{
    [CreateAssetMenu(fileName = "Kart Dimensions Preset", menuName = "Powerslide Kart Physics/Kart Preset/Kart Dimensions Preset", order = 0)]
    // Class for kart dimensions presets
    public class KartDimensionsPreset : ScriptableObject
    {
        public float rotationRateFactor = 10f;
        [Range(0.0f, 1.0f)]
        public float minRotationRate = 0.1f;
        [Range(0.0f, 1.0f)]
        public float maxRotationRate = 0.3f;
        [Range(0.0f, 1.0f)]
        public float visualRotationRate = 0.1f;
        public float airFlattenRate = 0.01f;
        public float frontLength = 1.0f;
        public float backLength = 1.0f;
        public float sideWidth = 1.0f;
        public Vector3 cornerCastSize = new Vector3(1.0f, 0.0f, 1.0f);
        public Vector3 cornerCastOffset = Vector3.zero;
        public bool oneCornerCastPerFrame = false;
        public float cornerCastDistance = 1.0f;
        public int maxCollisionContactPoints = 8;
        public float spinRate = 10f;
        public float spinHeight = 1.0f;
    }
}