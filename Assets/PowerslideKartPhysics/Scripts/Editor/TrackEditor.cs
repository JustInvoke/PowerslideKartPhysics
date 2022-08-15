// Copyright (c) 2022 Justin Couch / JustInvoke
#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

namespace PowerslideKartPhysics
{
    [CustomEditor(typeof(Track))]
    // Custom editor for the Track class
    public class TrackEditor : Editor
    {
        Track targetTrack;

        private void OnEnable() {
            targetTrack = (Track)target;
        }

        public override void OnInspectorGUI() {
            // Button for calculating waypoint indices
            if (GUILayout.Button("Calculate Waypoint Indices")) {
                targetTrack.CalculateWaypointIndices();
                EditorUtility.SetDirty(targetTrack);
            }
            DrawDefaultInspector();
        }
    }
}
#endif