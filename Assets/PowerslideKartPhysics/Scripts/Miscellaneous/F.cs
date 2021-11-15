// Copyright (c) 2022 Justin Couch / JustInvoke
using UnityEngine;
using System.Collections;

namespace PowerslideKartPhysics
{
    // Static class with extra functions
    public static class F
    {
        // Returns the number with the greatest absolute value
        public static float MaxAbs(params float[] nums) {
            float result = 0;

            for (int i = 0; i < nums.Length; i++) {
                if (Mathf.Abs(nums[i]) > Mathf.Abs(result)) {
                    result = nums[i];
                }
            }

            return result;
        }

        // Returns the topmost parent of a Transform with a certain component
        public static T GetTopmostParentComponent<T>(this Transform tr) where T : Component {
            T getting = tr.GetComponent<T>();

            while (tr.parent != null) {
                if (tr.parent.GetComponent<T>() != null) {
                    getting = tr.parent.GetComponent<T>();
                }

                tr = tr.parent;
            }

            return getting;
        }

        // Returns whether an object has a certain component or is a child of an object with a certain component
        public static bool Is<T>(this Component obj) where T : Component {
            return obj.transform.GetTopmostParentComponent<T>() != null;
        }

        // Returns whether an object has a Kart component or is a child of an object with a Kart component
        public static bool IsKart(this Component obj) {
            return obj.Is<Kart>();
        }

        // Returns whether an object has a SpawnedProjectileItem component or is a child of an object with a SpawnedProjectileItem component
        public static bool IsSpawnedProjectileItem(this Component obj) {
            return obj.Is<SpawnedProjectileItem>();
        }

        // Returns whether an object has a Wall component or is a child of an object with a Wall component
        public static bool IsWall(this Component obj) {
            return obj.Is<Wall>();
        }

        // Returns whether a LayerMask contains a certain layer
        public static bool Contains(this LayerMask mask, int layer) {
            return mask == (mask | (1 << layer));
        }
    }
}