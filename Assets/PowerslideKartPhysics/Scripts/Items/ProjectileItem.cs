// Copyright (c) 2022 Justin Couch / JustInvoke
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerslideKartPhysics
{
    // Class for projectile items (this is the casting/spawning class, not the actual spawned items. See SpawnedProjectileItem for spawned item class)
    public class ProjectileItem : Item
    {
        public GameObject itemPrefab;
        GameObject spawnedItem;
        public Vector3 spawnOffset;

        public override void Activate(ItemCastProperties props) {
            base.Activate(props);
            if (itemPrefab != null) {
                // Spawn projectile upon activation
                spawnedItem = Instantiate(itemPrefab, castProps.castPoint + castProps.castRotation * spawnOffset, castProps.castRotation);
                SpawnedProjectileItem projectile = spawnedItem.GetComponent<SpawnedProjectileItem>();
                if (projectile != null) {
                    projectile.Initialize(castProps);
                }
            }
        }

        // Destroy spawned item upon deactivation
        public override void Deactivate() {
            base.Deactivate();
            if (spawnedItem != null) {
                Destroy(spawnedItem);
                spawnedItem = null;
            }
        }
    }
}