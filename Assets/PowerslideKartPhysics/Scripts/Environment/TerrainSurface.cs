// Copyright (c) 2022 Justin Couch / JustInvoke
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PowerslideKartPhysics
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Terrain))]
    // Class for associating terrain textures with ground surface types
    public class TerrainSurface : GroundSurface
    {
        Transform tr;
        TerrainData terDat;
        float[,,] terrainAlphamap;
        public List<GroundSurfacePreset> groundSurfaces = new List<GroundSurfacePreset>();

        protected override void Awake() {
            base.Awake();
            tr = transform;
            if (GetComponent<Terrain>().terrainData != null) {
                terDat = GetComponent<Terrain>().terrainData;
                UpdateAlphamaps();
            }
        }

        // Update alphamap information for finding the most important texture at a position
        public void UpdateAlphamaps() {
            terrainAlphamap = terDat.GetAlphamaps(0, 0, terDat.alphamapWidth, terDat.alphamapHeight);
        }

        // Returns the dominant surface type at the position on terrain based on the most visible texture (greatest alpha)
        public GroundSurfacePreset GetDominantGroundSurfaceAtPoint(Vector3 pos) {
            Vector2 coord = new Vector2(Mathf.Clamp01((pos.z - tr.position.z) / Mathf.Max(terDat.size.z, 0.01f)), Mathf.Clamp01((pos.x - tr.position.x) / terDat.size.x));

            float maxVal = 0.0f;
            float curVal = 0.0f;
            GroundSurfacePreset dominantSurface = GroundSurfacePreset.CreateInstance<GroundSurfacePreset>();

            for (int i = 0; i < terrainAlphamap.GetLength(2); i++) {
                curVal = terrainAlphamap[Mathf.FloorToInt(coord.x * (terDat.alphamapWidth - 1)), Mathf.FloorToInt(coord.y * (terDat.alphamapHeight - 1)), i];

                if (curVal > maxVal) {
                    maxVal = curVal;
                    dominantSurface = groundSurfaces[Mathf.Min(i, groundSurfaces.Count - 1)];
                }
            }

            return dominantSurface;
        }

        // Returns the friction of the indicated surface
        public float GetFriction(GroundSurfacePreset surfaceProps) {
            if (surfaceProps == null) {
                surfaceProps = GroundSurfacePreset.CreateInstance<GroundSurfacePreset>();
            }

            if (surfaceProps.useColliderFriction && col != null) {
                return col.sharedMaterial != null ? col.sharedMaterial.dynamicFriction : 1.0f;
            }
            else {
                return surfaceProps.friction;
            }
        }

        // Returns the friction at the position
        public override float GetFriction(Vector3 pos) {
            return GetFriction(GetDominantGroundSurfaceAtPoint(pos));
        }

        // Returns the speed factor of the indicated surface
        public float GetSpeed(GroundSurfacePreset surfaceProps) {
            if (surfaceProps == null) {
                surfaceProps = GroundSurfacePreset.CreateInstance<GroundSurfacePreset>();
            }

            return surfaceProps.speed;
        }

        // Returns the speed factor at the position
        public override float GetSpeed(Vector3 pos) {
            return GetSpeed(GetDominantGroundSurfaceAtPoint(pos));
        }
    }
}