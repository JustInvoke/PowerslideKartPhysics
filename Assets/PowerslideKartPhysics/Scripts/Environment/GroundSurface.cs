// Copyright (c) 2022 Justin Couch / JustInvoke
using UnityEngine;
using System.Collections;

namespace PowerslideKartPhysics
{
    // Class for representing ground surface types
    [DisallowMultipleComponent]
    public class GroundSurface : MonoBehaviour
    {
        public GroundSurfacePreset props;
        protected Collider col;

        protected virtual void Awake() {
            col = GetComponent<Collider>();
        }

        // Returns the surface properties
        public GroundSurfacePreset GetProps() {
            return props != null ? props : GroundSurfacePreset.CreateInstance<GroundSurfacePreset>();
        }

        // Returns the friction of the surface
        public virtual float GetFriction() {
            if (props == null) {
                props = GroundSurfacePreset.CreateInstance<GroundSurfacePreset>();
            }

            if (props.useColliderFriction && col != null) {
                return col.sharedMaterial != null ? col.sharedMaterial.dynamicFriction : 1.0f;
            }
            else {
                return props.friction;
            }
        }

        // Returns the friction of the surface at the position (for override by TerrainSurface)
        public virtual float GetFriction(Vector3 pos) {
            return GetFriction();
        }

        // Returns the speed factor of the surface
        public virtual float GetSpeed() {
            if (props == null) {
                props = GroundSurfacePreset.CreateInstance<GroundSurfacePreset>();
            }

            return props.speed;
        }

        // Returns the speed factor of the surface at the position (for override by TerrainSurface)
        public virtual float GetSpeed(Vector3 pos) {
            return GetSpeed();
        }
    }
}
