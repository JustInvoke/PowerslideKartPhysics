// Copyright (c) 2022 Justin Couch / JustInvoke
using UnityEngine;
using System.Collections;

namespace PowerslideKartPhysics
{
    [CreateAssetMenu(fileName = "Kart Boost Preset", menuName = "Powerslide Kart Physics/Kart Preset/Kart Boost Preset", order = 8)]
    // Class for kart boost presets
    public class KartBoostPreset : ScriptableObject
    {
        public KartBoostType boostType = KartBoostType.DriftAuto;
        public bool canBoost = true;
        public float boostSpeedAdd = 10f;
        public float boostAccelAdd = 1.0f;
        public float boostDrive = 1.0f;
        public float boostPower = 1.0f;
        public int maxBoosts = 3;
        public float autoBoostInterval = 1.0f;
        [Range(0.0f, 1.0f)]
        public float driftManualBoostLimit = 0.5f;
        public bool driftManualFailCancel = true;
        public float boostRate = 1.0f;
        public float boostBurnRate = 1.0f;
        public float boostGroundPush = 10f;
        public float boostAirPush = 5.0f;
        public float airLandBoost = 2.0f;
        public float driftBoostAdd = 1.0f;
        public float boostAmount = 10f;
        public float boostAmountLimit = 10f;
        public float boostReserveLimit = Mathf.Infinity;
        public bool brakeCancelsBoost = true;
        public bool wallCollisionCancelsBoost = false;
        public float boostWheelie = 0.5f;
    }
}