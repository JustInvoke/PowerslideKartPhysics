// Copyright (c) 2022 Justin Couch / JustInvoke
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerslideKartPhysics
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Kart))]
    // This class controls saving to and loading from presets for kart properties
    public class KartPresetControl : MonoBehaviour
    {
        Kart kart;
        public bool loadOnAwake = false;
        public KartDimensionsPreset dimensionsPreset;
        public KartSpeedPreset speedPreset;
        public KartSteerPreset steerPreset;
        public KartSuspensionPreset suspensionPreset;
        public KartWheelsPreset wheelsPreset;
        public KartJumpPreset jumpPreset;
        public KartGravityPreset gravityPreset;
        public KartDriftPreset driftPreset;
        public KartBoostPreset boostPreset;
        public KartWallsPreset wallsPreset;

        private void Awake() {
            kart = GetComponent<Kart>();
            if (loadOnAwake && Application.isPlaying) {
                LoadDimensionsPreset(dimensionsPreset);
                LoadSpeedPreset(speedPreset);
                LoadSteerPreset(steerPreset);
                LoadSuspensionPreset(suspensionPreset);
                LoadWheelsPreset(wheelsPreset);
                LoadJumpPreset(jumpPreset);
                LoadGravityPreset(gravityPreset);
                LoadDriftPreset(driftPreset);
                LoadBoostPreset(boostPreset);
                LoadWallsPreset(wallsPreset);
            }
        }

        public void LoadDimensionsPreset(KartDimensionsPreset preset) {
            kart = GetComponent<Kart>();
            if (kart == null || preset == null) { return; }

            kart.rotationRateFactor = preset.rotationRateFactor;
            kart.minRotationRate = preset.minRotationRate;
            kart.maxRotationRate = preset.maxRotationRate;
            kart.visualRotationRate = preset.visualRotationRate;
            kart.airFlattenRate = preset.airFlattenRate;
            kart.frontLength = preset.frontLength;
            kart.backLength = preset.backLength;
            kart.sideWidth = preset.sideWidth;
            kart.cornerCastSize = preset.cornerCastSize;
            kart.cornerCastOffset = preset.cornerCastOffset;
            kart.oneCornerCastPerFrame = preset.oneCornerCastPerFrame;
            kart.cornerCastDistance = preset.cornerCastDistance;
            kart.maxCollisionContactPoints = preset.maxCollisionContactPoints;
            kart.spinRate = preset.spinRate;
            kart.spinHeight = preset.spinHeight;
        }

        public void SaveDimensionsPreset(KartDimensionsPreset preset) {
            kart = GetComponent<Kart>();
            if (kart == null || preset == null) { return; }

            preset.rotationRateFactor = kart.rotationRateFactor;
            preset.minRotationRate = kart.minRotationRate;
            preset.maxRotationRate = kart.maxRotationRate;
            preset.visualRotationRate = kart.visualRotationRate;
            preset.airFlattenRate = kart.airFlattenRate;
            preset.frontLength = kart.frontLength;
            preset.backLength = kart.backLength;
            preset.sideWidth = kart.sideWidth;
            preset.cornerCastSize = kart.cornerCastSize;
            preset.cornerCastOffset = kart.cornerCastOffset;
            preset.oneCornerCastPerFrame = kart.oneCornerCastPerFrame;
            preset.cornerCastDistance = kart.cornerCastDistance;
            preset.maxCollisionContactPoints = kart.maxCollisionContactPoints;
            preset.spinRate = kart.spinRate;
            preset.spinHeight = kart.spinHeight;
            SaveAssets(preset);
        }

        public void LoadSpeedPreset(KartSpeedPreset preset) {
            kart = GetComponent<Kart>();
            if (kart == null || preset == null) { return; }

            kart.maxSpeed = preset.maxSpeed;
            kart.maxReverseSpeed = preset.maxReverseSpeed;
            kart.acceleration = preset.acceleration;
            kart.brakeForce = preset.brakeForce;
            kart.coastingFriction = preset.coastingFriction;
            kart.slopeFriction = preset.slopeFriction;
            kart.airDriveFriction = preset.airDriveFriction;
            kart.autoStopSpeed = preset.autoStopSpeed;
            kart.autoStopForce = preset.autoStopForce;
            kart.autoStopNormalDotLimit = preset.autoStopNormalDotLimit;
            kart.maxFallSpeed = preset.maxFallSpeed;
            kart.spinDecel = preset.spinDecel;
        }

        public void SaveSpeedPreset(KartSpeedPreset preset) {
            kart = GetComponent<Kart>();
            if (kart == null || preset == null) { return; }

            preset.maxSpeed = kart.maxSpeed;
            preset.maxReverseSpeed = kart.maxReverseSpeed;
            preset.acceleration = kart.acceleration;
            preset.brakeForce = kart.brakeForce;
            preset.coastingFriction = kart.coastingFriction;
            preset.slopeFriction = kart.slopeFriction;
            preset.airDriveFriction = kart.airDriveFriction;
            preset.autoStopSpeed = kart.autoStopSpeed;
            preset.autoStopForce = kart.autoStopForce;
            preset.autoStopNormalDotLimit = kart.autoStopNormalDotLimit;
            preset.maxFallSpeed = kart.maxFallSpeed;
            preset.spinDecel = kart.spinDecel;
            SaveAssets(preset);
        }

        public void LoadSteerPreset(KartSteerPreset preset) {
            kart = GetComponent<Kart>();
            if (kart == null || preset == null) { return; }

            kart.steerRate = preset.steerRate;
            kart.maxSteer = preset.maxSteer;
            kart.minSteer = preset.minSteer;
            kart.airSteer = preset.airSteer;
            kart.steerSpeedLimit = preset.steerSpeedLimit;
            kart.steerSlowLimit = preset.steerSlowLimit;
            kart.brakeSteerIncrease = preset.brakeSteerIncrease;
            kart.dontInvertSteerReverseAccel = preset.dontInvertSteerReverseAccel;
            kart.visualSteerRate = preset.visualSteerRate;
            kart.visualSteerSpeedLimit = preset.visualSteerSpeedLimit;
            kart.turnTiltAmount = preset.turnTiltAmount;
            kart.turnTiltReferenceSpeed = preset.turnTiltReferenceSpeed;
            kart.turnTiltRate = preset.turnTiltRate;
            kart.turnTiltSideOffsetFactor = preset.turnTiltSideOffsetFactor;
            kart.invertTurnTiltHeightOffset = preset.invertTurnTiltHeightOffset;
            kart.localTiltOffsetCompensation = preset.localTiltOffsetCompensation;
            kart.accelTiltAmount = preset.accelTiltAmount;
            kart.sidewaysFriction = preset.sidewaysFriction;
            kart.airSidewaysFriction = preset.airSidewaysFriction;
            kart.brakeSlipAmount = preset.brakeSlipAmount;
        }

        public void SaveSteerPreset(KartSteerPreset preset) {
            kart = GetComponent<Kart>();
            if (kart == null || preset == null) { return; }

            preset.steerRate = kart.steerRate;
            preset.maxSteer = kart.maxSteer;
            preset.minSteer = kart.minSteer;
            preset.airSteer = kart.airSteer;
            preset.steerSpeedLimit = kart.steerSpeedLimit;
            preset.steerSlowLimit = kart.steerSlowLimit;
            preset.brakeSteerIncrease = kart.brakeSteerIncrease;
            preset.dontInvertSteerReverseAccel = kart.dontInvertSteerReverseAccel;
            preset.visualSteerRate = kart.visualSteerRate;
            preset.visualSteerSpeedLimit = kart.visualSteerSpeedLimit;
            preset.turnTiltAmount = kart.turnTiltAmount;
            preset.turnTiltReferenceSpeed = kart.turnTiltReferenceSpeed;
            preset.turnTiltRate = kart.turnTiltRate;
            preset.turnTiltSideOffsetFactor = kart.turnTiltSideOffsetFactor;
            preset.invertTurnTiltHeightOffset = kart.invertTurnTiltHeightOffset;
            preset.localTiltOffsetCompensation = kart.localTiltOffsetCompensation;
            preset.accelTiltAmount = kart.accelTiltAmount;
            preset.sidewaysFriction = kart.sidewaysFriction;
            preset.airSidewaysFriction = kart.airSidewaysFriction;
            preset.brakeSlipAmount = kart.brakeSlipAmount;
            SaveAssets(preset);
        }

        public void LoadSuspensionPreset(KartSuspensionPreset preset) {
            kart = GetComponent<Kart>();
            if (kart == null || preset == null) { return; }

            kart.springForce = preset.springForce;
            kart.springDampening = preset.springDampening;
            kart.springDampVelMin = preset.springDampVelMin;
            kart.springDampVelMax = preset.springDampVelMax;
            kart.compressionSpringFactor = preset.compressionSpringFactor;
            kart.groundStickForce = preset.groundStickForce;
            kart.groundStickCompression = preset.groundStickCompression;
        }

        public void SaveSuspensionPreset(KartSuspensionPreset preset) {
            kart = GetComponent<Kart>();
            if (kart == null || preset == null) { return; }

            preset.springForce = kart.springForce;
            preset.springDampening = kart.springDampening;
            preset.springDampVelMin = kart.springDampVelMin;
            preset.springDampVelMax = kart.springDampVelMax;
            preset.compressionSpringFactor = kart.compressionSpringFactor;
            preset.groundStickForce = kart.groundStickForce;
            preset.groundStickCompression = kart.groundStickCompression;
            SaveAssets(preset);
        }

        public void LoadWheelsPreset(KartWheelsPreset preset) {
            kart = GetComponent<Kart>();
            if (kart == null || preset == null) { return; }

            kart.wheelCastMask = preset.wheelCastMask;
            kart.maxWheelCastHits = preset.maxWheelCastHits;
            kart.oneWheelCastPerFrame = preset.oneWheelCastPerFrame;
            kart.groundNormalSmoothRate = preset.groundNormalSmoothRate;
        }

        public void SaveWheelsPreset(KartWheelsPreset preset) {
            kart = GetComponent<Kart>();
            if (kart == null || preset == null) { return; }

            preset.wheelCastMask = kart.wheelCastMask;
            preset.maxWheelCastHits = kart.maxWheelCastHits;
            preset.oneWheelCastPerFrame = kart.oneWheelCastPerFrame;
            preset.groundNormalSmoothRate = kart.groundNormalSmoothRate;
            SaveAssets(preset);
        }

        public void LoadJumpPreset(KartJumpPreset preset) {
            kart = GetComponent<Kart>();
            if (kart == null || preset == null) { return; }

            kart.canJump = preset.canJump;
            kart.jumpForce = preset.jumpForce;
            kart.jumpDuration = preset.jumpDuration;
            kart.jumpStickForce = preset.jumpStickForce;
            kart.airJumpTimeLimit = preset.airJumpTimeLimit;
        }

        public void SaveJumpPreset(KartJumpPreset preset) {
            kart = GetComponent<Kart>();
            if (kart == null || preset == null) { return; }

            preset.canJump = kart.canJump;
            preset.jumpForce = kart.jumpForce;
            preset.jumpDuration = kart.jumpDuration;
            preset.jumpStickForce = kart.jumpStickForce;
            preset.airJumpTimeLimit = kart.airJumpTimeLimit;
            SaveAssets(preset);
        }

        public void LoadGravityPreset(KartGravityPreset preset) {
            kart = GetComponent<Kart>();
            if (kart == null || preset == null) { return; }

            kart.gravityAdd = preset.gravityAdd;
            kart.gravityDir = preset.gravityDir;
            kart.gravityIsGroundNormal = preset.gravityIsGroundNormal;
            kart.airGravityMode = preset.airGravityMode;
            kart.gravityCastLayers = preset.gravityCastLayers;
            kart.gravityCastSegments = preset.gravityCastSegments;
            kart.gravityCastRadius = preset.gravityCastRadius;
            kart.gravityCastDistance = preset.gravityCastDistance;
            kart.gravityCastsPerFrame = preset.gravityCastsPerFrame;
            kart.drawGravityCastGizmos = preset.drawGravityCastGizmos;
        }

        public void SaveGravityPreset(KartGravityPreset preset) {
            kart = GetComponent<Kart>();
            if (kart == null || preset == null) { return; }

            preset.gravityAdd = kart.gravityAdd;
            preset.gravityDir = kart.gravityDir;
            preset.gravityIsGroundNormal = kart.gravityIsGroundNormal;
            preset.airGravityMode = kart.airGravityMode;
            preset.gravityCastLayers = kart.gravityCastLayers;
            preset.gravityCastSegments = kart.gravityCastSegments;
            preset.gravityCastRadius = kart.gravityCastRadius;
            preset.gravityCastDistance = kart.gravityCastDistance;
            preset.gravityCastsPerFrame = kart.gravityCastsPerFrame;
            preset.drawGravityCastGizmos = kart.drawGravityCastGizmos;
            SaveAssets(preset);
        }

        public void LoadDriftPreset(KartDriftPreset preset) {
            kart = GetComponent<Kart>();
            if (kart == null || preset == null) { return; }

            kart.canDrift = preset.canDrift;
            kart.canDriftInAir = preset.canDriftInAir;
            kart.minDriftAngle = preset.minDriftAngle;
            kart.maxDriftAngle = preset.maxDriftAngle;
            kart.visualDriftFactor = preset.visualDriftFactor;
            kart.visualDriftAirFactor = preset.visualDriftAirFactor;
            kart.driftSwingDuration = preset.driftSwingDuration;
            kart.driftSwingForce = preset.driftSwingForce;
            kart.minDriftSpeed = preset.minDriftSpeed;
            kart.wallCollisionCancelsDrift = preset.wallCollisionCancelsDrift;
            kart.brakeCancelsDrift = preset.brakeCancelsDrift;
            kart.burnoutSpeed = preset.burnoutSpeed;
            kart.burnoutSpeedLimit = preset.burnoutSpeedLimit;
        }

        public void SaveDriftPreset(KartDriftPreset preset) {
            kart = GetComponent<Kart>();
            if (kart == null || preset == null) { return; }

            preset.canDrift = kart.canDrift;
            preset.canDriftInAir = kart.canDriftInAir;
            preset.minDriftAngle = kart.minDriftAngle;
            preset.maxDriftAngle = kart.maxDriftAngle;
            preset.visualDriftFactor = kart.visualDriftFactor;
            preset.visualDriftAirFactor = kart.visualDriftAirFactor;
            preset.driftSwingDuration = kart.driftSwingDuration;
            preset.driftSwingForce = kart.driftSwingForce;
            preset.minDriftSpeed = kart.minDriftSpeed;
            preset.wallCollisionCancelsDrift = kart.wallCollisionCancelsDrift;
            preset.brakeCancelsDrift = kart.brakeCancelsDrift;
            preset.burnoutSpeed = kart.burnoutSpeed;
            preset.burnoutSpeedLimit = kart.burnoutSpeedLimit;
            SaveAssets(preset);
        }

        public void LoadBoostPreset(KartBoostPreset preset) {
            kart = GetComponent<Kart>();
            if (kart == null || preset == null) { return; }

            kart.boostType = preset.boostType;
            kart.canBoost = preset.canBoost;
            kart.boostSpeedAdd = preset.boostSpeedAdd;
            kart.boostAccelAdd = preset.boostAccelAdd;
            kart.boostDrive = preset.boostDrive;
            kart.boostPower = preset.boostPower;
            kart.maxBoosts = preset.maxBoosts;
            kart.autoBoostInterval = preset.autoBoostInterval;
            kart.driftManualBoostLimit = preset.driftManualBoostLimit;
            kart.driftManualFailCancel = preset.driftManualFailCancel;
            kart.boostRate = preset.boostRate;
            kart.boostBurnRate = preset.boostBurnRate;
            kart.boostGroundPush = preset.boostGroundPush;
            kart.boostAirPush = preset.boostAirPush;
            kart.airLandBoost = preset.airLandBoost;
            kart.driftBoostAdd = preset.driftBoostAdd;
            kart.boostAmount = preset.boostAmount;
            kart.boostAmountLimit = preset.boostAmountLimit;
            kart.boostReserveLimit = preset.boostReserveLimit;
            kart.brakeCancelsBoost = preset.brakeCancelsBoost;
            kart.wallCollisionCancelsBoost = preset.wallCollisionCancelsBoost;
            kart.boostWheelie = preset.boostWheelie;
        }

        public void SaveBoostPreset(KartBoostPreset preset) {
            kart = GetComponent<Kart>();
            if (kart == null || preset == null) { return; }

            preset.boostType = kart.boostType;
            preset.canBoost = kart.canBoost;
            preset.boostSpeedAdd = kart.boostSpeedAdd;
            preset.boostAccelAdd = kart.boostAccelAdd;
            preset.boostDrive = kart.boostDrive;
            preset.boostPower = kart.boostPower;
            preset.maxBoosts = kart.maxBoosts;
            preset.autoBoostInterval = kart.autoBoostInterval;
            preset.driftManualBoostLimit = kart.driftManualBoostLimit;
            preset.driftManualFailCancel = kart.driftManualFailCancel;
            preset.boostRate = kart.boostRate;
            preset.boostBurnRate = kart.boostBurnRate;
            preset.boostGroundPush = kart.boostGroundPush;
            preset.boostAirPush = kart.boostAirPush;
            preset.airLandBoost = kart.airLandBoost;
            preset.driftBoostAdd = kart.driftBoostAdd;
            preset.boostAmount = kart.boostAmount;
            preset.boostAmountLimit = kart.boostAmountLimit;
            preset.boostReserveLimit = kart.boostReserveLimit;
            preset.brakeCancelsBoost = kart.brakeCancelsBoost;
            preset.wallCollisionCancelsBoost = kart.wallCollisionCancelsBoost;
            preset.boostWheelie = kart.boostWheelie;
            SaveAssets(preset);
        }

        public void LoadWallsPreset(KartWallsPreset preset) {
            kart = GetComponent<Kart>();
            if (kart == null || preset == null) { return; }

            kart.wallFriction = preset.wallFriction;
            kart.wallBounceTurnAmount = preset.wallBounceTurnAmount;
            kart.wallBounceTurnDecayRate = preset.wallBounceTurnDecayRate;
            kart.minWallHitSpeed = preset.minWallHitSpeed;
            kart.wallHitDuration = preset.wallHitDuration;
            kart.wallCollisionProps = preset.wallCollisionProps;
            kart.localUpWallDotComparison = preset.localUpWallDotComparison;
        }

        public void SaveWallsPreset(KartWallsPreset preset) {
            kart = GetComponent<Kart>();
            if (kart == null || preset == null) { return; }

            preset.wallFriction = kart.wallFriction;
            preset.wallBounceTurnAmount = kart.wallBounceTurnAmount;
            preset.wallBounceTurnDecayRate = kart.wallBounceTurnDecayRate;
            preset.minWallHitSpeed = kart.minWallHitSpeed;
            preset.wallHitDuration = kart.wallHitDuration;
            preset.wallCollisionProps = kart.wallCollisionProps;
            preset.localUpWallDotComparison = kart.localUpWallDotComparison;
            SaveAssets(preset);
        }

        void SaveAssets(Object modifiedAsset) {
#if UNITY_EDITOR
            if (Application.isEditor) {
                UnityEditor.EditorUtility.SetDirty(modifiedAsset);
                UnityEditor.AssetDatabase.SaveAssets();
            }
#endif
        }
    }
}