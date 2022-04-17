// Copyright (c) 2022 Justin Couch / JustInvoke
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PowerslideKartPhysics
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    // Class for spawned projectile items
    public class SpawnedProjectileItem : MonoBehaviour
    {
        Transform tr;
        Rigidbody rb;
        Collider col;
        ItemCastProperties castProps;

        bool grounded = false;
        [Header("Movement")]
        public LayerMask groundMask = 1;
        public float groundCheckDistance = 1.0f;
        Vector3 groundNormal = Vector3.up;
        Vector3 groundPoint = Vector3.zero;

        public float launchHeight = 0.0f;
        public float startSpeed = 0.0f;
        public float targetSpeed = 0.0f;
        public bool inheritKartSpeed = true;
        public bool maintainKartSpeed = true;
        public float accel = 10f;
        Vector3 moveDir = Vector3.forward;
        public bool moveInAir = false;
        public float gravityAdd = -10f;
        public Vector3 gravityDir = Vector3.up;
        Vector3 currentGravityDir = Vector3.up;
        public bool inheritKartGravity = false;
        public bool gravityIsGroundNormal = false;
        public bool resetGravityDirInAir = true;
        public float forwardFriction = 0.0f;
        public float sideFriction = 10f;
        public float maxFallSpeed = 20f;
        public float fallSpeedDecel = 10f;

        [Header("Walls")]
        public bool wallBounceReflect = true;
        public bool itemBounceReflect = true;
        public float bounceReflectForce = 1.0f;
        public int maxBounces = 3;
        int bounces = 0;
        public bool destroyOnWallHit = false;
        public bool destroyOnItemHit = false;

        WallCollision wallDetector;
        public WallDetectProps wallCollisionProps = WallDetectProps.Default;

        float lifeTime = 0.0f;
        [Header("Caster")]
        public float casterIgnoreTime = 0.5f;
        public bool canHitCaster = true;
        Collider casterCol;
        Kart[] allKarts;
        Kart targetKart;
        public bool fetchKartsDuringSpawn = false;

        [Header("Homing")]
        public float homingAccuracy = 10f;
        public bool prioritizeKartsInFront = true;
        [Range(-1.0f, 1.0f)]
        public float minHomingAngle = 0.0f;
        public float maxHomingDist = 30f;
        public bool useLineOfSight = true;
        public LayerMask lineOfSightMask = 1;
        public bool findTargetWhileActive = true;

        [Header("Spin")]
        public Kart.SpinAxis kartSpin = Kart.SpinAxis.Yaw;
        public int kartSpinCount = 1;

        [Header("Events")]
        public UnityEvent collideEvent;
        public UnityEvent destroyEvent;

        protected virtual void Awake() {
            tr = transform;
            rb = GetComponent<Rigidbody>();
            col = GetComponent<Collider>();
            wallDetector = WallCollision.CreateFromType(wallCollisionProps.wallDetectionType);
        }

        // Initialze spawned item with the given launch properties
        public virtual void Initialize(ItemCastProperties props) {
            castProps = props;
            casterCol = props.castCollider;
            moveDir = (props.castDirection + props.castRotation * Vector3.up * launchHeight).normalized;

            // Match casting kart speed
            if (inheritKartSpeed) {
                rb.velocity = props.castKartVelocity + moveDir * (props.castSpeed + startSpeed);
            }
            else {
                rb.velocity = moveDir * (props.castSpeed + startSpeed);
            }

            if (inheritKartGravity) {
                currentGravityDir = props.castGravity;
            }

            if (targetSpeed > 0 && maintainKartSpeed) {
                targetSpeed += props.castKartVelocity.magnitude;
            }

            if (fetchKartsDuringSpawn) {
                allKarts = FindObjectsOfType<Kart>();
            }
            else {
                allKarts = props.allKarts;
            }

            if (homingAccuracy > 0) {
                FindHomingTarget();
            }
        }

        // Sets the target kart to the given kart
        public virtual void SetHomingTarget(Kart target) {
            targetKart = target;
        }

        // Finds the best target kart to follow
        public virtual void FindHomingTarget() {
            if (homingAccuracy > 0 && allKarts != null) {
                float closeDist = -1.0f;
                float closeAngle = -1.0f;
                for (int i = 0; i < allKarts.Length; i++) {
                    if (allKarts[i] != castProps.castKart) {
                        float curDist = (allKarts[i].transform.position - tr.position).sqrMagnitude;
                        float curAngle = Vector3.Dot((allKarts[i].transform.position - castProps.castPoint).normalized, moveDir);
                        bool lineOfSight = !useLineOfSight || !Physics.Linecast(tr.position, allKarts[i].transform.position, lineOfSightMask, QueryTriggerInteraction.Ignore);

                        if (i == 0) {
                            closeDist = curDist;
                            closeAngle = curAngle;
                        }

                        if (curDist <= maxHomingDist * maxHomingDist && curAngle >= minHomingAngle && lineOfSight) {
                            if (prioritizeKartsInFront) {
                                if (curAngle > closeAngle || i == 0) {
                                    closeAngle = curAngle;
                                    SetHomingTarget(allKarts[i]);
                                }
                            }
                            else if (curDist < closeDist || i == 0) {
                                closeDist = curDist;
                                SetHomingTarget(allKarts[i]);
                            }
                        }
                    }
                }
            }
        }

        protected virtual void FixedUpdate() {
            if (rb == null || col == null) { return; }

            lifeTime += Time.fixedDeltaTime;
            rb.AddForce(currentGravityDir * gravityAdd, ForceMode.Acceleration); // Apply fake gravity

            // Ignore collision with casting kart
            if (col != null && casterCol != null) {
                Physics.IgnoreCollision(col, casterCol, lifeTime <= casterIgnoreTime || !canHitCaster);
            }

            // Check to see if grounded
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(tr.position, -currentGravityDir, out hit, groundCheckDistance, groundMask, QueryTriggerInteraction.Ignore)) {
                grounded = true;
                groundNormal = hit.normal;
                groundPoint = hit.point;

                if (gravityIsGroundNormal) {
                    currentGravityDir = groundNormal;
                }
            }
            else {
                grounded = false;
                groundNormal = Vector3.up;

                if (resetGravityDirInAir) {
                    currentGravityDir = gravityDir.normalized;
                }
            }

            // Limit falling speed
            float velGravDot = -Vector3.Dot(rb.velocity, currentGravityDir);
            if (!grounded && velGravDot > maxFallSpeed) {
                rb.AddForce(currentGravityDir * (velGravDot - maxFallSpeed) * fallSpeedDecel, ForceMode.Acceleration);
            }

            // Look for homing target
            if (findTargetWhileActive && homingAccuracy > 0 && targetKart == null) {
                FindHomingTarget();
            }

            // Adjust movement direction toward target
            if ((grounded || moveInAir) && homingAccuracy > 0 && targetKart != null) {
                Vector3 targetDir = targetKart.transform.position - tr.position;
                moveDir = Vector3.Slerp(moveDir, targetDir.normalized, homingAccuracy * Time.fixedDeltaTime);
            }
            else {
                moveDir = Vector3.ProjectOnPlane(moveDir, currentGravityDir).normalized;
            }

            Quaternion moveRot = Quaternion.LookRotation(moveDir, currentGravityDir);
            Vector3 forwardDir = moveRot * Vector3.forward;
            Vector3 rightDir = moveRot * Vector3.right;
            Vector3 localVel = Vector3.forward * Vector3.Dot(forwardDir, rb.velocity) + Vector3.right * Vector3.Dot(rightDir, rb.velocity);
            Debug.DrawRay(tr.position, forwardDir * localVel.z, Color.blue);
            Debug.DrawRay(tr.position, rightDir * localVel.x, Color.red);

            if (grounded || moveInAir) {
                if (targetSpeed > 0) {
                    // Add movement force
                    rb.AddForce(forwardDir * (targetSpeed - localVel.z) * accel, ForceMode.Acceleration);
                }

                // Add friction forces
                rb.AddForce(forwardDir * -localVel.z * forwardFriction, ForceMode.Acceleration);
                rb.AddForce(rightDir * -localVel.x * sideFriction, ForceMode.Acceleration);
            }
        }

        ContactPoint[] collisionContacts = new ContactPoint[2]; // More than two contact points to check is probably unnecessary, but you are free to increase this
        protected virtual void OnCollisionEnter(Collision colHit) {
            int contactCount = colHit.GetContacts(collisionContacts);
            for (int i = 0; i < contactCount; i++) {
                ContactPoint curCol = collisionContacts[i];
                WallCollisionProps wallProps = new WallCollisionProps(curCol, currentGravityDir, wallCollisionProps.wallDotLimit, wallCollisionProps.wallMask, wallCollisionProps.wallTag);
                bool wallHit = wallDetector.WallTest(wallProps);
                bool itemHit = curCol.otherCollider.IsSpawnedProjectileItem();

                if (curCol.otherCollider.IsKart()) {
                    if (curCol.otherCollider != casterCol || (lifeTime > casterIgnoreTime && canHitCaster && curCol.otherCollider == casterCol)) {
                        // Spin out kart upon collision
                        curCol.otherCollider.transform.GetTopmostParentComponent<Kart>().SpinOut(kartSpin, kartSpinCount);
                        Destroy(gameObject);
                    }
                }
                else if ((wallHit && destroyOnWallHit) || (itemHit && destroyOnItemHit)) {
                    // Destroy upon wall collision
                    Destroy(gameObject);
                }
                else {
                    // Bounce collision logic
                    if ((wallBounceReflect && wallHit) || (itemBounceReflect && itemHit)) {
                        moveDir = Vector3.ProjectOnPlane(Vector3.Reflect(moveDir, curCol.normal), currentGravityDir).normalized;
                        rb.velocity = Vector3.Reflect(rb.velocity, curCol.normal) * bounceReflectForce;
                        targetSpeed *= bounceReflectForce;

                        bounces++;
                        if (bounces > maxBounces) {
                            Destroy(gameObject);
                        }
                        else {
                            collideEvent.Invoke();
                        }
                    }
                }
            }
        }

        private void OnDestroy() {
            destroyEvent.Invoke();
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, -gravityDir.normalized * groundCheckDistance);
        }
    }
}