// Copyright (c) 2022 Justin Couch / JustInvoke
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PowerslideKartPhysics
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Kart))]
    // Class for using items, attached to kart
    public class ItemCaster : MonoBehaviour
    {
        Kart kart;
        Transform kartTr;
        Rigidbody kartRb;
        Collider kartCol;
        public Item item;
        public int ammo = 0;
        public float minCastInterval = 0.1f;
        float timeSinceCast = 0.0f;
        public UnityEvent castEvent;

        private void Awake() {
            kart = GetComponent<Kart>();
            if (kart != null) {
                kartTr = kart.transform;
                kartRb = kart.GetComponent<Rigidbody>();
                if (kart.rotator != null) {
                    kartCol = kart.rotator.GetComponent<Collider>();
                }
            }
        }

        private void Update() {
            timeSinceCast += Time.deltaTime;
        }

        // Cast currently equipped item
        public void Cast() {
            if (item != null && kart != null && ammo > 0 && timeSinceCast >= minCastInterval) {
                if (kart.active && !kart.spinningOut) {
                    ammo = Mathf.Max(0, ammo - 1);
                    timeSinceCast = 0.0f;
                    ItemCastProperties props = new ItemCastProperties();
                    props.castKart = kart;

                    if (kartRb != null) {
                        props.castKartVelocity = kartRb.velocity;
                    }

                    props.castGravity = kart.currentGravityDir;
                    props.castPoint = kartTr.position;

                    if (kart.rotator != null) {
                        props.castRotation = kart.rotator.rotation;
                    }

                    props.castCollider = kartCol;
                    props.castDirection = kart.forwardDir;
                    item.Activate(props);
                    castEvent.Invoke();
                }
            }
        }

        // Equip the specified single-use item
        public void GiveItem(Item givenItem) {
            GiveItem(givenItem, 1, true);
        }

        // Equip the specified item with the ammo amount
        public void GiveItem(Item givenItem, int ammoCount) {
            GiveItem(givenItem, ammoCount, true);
        }

        // Equip the specified item with the ammo amount, overwriting currently equipped item if bypass is true
        public void GiveItem(Item givenItem, int ammoCount, bool bypass) {
            if (bypass || ammo == 0) {
                item = givenItem;
                ammo = ammoCount;
            }
        }
    }

    // Struct for passing item cast data
    public struct ItemCastProperties
    {
        public Kart castKart;
        public Kart[] allKarts;
        public Vector3 castKartVelocity;
        public Vector3 castPoint;
        public Quaternion castRotation;
        public Vector3 castDirection;
        public float castSpeed;
        public Vector3 castGravity;
        public Collider castCollider;
    }
}
