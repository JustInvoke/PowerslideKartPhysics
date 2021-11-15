// Copyright (c) 2022 Justin Couch / JustInvoke
#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

namespace PowerslideKartPhysics
{
    [CustomEditor(typeof(TerrainSurface))]
    // Custom editor for the TerrainSurface class
    public class TerrainSurfaceEditor : Editor
    {
        TerrainSurface targetTerrain;

        private void OnEnable() {
            targetTerrain = (TerrainSurface)target;
        }

        public override void OnInspectorGUI() {
            // Draw default inspector if no terrain surface attached
            if (targetTerrain == null) {
                base.OnInspectorGUI();
                return;
            }

            SerializedProperty serializedTerrainSurfaces = serializedObject.FindProperty("groundSurfaces");

            TerrainData terDat = targetTerrain.GetComponent<Terrain>().terrainData;
            if (terDat == null) { return; }

            // Draw labels for each texture found on the terrain
            EditorGUILayout.LabelField("Terrain Textures:", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            for (int i = 0; i < terDat.terrainLayers.Length; i++) {
                EditorGUILayout.LabelField(i.ToString() + ": " + terDat.terrainLayers[i].diffuseTexture.name, EditorStyles.label);
            }
            EditorGUI.indentLevel--;

            // Adjust the number of surface types in the TerrainSurface to match the number of textures on the terrain
            EditorGUILayout.LabelField("Corresponding Surface Properties:", EditorStyles.boldLabel);
            while (targetTerrain.groundSurfaces.Count != terDat.terrainLayers.Length) {
                if (targetTerrain.groundSurfaces.Count > terDat.terrainLayers.Length) {
                    targetTerrain.groundSurfaces.RemoveAt(targetTerrain.groundSurfaces.Count - 1);
                }
                else {
                    targetTerrain.groundSurfaces.Add(null);
                }
            }

            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedTerrainSurfaces, true);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif