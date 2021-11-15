// Copyright (c) 2022 Justin Couch / JustInvoke
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerslideKartPhysics
{
    [DisallowMultipleComponent]
    // Class for objects that give items to karts when touched
    public class ItemGiver : MonoBehaviour
    {
        ItemManager manager;
        Collider trig;
        Renderer rend;
        public string itemName;
        public int ammo = 1;
        public float cooldown = 1.0f;
        float offTime = 0.0f;

        private void Awake() {
            manager = FindObjectOfType<ItemManager>();
            trig = GetComponent<Collider>();
            rend = GetComponent<Renderer>();
            offTime = cooldown;
        }

        private void Update() {
            if (trig == null || rend == null) { return; }

            offTime += Time.deltaTime;

            // Disable trigger and renderer during cooldown
            trig.enabled = rend.enabled = offTime >= cooldown;
        }

        private void OnTriggerEnter(Collider other) {
            if (manager != null) {
                // Give item to caster
                ItemCaster caster = other.transform.GetTopmostParentComponent<ItemCaster>();
                if (caster != null) {
                    offTime = 0.0f;

                    // Give specific item if named, otherwise random item
                    caster.GiveItem(
                        string.IsNullOrEmpty(itemName) ? manager.GetRandomItem() : manager.GetItem(itemName),
                        ammo, false);
                }
            }
        }
    }
}