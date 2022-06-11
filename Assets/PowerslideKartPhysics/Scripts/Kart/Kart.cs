// Copyright (c) 2022 Justin Couch / JustInvoke
using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace PowerslideKartPhysics
{
    public enum KartBoostType { DriftAuto, DriftManual, Manual }

    // Main kart class controlling behavior and physics
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public class Kart : MonoBehaviour
    {
        public bool active = true;
        Transform tr;
        [System.NonSerialized]
        public Rigidbody rb;
        public Transform rotator;
        public Transform visualHolder;

        // Dimensions
        public float rotationRateFactor = 10f;
        [Range(0.0f, 1.0f)]
        public float minRotationRate = 0.1f;
        [Range(0.0f, 1.0f)]
        public float maxRotationRate = 0.3f;
        [Range(0.0f, 1.0f)]
        public float visualRotationRate = 0.1f;
        public float airFlattenRate = 0.01f;
        [System.NonSerialized]
        public Vector3 forwardDir;
        [System.NonSerialized]
        public Vector3 upDir;
        [System.NonSerialized]
        public Vector3 rightDir;
        public float frontLength = 1.0f;
        public float backLength = 1.0f;
        public float sideWidth = 1.0f;
        public Vector3 cornerCastSize = new Vector3(1.0f, 0.0f, 1.0f);
        public Vector3 cornerCastOffset = Vector3.zero;
        public bool oneCornerCastPerFrame = false;
        public float cornerCastDistance = 1.0f;
        int curCornerCast = 0;
        int lastGroundedCorner = 0;
        Vector3[] cornerCastPoints;
        public int maxCollisionContactPoints = 8;
        ContactPoint[] collisionContacts;
        public float spinRate = 10f;
        public float spinHeight = 1.0f;

        // Speed
        public float maxSpeed = 30f;
        public float maxReverseSpeed = 10f;
        public float acceleration = 1.0f;
        public float brakeForce = 1.5f;
        public float coastingFriction = 0.5f;
        public float slopeFriction = 0.5f;
        public float airDriveFriction = 0.0f;
        [System.NonSerialized]
        public Vector3 localVel = Vector3.zero;
        Vector3 localVelPrev = Vector3.zero;
        Vector3 localAccel = Vector3.zero;
        [System.NonSerialized]
        public float velMag;
        public float autoStopSpeed = 1.0f;
        public float autoStopForce = 1.0f;
        public float autoStopNormalDotLimit = 0.9f;
        float targetInput = 0.0f;
        public float maxFallSpeed = 30f;
        float maxGroundFriction = 1.0f;
        float maxGroundSpeed = 1.0f;
        public float spinDecel = 1.0f;

        // Steer
        [Range(0.0f, 1.0f)]
        public float steerRate = 1.0f;
        public float maxSteer = 1.0f;
        public float minSteer = 0.5f;
        public float airSteer = 0.5f;
        public float steerSpeedLimit = 30.0f;
        public float steerSlowLimit = 5.0f;
        float targetTurnSpeed = 0.0f;
        public float brakeSteerIncrease = 0.5f;
        public bool dontInvertSteerReverseAccel = true;
        float visualSteer = 0.0f;
        [Range(0.0f, 1.0f)]
        public float visualSteerRate = 0.1f;
        public float visualSteerSpeedLimit = 10f;
        public float turnTiltAmount = 0.0f;
        public float turnTiltReferenceSpeed = 20f;
        [Range(0.0f, 1.0f)]
        public float turnTiltRate = 1.0f;
        public float turnTiltSideOffsetFactor = 1.0f;
        public bool invertTurnTiltHeightOffset = false;
        public float localTiltOffsetCompensation = 0.0f;
        public float accelTiltAmount = 0.0f;
        float forwardTilt = 0.0f;
        float sideTilt = 0.0f;
        public float sidewaysFriction = 10f;
        public float airSidewaysFriction = 5.0f;
        public float brakeSlipAmount = 0.0f;

        public float GetVisualSteer() { return visualSteer; }

        [System.NonSerialized]
        public bool spinningOut = false;
        Vector3 spinForward = Vector3.forward;
        Vector3 spinUp = Vector3.up;
        Vector3 spinOffset = Vector3.zero;

        // Suspension
        public float springForce = 50f;
        public float springDampening = 0.1f;
        public float springDampVelMin = -1.0f;
        public float springDampVelMax = 1.0f;
        [Range(0.0f, 1.0f)]
        public float compressionSpringFactor = 0.5f;
        public float groundStickForce = 100f;
        [Range(0.0f, 1.0f)]
        public float groundStickCompression = 0.5f;

        public KartWheel[] wheels;
        public LayerMask wheelCastMask = 1;
        public int maxWheelCastHits = 2;
        RaycastHit[] groundHits;
        public bool oneWheelCastPerFrame = false;
        int curWheelCast = 0;
        int lastGroundedWheel = 0;
        Vector3[] stableWheelPoints;

        [System.NonSerialized]
        public bool grounded;
        bool wasGrounded;
        [System.NonSerialized]
        public bool airGrounded;
        float compression;
        Vector3 groundNormal = Vector3.up;
        Vector3 rawGroundNormal = Vector3.zero;
        public float groundNormalSmoothRate = 10f;
        Vector3 groundVel = Vector3.zero;
        Vector3 groundAngVel = Vector3.zero;

        // Jump
        public bool canJump = true;
        public float jumpForce = 100f;
        public float jumpDuration = 0.1f;
        public float jumpStickForce = 10f;
        float jumpTime = 0.0f;
        bool jumped;
        bool leftGroundJump;
        public float airJumpTimeLimit = 0.1f;
        float airTime = 0.0f;

        // Gravity
        public float gravityAdd = -10f;
        public Vector3 gravityDir = Vector3.up;
        [System.NonSerialized]
        public Vector3 currentGravityDir = Vector3.up;
        public bool gravityIsGroundNormal = false;
        public enum GravityMode { Initial, NearestSurface, LastSetDirection }
        public GravityMode airGravityMode = GravityMode.Initial;
        public int gravityCastLayers = 8;
        public int gravityCastSegments = 8;
        public float gravityCastRadius = 2.0f;
        public float gravityCastDistance = 1000f;
        public int gravityCastsPerFrame = 10;
        public bool drawGravityCastGizmos = false;
        bool isGravityCasting = false;

        public float GetJumpedAirTime() { return jumped ? airTime : 0.0f; }

        // Drift
        [System.NonSerialized]
        public bool drifting;
        [System.NonSerialized]
        public int driftDir = 0;
        bool driftReleased;
        public bool canDrift = true;
        public bool canDriftInAir = true;
        public float minDriftAngle = 0.5f;
        public float maxDriftAngle = 1.5f;
        public float visualDriftFactor = 0.5f;
        public float visualDriftAirFactor = 0.5f;
        float driftSwingTime = 0.0f;
        public float driftSwingDuration = 0.5f;
        public float driftSwingForce = 1.0f;
        public float minDriftSpeed = 5.0f;
        public bool wallCollisionCancelsDrift = true;
        public bool brakeCancelsDrift = false;
        [System.NonSerialized]
        public bool burnout;
        public float burnoutSpeed = 1.0f;
        public float burnoutSpeedLimit = 5.0f;

        Vector3 targetForward = Vector3.forward;
        Vector3 targetUp = Vector3.up;

        // Boost
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
        [System.NonSerialized]
        public int boostCount = 0;
        [System.NonSerialized]
        public float boostTime = 0.0f;
        bool boostFailed = false;
        public float boostAmount = 10f;
        public float boostAmountLimit = 10f;
        [System.NonSerialized]
        public float boostReserve = 0.0f;
        public float boostReserveLimit = Mathf.Infinity;
        bool boostPadUsed = false;
        float boostPadTimer = 0.0f;
        bool validBoost = true;
        public bool brakeCancelsBoost = true;
        public bool wallCollisionCancelsBoost = false;
        public float boostWheelie = 0.5f;

        // Walls
        public float wallFriction = 5.0f;
        float wallBounceTurn = 0.0f;
        public float wallBounceTurnAmount = 0.5f;
        public float wallBounceTurnDecayRate = 20f;
        public float minWallHitSpeed = 5.0f;
        public float wallHitDuration = 0.5f;
        float wallHitTime = 0.0f;
        WallCollision wallDetector;
        public WallDetectProps wallCollisionProps = WallDetectProps.Default;
        public bool localUpWallDotComparison = true;

        // Input
        float accelInput;
        float brakeInput;
        float steerInput;
        bool driftButton;
        bool driftButtonDown;
        bool boostButton;
        bool boostButtonDown;

        // Events
        public UnityEvent jumpEvent;
        public UnityEvent landEvent;
        float lastLandTime = 0.0f;
        public UnityEvent boostStartEvent;
        public UnityEvent boostFailEvent;
        public Events.DoubleVector3 collisionEvent;
        public UnityEvent spinOutEvent;

        private void Awake() {
            tr = transform;
            rb = GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotation; // Automatically constrain rotation
            groundHits = new RaycastHit[maxWheelCastHits];
            collisionContacts = new ContactPoint[maxCollisionContactPoints];
            cornerCastPoints = new Vector3[4];
            stableWheelPoints = new Vector3[wheels.Length]; // Stable raycast points not susceptible to tilting
            for (int i = 0; i < stableWheelPoints.Length; i++) {
                stableWheelPoints[i] = rotator.InverseTransformPoint(wheels[i].transform.position);
            }

            wallDetector = WallCollision.CreateFromType(wallCollisionProps.wallDetectionType); // Set up wall collision detection

            currentGravityDir = gravityDir.normalized;
            if (airGravityMode == GravityMode.NearestSurface && gravityCastsPerFrame > 0) {
                isGravityCasting = true;
                StartCoroutine(EvaluateNearestSurfacePoint()); // Start coroutine for finding nearest surface to gravitate towards
            }
        }

        private void FixedUpdate() {
            if (rotator == null || visualHolder == null || rotator == visualHolder || wheels.Length == 0) { return; }

            // Check for change in gravity mode to start gravity casts
            if (!isGravityCasting) {
                if (airGravityMode == GravityMode.NearestSurface && gravityCastsPerFrame > 0) {
                    isGravityCasting = true;
                    StartCoroutine(EvaluateNearestSurfacePoint()); // Start corouting for finding nearest surface to gravitate towards
                }
            }
            else if (airGravityMode != GravityMode.NearestSurface || gravityCastsPerFrame == 0) {
                isGravityCasting = false;
            }

            rb.AddForce(currentGravityDir * gravityAdd, ForceMode.Acceleration); // Add fake gravity
            velMag = rb.velocity.magnitude;
            localVelPrev = localVel;
            localVel = rotator.InverseTransformDirection(rb.velocity);
            localAccel = localVel - localVelPrev;
            forwardDir = rotator.forward;
            upDir = rotator.up;
            rightDir = rotator.right;
            targetInput = Mathf.Clamp01(accelInput + boostReserve * boostDrive) - brakeInput; // Get accel and brake input

            // Calculating driving speed
            float targetSpeed = ((targetInput >= 0 ? maxSpeed : -maxReverseSpeed) * Mathf.Abs(targetInput) + Mathf.Clamp01(boostReserve) * boostSpeedAdd) * maxGroundSpeed;

            // Brake canceling
            if (brakeInput > 0.1f) {
                if (brakeCancelsBoost) {
                    EmptyBoostReserve();
                }

                if (brakeCancelsDrift) {
                    CancelDrift();
                }
            }

            // Timer decrementing
            jumpTime = Mathf.Max(0.0f, jumpTime - Time.fixedDeltaTime);
            driftSwingTime = Mathf.Max(0.0f, driftSwingTime - Time.fixedDeltaTime);
            wallBounceTurn = Mathf.Lerp(wallBounceTurn, 0.0f, wallBounceTurnDecayRate * Time.fixedDeltaTime);
            wallHitTime = Mathf.Max(0.0f, wallHitTime - Time.fixedDeltaTime);
            boostPadTimer = Mathf.Max(0.0f, boostPadTimer - Time.fixedDeltaTime);

            bool wasGrounded = grounded;
            GroundCheck(); // Check to see if on ground

            if (drifting && !grounded && !canDriftInAir) {
                CancelDrift();
            }

            // Rotating the kart
            if (grounded || airGrounded) {
                rotator.rotation = Quaternion.Lerp(rotator.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(forwardDir, groundNormal).normalized, groundNormal),
                    Mathf.Clamp(Vector3.Dot(rb.velocity.normalized, groundNormal) * velMag * rotationRateFactor, minRotationRate, maxRotationRate));

                rotator.Rotate(groundAngVel * Mathf.Rad2Deg * Time.fixedDeltaTime, Space.World);
                rotator.localPosition = Vector3.zero;
            }

            // Burnout logic
            if (!burnout && grounded && Mathf.Abs(localVel.z) < burnoutSpeedLimit - 0.1f && accelInput > 0.9f && brakeInput > 0.9f) {
                burnout = true;
            }
            else if (burnout && !(grounded && Mathf.Abs(localVel.z) < burnoutSpeedLimit + 0.1f && accelInput > 0.9f && brakeInput > 0.9f)) {
                burnout = false;
            }

            // Calculating turning rate
            float targetTurn = 0.0f;
            if (burnout) {
                targetSpeed = burnoutSpeed;
                targetTurn = steerInput;
            }
            else if (drifting && driftDir != 0) {
                targetTurn = Mathf.Lerp(maxSteer, minSteer, Mathf.Abs(localVel.z) / Mathf.Max(steerSpeedLimit, 0.01f))
                    * Mathf.Lerp(minDriftAngle, maxDriftAngle, Mathf.Abs((steerInput + driftDir) * 0.5f)) * driftDir
                    * (Mathf.Sign(targetInput) != Mathf.Sign(localVel.z) && Mathf.Abs(targetInput) > 0.001f ? 1.0f + brakeSteerIncrease : 1.0f);
            }
            else {
                targetTurn = Mathf.Lerp(maxSteer, minSteer, Mathf.Abs(localVel.z) / Mathf.Max(steerSpeedLimit, 0.01f)) * steerInput
                    * (grounded ? (dontInvertSteerReverseAccel && accelInput > 0 ? 1.0f : Mathf.Sign(localVel.z))
                        * (steerSlowLimit > 0 ? Mathf.Clamp01(Mathf.Abs(localVel.z) / Mathf.Max(steerSlowLimit, 0.01f)) : 1.0f) : 1.0f)
                    * (grounded || (jumped && airGrounded && !leftGroundJump) ? 1.0f : airSteer)
                    * (Mathf.Sign(targetInput) != Mathf.Sign(localVel.z) && Mathf.Abs(targetInput) > 0.001f ? 1.0f + brakeSteerIncrease : 1.0f);
            }

            // Final turn rate
            targetTurnSpeed = Mathf.Lerp(targetTurnSpeed, targetTurn, steerRate * 100f * Time.fixedDeltaTime);

            // Visual turn rate
            float targetVisualSteer = drifting && driftDir != 0 ? -steerInput - driftDir : steerInput * Mathf.Clamp01(visualSteerSpeedLimit / Mathf.Max(Mathf.Abs(localVel.z), 0.001f));
            visualSteer = Mathf.Lerp(visualSteer, targetVisualSteer, visualSteerRate * 100f * Time.fixedDeltaTime);

            // Side friction application
            float brakeSlipFactor = Mathf.Clamp(1.0f - (localVel.z > 0 ? brakeInput : accelInput) * Mathf.Clamp01(velMag * 0.1f) * brakeSlipAmount, 0.0f, 0.9f);
            if (grounded || (jumped && airGrounded && !leftGroundJump)) {
                rb.AddForce(rightDir * (-localVel.x * sidewaysFriction * (grounded ? maxGroundFriction : 1.0f) * brakeSlipFactor - Mathf.Clamp01(driftSwingTime) * driftDir * 100f * driftSwingForce), ForceMode.Acceleration);
            }
            else {
                rb.AddForce(rightDir * (-localVel.x * airSidewaysFriction * (1.0f - Mathf.Abs(Vector3.Dot(rightDir, currentGravityDir))) * brakeSlipFactor - Mathf.Clamp01(driftSwingTime) * driftDir * 100f * driftSwingForce), ForceMode.Acceleration);
            }

            // Grounded state
            if (grounded) {
                // Local travel velocity for suspension
                float travelVel = Mathf.Clamp(localVel.y, springDampVelMin, springDampVelMax * (jumpTime == 0 ? 1.0f : 0.0f));

                // Suspension force
                rb.AddForce(
                    groundNormal * springForce * ((1.0f - compression * Mathf.Clamp01(compressionSpringFactor)) - springDampening * travelVel)
                    , ForceMode.Acceleration);

                // Landing after jumping
                if (jumpTime == 0) {
                    // Jump landing boost
                    if (canBoost && jumped && leftGroundJump && airLandBoost > 0 && !spinningOut) {
                        AddBoost(boostPower * Mathf.Min(airLandBoost, airTime), Mathf.Clamp01(boostReserve) * airLandBoost);
                        boostStartEvent.Invoke();
                    }

                    jumped = false;

                    // Ground stick force
                    rb.AddForce(
                        -groundNormal * groundStickForce * Mathf.Clamp01(compression - groundStickCompression) * Mathf.Clamp01(Vector3.Dot(groundNormal, currentGravityDir))
                        , ForceMode.Acceleration);
                }

                airTime = 0.0f;

                if (!spinningOut) {
                    // Acceleration and braking force
                    rb.AddForce(forwardDir * (targetSpeed - localVel.z) * (Mathf.Abs(targetSpeed) > Mathf.Abs(localVel.z) && System.Math.Sign(targetSpeed) == System.Math.Sign(localVel.z) ? acceleration + Mathf.Clamp01(boostReserve) * boostAccelAdd : 1.0f) * maxGroundFriction * (Mathf.Sign(targetInput) != Mathf.Sign(localVel.z) ? brakeForce : 1.0f)
                        * (targetInput == 0 && !(Mathf.Abs(localVel.z) > Mathf.Abs(targetSpeed) && System.Math.Sign(targetSpeed) == System.Math.Sign(localVel.z)) ? coastingFriction : 1.0f)
                        * Mathf.Clamp01(1.0f + slopeFriction - Vector3.Dot(forwardDir * Mathf.Sign(targetSpeed), currentGravityDir)), ForceMode.Acceleration);

                    // Staying parked at low speed
                    if (Mathf.Abs(targetInput) < 0.001f && velMag < autoStopSpeed && Vector3.Dot(groundNormal, currentGravityDir) > autoStopNormalDotLimit) {
                        rb.AddForce(-rb.velocity * autoStopForce, ForceMode.Acceleration);
                        rb.AddForce(-Vector3.ProjectOnPlane((rb.useGravity ? Physics.gravity : Vector3.zero) + currentGravityDir * gravityAdd, groundNormal), ForceMode.Acceleration); // Canceling out sliding on slopes due to gravity
                    }
                }

                // Land event invocation
                if (!wasGrounded && Time.timeSinceLevelLoad - lastLandTime >= 0.2f && localVel.y < -1.0f) {
                    lastLandTime = Time.timeSinceLevelLoad;
                    landEvent.Invoke();
                }
            }
            else if (!airGrounded) // Air grounded state indicates wheels are off ground, but kart is still close to ground (example: jumping but not off a ramp)
            {
                // If completely in air (not air grounded) then rotate kart upright
                rotator.rotation = Quaternion.Lerp(rotator.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(forwardDir, currentGravityDir).normalized, currentGravityDir), airFlattenRate * 100f * Time.fixedDeltaTime);
                rotator.localPosition = Vector3.zero;
                leftGroundJump = true;
            }
            else if (jumped && jumpTime == 0) {
                // If not grounded but still air grounded, apply ground stick force if not jumping
                rb.AddForce(
                    -groundNormal * jumpStickForce * Mathf.Clamp01(Vector3.Dot(groundNormal, currentGravityDir))
                    , ForceMode.Acceleration);
            }

            if (!grounded) {
                airTime += Time.fixedDeltaTime;
                if (airDriveFriction > 0) {
                    // Air acceleration and braking force
                    rb.AddForce(forwardDir * airDriveFriction * (targetSpeed - localVel.z) * (Mathf.Abs(targetSpeed) > Mathf.Abs(localVel.z) && System.Math.Sign(targetSpeed) == System.Math.Sign(localVel.z) ? acceleration + Mathf.Clamp01(boostReserve) * boostAccelAdd : 1.0f) * (Mathf.Sign(targetInput) != Mathf.Sign(localVel.z) ? brakeForce : 1.0f)
                        * (targetInput == 0 && !(Mathf.Abs(localVel.z) > Mathf.Abs(targetSpeed) && System.Math.Sign(targetSpeed) == System.Math.Sign(localVel.z)) ? coastingFriction : 1.0f)
                        * Mathf.Clamp01(1.0f + slopeFriction - Vector3.Dot(forwardDir * Mathf.Sign(targetSpeed), currentGravityDir)), ForceMode.Acceleration);
                }
            }

            // Boost reserve decrementing
            if (boostType == KartBoostType.DriftAuto || boostType == KartBoostType.DriftManual) {
                boostReserve = Mathf.Max(0.0f, boostReserve - boostBurnRate * Time.fixedDeltaTime);
            }

            // Boost force
            rb.AddForce(forwardDir * Mathf.Clamp01(boostReserve) * (grounded ? boostGroundPush : boostAirPush), ForceMode.Acceleration);

            // Drift auto boost logic
            if (boostType == KartBoostType.DriftAuto) {
                // Drift state
                if (drifting && driftDir != 0 && !boostFailed) {
                    boostTime += boostRate * Time.fixedDeltaTime;
                    boostCount = Mathf.Min(Mathf.FloorToInt(boostTime / Mathf.Max(0.0001f, autoBoostInterval)), maxBoosts);
                }

                // Drift end
                if (!drifting || driftDir == 0) {
                    if (boostTime > 0) {
                        EndDriftBoost();
                    }
                    else {
                        CancelDriftBoost(false);
                    }
                }
            }
            else if (boostType == KartBoostType.DriftManual) // Drift manual boost logic
            {
                // Drift state
                if (drifting && driftDir != 0 && !boostFailed) {
                    boostTime = Mathf.Clamp01(boostTime + boostRate * Time.fixedDeltaTime);
                }

                // Drift end
                if (!drifting || driftDir == 0) {
                    CancelDriftBoost(false);
                }

                // Premature boost
                if (boostTime >= 1 || boostCount > maxBoosts - 1) {
                    if (boostTime >= 1) {
                        boostFailEvent.Invoke();
                    }

                    CancelDriftBoost(true);
                }
            }
            else if (boostType == KartBoostType.Manual) // Manual boost logic
            {
                // Drift state
                float addedDriftBoost = 0.0f;
                if (drifting && driftDir != 0) {
                    AddBoost(driftBoostAdd * Time.fixedDeltaTime);
                    addedDriftBoost = driftBoostAdd * Time.fixedDeltaTime * 2.0f;
                }

                // Boost button on state
                if (boostButton && boostAmount > addedDriftBoost && validBoost && !spinningOut) {
                    boostAmount = Mathf.Max(0.0f, boostAmount - boostBurnRate * Time.fixedDeltaTime);
                    boostReserve = Mathf.Min(boostReserve + boostRate * Time.fixedDeltaTime, boostReserveLimit);
                }
                else if (!boostButton || boostAmount <= addedDriftBoost || !validBoost) // Boost button off state
                {
                    boostReserve = Mathf.Max(0.0f, boostReserve - boostBurnRate * Time.fixedDeltaTime);
                    validBoost = false;
                }
            }

            // Drift visual rotation logic
            if (drifting && driftDir != 0) {
                float angle = Mathf.Lerp(minDriftAngle, maxDriftAngle, Mathf.Abs((steerInput + driftDir) * 0.5f)) * driftDir * visualDriftFactor;
                targetForward = new Vector3(Mathf.Sin(angle), 0.0f, Mathf.Cos(angle));
            }
            else if (!grounded && canDrift && driftButton && localVel.z >= minDriftSpeed) {
                float angle = minDriftAngle * steerInput * visualDriftAirFactor;
                targetForward = new Vector3(Mathf.Sin(angle), 0.0f, Mathf.Cos(angle));
            }
            else {
                targetForward = Vector3.forward;
            }

            // Local visual pitch tilting
            forwardTilt = Mathf.Clamp01(boostReserve - 1.0f) * boostWheelie
                + Mathf.Clamp(localAccel.z, -1.0f, 1.0f) * accelTiltAmount;

            // Local visual roll tilting
            sideTilt = Mathf.Lerp(
                    sideTilt, Mathf.Clamp(targetTurnSpeed, -1.0f, 1.0f) * turnTiltAmount * Mathf.Clamp01(velMag / Mathf.Max(0.01f, turnTiltReferenceSpeed)) * Mathf.Sign(localVel.z)
                    + Mathf.Clamp(localAccel.x, -1.0f, 1.0f) * accelTiltAmount,
                turnTiltRate * 100f * Time.fixedDeltaTime);

            // Local visual rotation
            targetUp = new Vector3(sideTilt * (1.0f - Mathf.Abs(targetForward.x)), 1.0f - Mathf.Abs(sideTilt), sideTilt * (1.0f - Mathf.Abs(targetForward.z)));
            targetForward = new Vector3(targetForward.x, targetForward.y + forwardTilt, targetForward.z);

            if (spinningOut) {
                // Visual rotation while spinning out
                visualHolder.localRotation = Quaternion.Lerp(visualHolder.localRotation, Quaternion.LookRotation(spinForward, spinUp), 20f * Time.fixedDeltaTime);
                visualHolder.localPosition = Vector3.Lerp(visualHolder.localPosition, Vector3.zero + spinOffset, 20f * Time.fixedDeltaTime);
                rb.AddForce(new Vector3(-rb.velocity.x, 0.0f, -rb.velocity.z) * spinDecel, ForceMode.Acceleration); // Slow down while spinning out
            }
            else {
                // Visual rotation while driving
                visualHolder.localRotation = Quaternion.Lerp(visualHolder.localRotation, Quaternion.LookRotation(targetForward, Vector3.up) * Quaternion.AngleAxis(-sideTilt, Vector3.forward), visualRotationRate * 100f * Time.fixedDeltaTime);

                // Calculations for local offset based on rotation
                Vector3 localVisForward = visualHolder.localRotation * Vector3.forward;
                localVisForward = new Vector3(0.0f, localVisForward.y, localVisForward.z).normalized;
                Vector3 localVisRight = visualHolder.localRotation * Vector3.right;
                float forwardAngle = Mathf.Atan2(localVisForward.y, localVisForward.z);
                float sideAngle = Mathf.Atan2(localVisRight.y, localVisRight.x);
                float forwardHeightOffset = Mathf.Tan(forwardAngle) * (forwardAngle > 0 ? backLength : frontLength);
                float sideHeightOffset = Mathf.Tan(sideAngle) * sideWidth * 0.5f;

                // Visual offset
                visualHolder.localPosition = Vector3.up * (Mathf.Abs(forwardHeightOffset) + Mathf.Abs(sideHeightOffset) * (invertTurnTiltHeightOffset ? -1.0f : 1.0f))
                    - Vector3.right * sideHeightOffset * 2.0f * turnTiltSideOffsetFactor * Mathf.Abs(targetForward.z)
                    - Vector3.forward * forwardHeightOffset;

                visualHolder.position -= visualHolder.up * Mathf.Abs(sideHeightOffset) * localTiltOffsetCompensation;

                // Rotate kart based on steering and wall collision
                rotator.Rotate(Vector3.up, (targetTurnSpeed + wallBounceTurn) * 100f * Time.fixedDeltaTime, Space.Self);
            }

            // Fall speed limiting
            if (!grounded && localVel.y < -maxFallSpeed) {
                rb.AddForce(currentGravityDir * -(maxFallSpeed + localVel.y), ForceMode.Acceleration);
            }

            // Jump force
            if (jumpTime > 0) {
                rb.AddForce(upDir * jumpForce * jumpTime, ForceMode.Acceleration);
            }
        }

        private void Update() {
            // Jump starting
            if (canJump && driftButtonDown && !jumped && !spinningOut && (grounded || airGrounded || airTime <= airJumpTimeLimit)) {
                jumped = true;
                jumpTime = jumpDuration;
                leftGroundJump = false;
                boostPadUsed = false;
                jumpEvent.Invoke();
            }

            // Drift starting
            if (canDrift && (grounded || canDriftInAir) && driftButton && driftReleased && !spinningOut) {
                drifting = true;

                if (grounded && jumpTime == 0 && driftDir == 0) {
                    if (System.Math.Sign(steerInput) != 0) {
                        driftDir = (int)Mathf.Sign(steerInput);
                        driftSwingTime = driftSwingDuration;
                    }
                    driftReleased = false;
                }
            }

            // Drift auto boost input
            if (boostType == KartBoostType.DriftAuto) {
                // Ending drift
                if (!driftButton) {
                    driftReleased = true;
                    EndDriftBoost();
                }
                else if (burnout || localVel.z < minDriftSpeed) {
                    CancelDrift();
                }
            }
            else if (boostType == KartBoostType.DriftManual) // Drift manual boost input
            {
                // Ending drift
                if (!driftButton || burnout || localVel.z < minDriftSpeed) {
                    CancelDrift();
                }

                if (!driftButton) {
                    driftReleased = true;
                }

                if (canBoost && boostButtonDown) {
                    // Boost success
                    if (boostTime >= driftManualBoostLimit) {
                        boostCount++;
                        AddBoost(boostTime * boostCount * boostPower, boostCount);
                        boostTime = 0.0f;
                        boostStartEvent.Invoke();
                    }
                    else // Boost fail
                    {
                        if (drifting && driftDir != 0 && boostTime > 0) {
                            boostFailEvent.Invoke();
                        }

                        if (driftManualFailCancel) {
                            CancelDriftBoost(true);
                        }
                        else {
                            boostCount++;
                            boostTime = 0.0f;
                        }
                    }
                }
            }
            else if (boostType == KartBoostType.Manual) // Manual boost input
            {
                // Ending drift
                if (!driftButton || burnout || localVel.z < minDriftSpeed) {
                    CancelDrift();
                }

                if (!driftButton) {
                    driftReleased = true;
                }

                // Boost starting
                if (canBoost && boostButtonDown && boostAmount > 0 && !spinningOut) {
                    validBoost = true;
                    boostStartEvent.Invoke();
                }
            }
        }

        // Checking to see if kart is on ground
        void GroundCheck() {
            rawGroundNormal = Vector3.zero;
            compression = 0.0f;
            maxGroundFriction = 1.0f;
            maxGroundSpeed = 1.0f;
            cornerCastPoints[0] = rotator.TransformPoint(cornerCastOffset + new Vector3(cornerCastSize.x * 0.5f, cornerCastSize.y, cornerCastSize.z * 0.5f));
            cornerCastPoints[1] = rotator.TransformPoint(cornerCastOffset + new Vector3(cornerCastSize.x * -0.5f, cornerCastSize.y, cornerCastSize.z * 0.5f));
            cornerCastPoints[2] = rotator.TransformPoint(cornerCastOffset + new Vector3(cornerCastSize.x * -0.5f, cornerCastSize.y, cornerCastSize.z * -0.5f));
            cornerCastPoints[3] = rotator.TransformPoint(cornerCastOffset + new Vector3(cornerCastSize.x * 0.5f, cornerCastSize.y, cornerCastSize.z * -0.5f));

            // Logic for asynchronous wheel raycasts and maintaining previous ground hit info
            if (!oneWheelCastPerFrame || (oneWheelCastPerFrame && curWheelCast == lastGroundedWheel)) {
                grounded = false;
            }

            if (!oneCornerCastPerFrame || (oneCornerCastPerFrame && curCornerCast == lastGroundedCorner)) {
                airGrounded = false;
            }

            RaycastHit hit = new RaycastHit();

            // Asynchronous wheel raycasting (visual)
            if (oneWheelCastPerFrame) {
                ClearGroundHits();
                KartWheel curWheel = wheels[curWheelCast];
                bool wheelHit = Physics.RaycastNonAlloc(curWheel.transform.position, -curWheel.transform.up, groundHits, curWheel.suspensionDistance, wheelCastMask, QueryTriggerInteraction.Ignore) > 0;

                if (wheelHit) {
                    wheelHit = EvaluateGroundHits(groundHits, out hit);
                }

                // Setting ground hit info for visual wheel
                if (wheelHit) {
                    curWheel.grounded = true;
                    curWheel.localVel = curWheel.transform.InverseTransformDirection(rb.velocity);
                    curWheel.contactPoint = hit.point;
                    curWheel.contactNormal = hit.normal;
                    curWheel.contactTr = hit.transform;
                    curWheel.contactDistance = hit.distance;

                    if (hit.rigidbody != null) {
                        curWheel.localVel -= curWheel.transform.InverseTransformDirection(hit.rigidbody.GetPointVelocity(curWheel.transform.position));
                    }

                    GroundSurface surface = hit.collider.GetComponent<GroundSurface>();

                    if (surface != null) {
                        curWheel.SetSurface(surface);
                    }
                    else {
                        curWheel.SetSurface(null);
                    }
                }
                else {
                    curWheel.grounded = false;
                    curWheel.SetSurface(null);
                }
            }
            else // Simultaneous wheel raycasting (visual)
            {
                for (int i = 0; i < wheels.Length; i++) {
                    ClearGroundHits();
                    KartWheel curWheel = wheels[i];
                    bool wheelHit = Physics.RaycastNonAlloc(curWheel.transform.position, -curWheel.transform.up, groundHits, curWheel.suspensionDistance, wheelCastMask, QueryTriggerInteraction.Ignore) > 0;

                    if (wheelHit) {
                        wheelHit = EvaluateGroundHits(groundHits, out hit);
                    }

                    // Setting ground hit info for visual wheel
                    if (wheelHit) {
                        curWheel.grounded = true;
                        curWheel.localVel = curWheel.transform.InverseTransformDirection(rb.velocity);
                        curWheel.contactPoint = hit.point;
                        curWheel.contactNormal = hit.normal;
                        curWheel.contactTr = hit.transform;
                        curWheel.contactDistance = hit.distance;

                        if (hit.rigidbody != null) {
                            curWheel.localVel -= curWheel.transform.InverseTransformDirection(hit.rigidbody.GetPointVelocity(curWheel.transform.position));
                        }

                        GroundSurface surface = hit.collider.GetComponent<GroundSurface>();

                        if (surface != null) {
                            curWheel.SetSurface(surface);
                        }
                        else {
                            curWheel.SetSurface(null);
                        }
                    }
                    else {
                        curWheel.grounded = false;
                        curWheel.SetSurface(null);
                    }
                }
            }

            groundVel = Vector3.zero;
            groundAngVel = Vector3.zero;
            int groundedWheels = 0;

            // Asynchronous wheel raycasting (physics/stable points)
            if (oneWheelCastPerFrame) {
                ClearGroundHits();
                KartWheel curWheel = wheels[curWheelCast];
                bool wheelHit = Physics.RaycastNonAlloc(rotator.TransformPoint(stableWheelPoints[curWheelCast]), -upDir, groundHits, curWheel.suspensionDistance, wheelCastMask, QueryTriggerInteraction.Ignore) > 0;

                if (wheelHit) {
                    wheelHit = EvaluateGroundHits(groundHits, out hit);
                }

                // Physical ground hit info
                if (wheelHit) {
                    grounded = true;
                    lastGroundedWheel = curWheelCast;
                    groundedWheels++;
                    maxGroundFriction = curWheel.surfaceFriction;
                    maxGroundSpeed = curWheel.surfaceSpeed;
                    if (hit.rigidbody != null) {
                        groundVel = hit.rigidbody.GetPointVelocity(rotator.position);
                        groundAngVel = hit.rigidbody.angularVelocity;
                    }

                    compression += Mathf.Clamp01(hit.distance / Mathf.Max(curWheel.suspensionDistance, 0.01f));
                    rawGroundNormal += hit.normal;
                }
                curWheelCast = (curWheelCast + 1) % wheels.Length;
            }
            else // Simultaneous wheel raycasting (physics/stable points)
            {
                for (int i = 0; i < wheels.Length; i++) {
                    ClearGroundHits();
                    KartWheel curWheel = wheels[i];
                    bool wheelHit = Physics.RaycastNonAlloc(rotator.TransformPoint(stableWheelPoints[i]), -upDir, groundHits, curWheel.suspensionDistance, wheelCastMask, QueryTriggerInteraction.Ignore) > 0;

                    if (wheelHit) {
                        wheelHit = EvaluateGroundHits(groundHits, out hit);
                    }

                    // Physical ground hit info
                    if (wheelHit) {
                        grounded = true;
                        groundedWheels++;
                        if (i == 0) {
                            maxGroundFriction = 0.0f;
                            maxGroundSpeed = 0.0f;
                        }
                        maxGroundFriction = Mathf.Max(maxGroundFriction, curWheel.surfaceFriction);
                        maxGroundSpeed = Mathf.Max(maxGroundSpeed, curWheel.surfaceSpeed);
                        if (hit.rigidbody != null) {
                            groundVel = hit.rigidbody.GetPointVelocity(rotator.position);
                            groundAngVel = hit.rigidbody.angularVelocity;
                        }

                        compression += Mathf.Clamp01(hit.distance / Mathf.Max(curWheel.suspensionDistance, 0.01f));
                        rawGroundNormal += hit.normal;
                    }
                }
            }

            localVel -= rotator.InverseTransformDirection(groundVel); // Applying velocity of ground (if driving on rigidbody)

            if (groundedWheels > 1) {
                compression = Mathf.Clamp01(compression / groundedWheels);
            }

            // If not grounded, check for air grounded state with corner raycasts
            if (!grounded) {

                // Asynchronous corner raycasting
                if (oneCornerCastPerFrame) {
                    ClearGroundHits();
                    bool cornerHit = Physics.RaycastNonAlloc(cornerCastPoints[curCornerCast], -upDir, groundHits, cornerCastDistance, wheelCastMask, QueryTriggerInteraction.Ignore) > 0;

                    if (cornerHit) {
                        cornerHit = EvaluateGroundHits(groundHits, out hit);
                    }

                    // Corner hit info
                    if (cornerHit) {
                        airGrounded = true;
                        lastGroundedCorner = curCornerCast;
                        rawGroundNormal += hit.normal;
                    }
                    curCornerCast = (curCornerCast + 1) % cornerCastPoints.Length;
                }
                else // Simultaneous corner raycasting
                {
                    for (int i = 0; i < cornerCastPoints.Length; i++) {
                        ClearGroundHits();
                        bool cornerHit = Physics.RaycastNonAlloc(cornerCastPoints[i], -upDir, groundHits, cornerCastDistance, wheelCastMask, QueryTriggerInteraction.Ignore) > 0;

                        if (cornerHit) {
                            cornerHit = EvaluateGroundHits(groundHits, out hit);
                        }

                        // Corner hit info
                        if (cornerHit) {
                            airGrounded = true;
                            rawGroundNormal += hit.normal;
                        }
                    }
                }
            }

            rawGroundNormal.Normalize();
            if (grounded || airGrounded) {
                // Smoothing the ground normal
                groundNormal = Vector3.Slerp(groundNormal, rawGroundNormal, groundNormalSmoothRate * Time.fixedDeltaTime);

                // Updating gravity while grounded
                if (gravityIsGroundNormal) {
                    currentGravityDir = groundNormal;
                }
                else {
                    currentGravityDir = gravityDir.normalized;
                }
            }
            else {
                // Updating gravity direction while airborne
                groundNormal = upDir;
                switch (airGravityMode) {
                    case GravityMode.Initial:
                        currentGravityDir = gravityDir.normalized;
                        break;
                    case GravityMode.NearestSurface:
                        currentGravityDir = (rotator.position - nearestSurfacePoint).normalized;
                        break;
                    case GravityMode.LastSetDirection:
                        break;
                }
            }
        }

        // Determines if a raycast hit is valid, basically excluding colliders that are children of the kart
        bool EvaluateGroundHits(RaycastHit[] hits, out RaycastHit hit) {
            hit = new RaycastHit();
            if (hits == null) { return false; }

            bool hitted = false;
            float minDist = Mathf.Infinity;
            for (int i = 0; i < hits.Length; i++) {
                if (hits[i].collider != null && !hits[i].collider.transform.IsChildOf(tr) && (hits[i].distance < minDist || !hitted)) {
                    hit = hits[i];
                    minDist = hits[i].distance;
                    hitted = true;
                }
            }
            return hitted;
        }

        // Clears the ground hits so no old hits are used for different wheels
        void ClearGroundHits() {
            for (int i = 0; i < groundHits.Length; i++) {
                groundHits[i] = new RaycastHit();
            }
        }

        Vector3 nearestSurfacePoint = Vector3.zero;
        int curGravityCasts = 0;

        // Repeating coroutine to find nearest surface point while airborne, stored in nearestSurfacePoint
        IEnumerator EvaluateNearestSurfacePoint() {
            if (rotator == null || !isGravityCasting) { yield break; }

            // Only check for nearest surface when airborne
            while (grounded || airGrounded) {
                yield return null;
            }

            Vector3 tempNearPoint = rotator.position;
            float closeDist = Mathf.Infinity;
            float segAngle = Mathf.PI * 2.0f / gravityCastSegments; // Angle between segments
            float curSegAngle = 0.0f;
            float layerAngle = Mathf.PI / gravityCastLayers; // Angle between layers
            float curLayerAngle = -Mathf.PI * 0.5f + layerAngle;

            // Cast point directly below kart
            RaycastHit hit = new RaycastHit();
            if (Physics.SphereCast(rotator.position, gravityCastRadius, -rotator.up, out hit, gravityCastDistance, wheelCastMask, QueryTriggerInteraction.Ignore)) {
                if (hit.distance < closeDist) {
                    closeDist = hit.distance;
                    tempNearPoint = hit.point;
                }
            }

            // Loop through all layers and segments to cast points around the kart
            for (int i = 0; i < gravityCastLayers - 1; i++) {
                for (float j = 0; j < gravityCastSegments; j++) {
                    Vector3 castDir = rotator.TransformDirection(Mathf.Sin(curSegAngle) * Mathf.Cos(curLayerAngle), Mathf.Sin(curLayerAngle), Mathf.Cos(curSegAngle) * Mathf.Cos(curLayerAngle));
                    if (Physics.SphereCast(rotator.position, gravityCastRadius, castDir, out hit, gravityCastDistance, wheelCastMask, QueryTriggerInteraction.Ignore)) {
                        if (hit.distance < closeDist) {
                            closeDist = hit.distance;
                            tempNearPoint = hit.point;
                        }
                    }
                    curSegAngle += segAngle;

                    // Count number of casts this frame and wait until next frame if limit has been reached
                    curGravityCasts++;
                    if (curGravityCasts >= gravityCastsPerFrame) {
                        curGravityCasts = 0;
                        yield return new WaitForFixedUpdate();
                    }
                }
                curLayerAngle += layerAngle;
            }

            // Cast point directly above kart
            if (Physics.SphereCast(rotator.position, gravityCastRadius, rotator.up, out hit, gravityCastDistance, wheelCastMask, QueryTriggerInteraction.Ignore)) {
                if (hit.distance < closeDist) {
                    tempNearPoint = hit.point;
                }
            }

            nearestSurfacePoint = tempNearPoint;
            StartCoroutine(EvaluateNearestSurfacePoint()); // Restart gravity cast check
        }

        // End drift state
        void CancelDrift() {
            drifting = false;
            driftDir = 0;
            driftSwingTime = 0.0f;
        }

        // End drift state and award boost for the drift auto boost type
        void EndDriftBoost() {
            CancelDrift();
            if (canBoost) {
                AddBoost(boostCount * boostPower, boostCount);
                if (boostCount > 0) {
                    boostStartEvent.Invoke();
                }
            }
            CancelDriftBoost(false);
        }

        // Award boost to kart
        public void AddBoost(float boostToAdd) {
            if (canBoost) {
                if (boostType == KartBoostType.DriftAuto || boostType == KartBoostType.DriftManual) {
                    boostReserve = Mathf.Min(boostReserve + boostToAdd, boostReserveLimit);
                }
                else if (boostType == KartBoostType.Manual) {
                    boostAmount = Mathf.Min(boostAmount + boostToAdd, boostAmountLimit);
                }
            }
        }

        // Award boost to kart and push forward with force
        public void AddBoost(float boostToAdd, float pushForce) {
            if (canBoost) {
                AddBoost(boostToAdd);
                rb.AddForce(forwardDir * pushForce, ForceMode.VelocityChange);
            }
        }

        // Ending drift auto boost
        void CancelDriftBoost(bool failed) {
            boostTime = 0.0f;
            boostFailed = failed;
            boostCount = 0;
        }

        // Remove current boost amount being used
        void EmptyBoostReserve() {
            boostReserve = 0.0f;
        }

        // Return current boost value based on the boost type
        public float GetBoostValue() {
            switch (boostType) {
                case KartBoostType.DriftAuto:
                    return boostCount < maxBoosts ? Mathf.Repeat(boostTime, autoBoostInterval) / Mathf.Max(0.01f, autoBoostInterval) : 1.0f;
                case KartBoostType.DriftManual:
                    return boostTime;
                case KartBoostType.Manual:
                    return boostAmount / Mathf.Max(0.01f, boostAmountLimit);
            }
            return 0.0f;
        }

        // Return whether the boost is ready based on the boost type
        public bool IsBoostReady() {
            return (boostType == KartBoostType.DriftAuto && boostCount > 0)
                || (boostType == KartBoostType.DriftManual && boostTime >= driftManualBoostLimit)
                || (boostType == KartBoostType.Manual && boostAmount > 0);
        }

        public enum SpinAxis { Yaw, Pitch, Roll }

        // Start spinning out around the given axis for the number of rotations
        public void SpinOut(SpinAxis spinType, int spinCount) {
            if (!spinningOut) {
                CancelDrift();
                CancelDriftBoost(true);
                EmptyBoostReserve();
                StartCoroutine(SpinCycle(spinType, Mathf.Max(0, spinCount)));
                spinOutEvent.Invoke();
            }
        }

        // Spin cycle that calculates the current spin angle
        IEnumerator SpinCycle(SpinAxis spinType, float spinAmount) {
            // Spin start
            spinningOut = true;
            float spinDir = Mathf.Sign(0.5f - Random.value);
            float curSpin = 0.0f;
            float maxSpin = spinAmount * Mathf.PI * 2.0f;

            // Actual spin cycle
            while (Mathf.Abs(curSpin) < maxSpin) {
                curSpin += spinDir * spinRate * Mathf.Clamp((maxSpin - Mathf.Abs(curSpin)), 0.1f, 1.0f) * Time.fixedDeltaTime;
                switch (spinType) {
                    case SpinAxis.Yaw:
                        spinForward = new Vector3(Mathf.Sin(curSpin), Mathf.Sin(curSpin * 2.0f) * 0.1f, Mathf.Cos(curSpin));
                        spinUp = Vector3.up;
                        break;
                    case SpinAxis.Roll:
                        spinUp = new Vector3(Mathf.Sin(curSpin), Mathf.Cos(curSpin), 0.0f);
                        break;
                    case SpinAxis.Pitch:
                        spinForward = new Vector3(0.0f, Mathf.Sin(curSpin), Mathf.Cos(curSpin));
                        spinUp = new Vector3(0.0f, Mathf.Cos(curSpin), -Mathf.Sin(curSpin));
                        break;
                }

                if (spinType != SpinAxis.Yaw) {
                    spinOffset = Vector3.up * spinHeight * Mathf.Sin((Mathf.Abs(curSpin) / Mathf.Max(maxSpin, 0.001f)) * Mathf.PI);
                }
                yield return new WaitForFixedUpdate();
            }

            // Spin end
            spinningOut = false;
            spinForward = Vector3.forward;
            spinOffset = Vector3.zero;
            spinUp = Vector3.up;
            boostPadUsed = false;
        }

        // Return whether at least one wheel is sliding
        public bool IsWheelSliding() {
            for (int i = 0; i < wheels.Length; i++) {
                if (wheels[i].sliding) {
                    return true;
                }
            }
            return false;
        }

        // Get the surface type from any wheel
        public GroundSurfacePreset GetWheelSurface() {
            return GetWheelSurface(false);
        }

        // Get the surface type from any wheel, excluding ones that are not sliding based on the argument
        public GroundSurfacePreset GetWheelSurface(bool onlySliding) {
            for (int i = 0; i < wheels.Length; i++) {
                if (wheels[i].grounded && (wheels[i].sliding || !onlySliding)) {
                    return wheels[i].surfaceProps;
                }
            }
            return null;
        }

        void OnTriggerEnter(Collider trig) {
            // Spin out when touching hazard
            Hazard haz = trig.GetComponent<Hazard>();
            if (!spinningOut && haz != null) {
                SpinOut(haz.spinAxis, haz.spinCount);
            }
        }

        void OnTriggerStay(Collider trig) {
            // Boost pad detection
            BoostPad bPad = trig.GetComponent<BoostPad>();
            if (canBoost && grounded && bPad != null && !spinningOut && ((!boostPadUsed && boostPadTimer == 0) || bPad.continuous)) {
                // Single hit boost pad (award boost upon initial touch
                if (!boostPadUsed && boostPadTimer == 0 && !bPad.continuous) {
                    AddBoost(bPad.boostAmount, bPad.boostForce);
                    boostStartEvent.Invoke();
                }
                else if (bPad.continuous) // Continuous boost pad (always boost while touching)
                {
                    if (!boostPadUsed) {
                        boostStartEvent.Invoke();
                    }
                    AddBoost(bPad.boostAmount * Time.fixedDeltaTime);
                }
                boostPadUsed = true;
                boostPadTimer = bPad.delayInterval;
            }
        }

        void OnTriggerExit(Collider trig) {
            // Exiting boost pad
            if (trig.GetComponent<BoostPad>()) {
                boostPadUsed = false;
            }
        }

        void OnCollisionStay(Collision col) {
            if (rotator == null) { return; }

            bool wallHit = false;
            int contactCount = col.GetContacts(collisionContacts);
            for (int i = 0; i < contactCount; i++) {
                ContactPoint curCol = collisionContacts[i];

                // Wall collision detection
                WallCollisionProps wallProps = new WallCollisionProps(curCol, localUpWallDotComparison ? upDir : currentGravityDir, wallCollisionProps.wallDotLimit, wallCollisionProps.wallMask, wallCollisionProps.wallTag);
                if (!wallHit && wallDetector.WallTest(wallProps) && !curCol.otherCollider.IsKart() && !curCol.otherCollider.IsSpawnedProjectileItem()) {
                    wallHit = true;
                    if (wallCollisionCancelsDrift) {
                        CancelDrift();
                        CancelDriftBoost(true);
                    }

                    if (wallCollisionCancelsBoost) {
                        EmptyBoostReserve();
                    }

                    Vector3 localContact = rotator.InverseTransformPoint(curCol.point);
                    wallBounceTurn = F.MaxAbs(wallBounceTurn, col.relativeVelocity.magnitude * Vector3.Dot(-forwardDir, curCol.normal) * (localContact.x > 0 ? -1.0f : 1.0f) * wallBounceTurnAmount);

                    // Wall friction application
                    if (grounded && !spinningOut) {
                        rb.AddForce(-new Vector3(rb.velocity.x, 0.0f, rb.velocity.z) * wallFriction, ForceMode.Acceleration);
                    }
                }
            }
        }

        void OnCollisionEnter(Collision col) {
            bool wallHit = false;
            int contactCount = col.GetContacts(collisionContacts);
            for (int i = 0; i < contactCount; i++) {
                ContactPoint curCol = collisionContacts[i];

                // Spin out upon hazard collision
                Hazard haz = curCol.otherCollider.GetComponent<Hazard>();
                if (!spinningOut && haz != null) {
                    SpinOut(haz.spinAxis, haz.spinCount);
                }

                // Wall collision detection
                if (wallHitTime == 0 && col.relativeVelocity.magnitude > minWallHitSpeed) {
                    WallCollisionProps wallProps = new WallCollisionProps(curCol, localUpWallDotComparison ? upDir : currentGravityDir, wallCollisionProps.wallDotLimit, wallCollisionProps.wallMask, wallCollisionProps.wallTag);
                    if ((!wallHit && wallDetector.WallTest(wallProps) && !curCol.otherCollider.IsSpawnedProjectileItem()) || curCol.otherCollider.IsKart()) {
                        // Wall hit event invocation
                        wallHit = true;
                        wallHitTime = wallHitDuration;
                        collisionEvent.Invoke(curCol.point, col.relativeVelocity);
                    }
                }
            }
        }

        // Set accel input
        public void SetAccel(float accel) {
            accelInput = active ? Mathf.Clamp01(accel) : 0.0f;
        }

        // Set brake input
        public void SetBrake(float brake) {
            brakeInput = active ? Mathf.Clamp01(brake) : 0.0f;
        }

        // Set steer input
        public void SetSteer(float steer) {
            steerInput = active ? Mathf.Clamp(steer, -1.0f, 1.0f) : 0.0f;
        }

        // Set drift input
        public void SetDrift(bool drift) {
            driftButtonDown = active && !driftButton && drift;
            driftButton = active && drift;
        }

        // Set boost input
        public void SetBoost(bool boostIn) {
            boostButtonDown = active && !boostButton && boostIn;
            boostButton = active && boostIn;
        }

        void OnDrawGizmosSelected() {
            if (rotator != null) {
                if (visualHolder != null) {
                    // Draw dimensions
                    Gizmos.color = Color.blue;
                    Gizmos.DrawRay(visualHolder.position, visualHolder.forward * frontLength * 0.5f);
                    Gizmos.DrawRay(visualHolder.position, -visualHolder.forward * backLength * 0.5f);
                    Gizmos.DrawRay(visualHolder.position, visualHolder.right * sideWidth * 0.5f);
                    Gizmos.DrawRay(visualHolder.position, -visualHolder.right * sideWidth * 0.5f);
                }

                // Draw corner cast dimensions
                Vector3[] castPoints = new Vector3[4];
                castPoints[0] = rotator.TransformPoint(cornerCastOffset + new Vector3(cornerCastSize.x * 0.5f, cornerCastSize.y, cornerCastSize.z * 0.5f));
                castPoints[1] = rotator.TransformPoint(cornerCastOffset + new Vector3(cornerCastSize.x * -0.5f, cornerCastSize.y, cornerCastSize.z * 0.5f));
                castPoints[2] = rotator.TransformPoint(cornerCastOffset + new Vector3(cornerCastSize.x * -0.5f, cornerCastSize.y, cornerCastSize.z * -0.5f));
                castPoints[3] = rotator.TransformPoint(cornerCastOffset + new Vector3(cornerCastSize.x * 0.5f, cornerCastSize.y, cornerCastSize.z * -0.5f));

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(castPoints[0], castPoints[1]);
                Gizmos.DrawLine(castPoints[1], castPoints[2]);
                Gizmos.DrawLine(castPoints[2], castPoints[3]);
                Gizmos.DrawLine(castPoints[3], castPoints[0]);

                Gizmos.color = Color.red;
                Gizmos.DrawRay(castPoints[0], -rotator.up * cornerCastDistance);
                Gizmos.DrawRay(castPoints[1], -rotator.up * cornerCastDistance);
                Gizmos.DrawRay(castPoints[2], -rotator.up * cornerCastDistance);
                Gizmos.DrawRay(castPoints[3], -rotator.up * cornerCastDistance);

                // Draw stable wheel points
                Gizmos.color = Color.magenta;
                if (stableWheelPoints != null) {
                    for (int i = 0; i < stableWheelPoints.Length; i++) {
                        Gizmos.DrawRay(rotator.TransformPoint(stableWheelPoints[i]), -rotator.up * wheels[i].suspensionDistance);
                    }
                }

                // Visualize gravity sphere casts for finding nearest surface
                if (drawGravityCastGizmos) {
                    float segAngle = Mathf.PI * 2.0f / gravityCastSegments; // Angle between segments
                    float curSegAngle = 0.0f;
                    float layerAngle = Mathf.PI / gravityCastLayers; // Angle between layers
                    float curLayerAngle = -Mathf.PI * 0.5f + layerAngle;

                    Gizmos.color = Color.blue;
                    Vector3 point = rotator.TransformPoint(Vector3.down * gravityCastDistance); // Cast point directly below kart
                    Vector3 diff = point - rotator.position;
                    Gizmos.DrawWireSphere(point, gravityCastRadius);
                    GizmosExtra.DrawWireCylinder(rotator.position + diff * 0.5f, diff.normalized, gravityCastRadius, diff.magnitude);

                    // Loop through all layers and segments to draw cast points
                    for (int i = 0; i < gravityCastLayers - 1; i++) {
                        for (float j = 0; j < gravityCastSegments; j++) {
                            point = rotator.TransformPoint(new Vector3(Mathf.Sin(curSegAngle) * Mathf.Cos(curLayerAngle), Mathf.Sin(curLayerAngle), Mathf.Cos(curSegAngle) * Mathf.Cos(curLayerAngle)) * gravityCastDistance);
                            diff = point - rotator.position;
                            Gizmos.DrawWireSphere(point, gravityCastRadius);
                            GizmosExtra.DrawWireCylinder(rotator.position + diff * 0.5f, diff.normalized, gravityCastRadius, diff.magnitude);
                            curSegAngle += segAngle;
                        }
                        curLayerAngle += layerAngle;
                    }

                    point = rotator.TransformPoint(Vector3.up * gravityCastDistance); // Cast point directly above kart
                    diff = point - rotator.position;
                    Gizmos.DrawWireSphere(point, gravityCastRadius);
                    GizmosExtra.DrawWireCylinder(rotator.position + diff * 0.5f, diff.normalized, gravityCastRadius, diff.magnitude);
                }
            }
        }
    }
}