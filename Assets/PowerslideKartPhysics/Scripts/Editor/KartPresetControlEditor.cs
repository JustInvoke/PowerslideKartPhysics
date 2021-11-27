// Copyright (c) 2022 Justin Couch / JustInvoke
#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

namespace PowerslideKartPhysics
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(KartPresetControl))]
    // Custom editor for KartPresetControl class
    public class KartPresetControlEditor : Editor
    {
        static bool showButtons = false;
        KartPresetControl presetControl;

        private void OnEnable() {
            presetControl = (KartPresetControl)target;
        }

        public override void OnInspectorGUI() {
            if (presetControl == null) {
                base.OnInspectorGUI();
                return;
            }

            GUIStyle buttonStyle = new GUIStyle(EditorStyles.toolbarButton);
            buttonStyle.fontSize = 11;
            buttonStyle.fixedHeight = EditorGUIUtility.singleLineHeight * 1.3f;

            serializedObject.Update();

            SerializedProperty prop = serializedObject.GetIterator();

            if (prop.NextVisible(true)) {
                EditorGUILayout.PropertyField(prop);
            }

            if (prop.NextVisible(false)) {
                EditorGUILayout.PropertyField(prop);
            }

            // Draw save and load buttons
            showButtons = EditorGUILayout.Foldout(showButtons, "Save/Load Buttons", false, EditorStyles.foldoutHeader);
            if (showButtons) {
                DrawSection(presetControl, "dimensionsPreset", "Dimensions", buttonStyle);
                DrawSection(presetControl, "speedPreset", "Speed", buttonStyle);
                DrawSection(presetControl, "steerPreset", "Steer", buttonStyle);
                DrawSection(presetControl, "suspensionPreset", "Suspension", buttonStyle);
                DrawSection(presetControl, "wheelsPreset", "Wheels", buttonStyle);
                DrawSection(presetControl, "jumpPreset", "Jump", buttonStyle);
                DrawSection(presetControl, "gravityPreset", "Gravity", buttonStyle);
                DrawSection(presetControl, "driftPreset", "Drift", buttonStyle);
                DrawSection(presetControl, "boostPreset", "Boost", buttonStyle);
                DrawSection(presetControl, "wallsPreset", "Walls", buttonStyle);
            }

            serializedObject.ApplyModifiedProperties();
        }

        // Draw buttons for saving and loading each preset type
        void DrawSection(KartPresetControl kpc, string prop, string buttonName, GUIStyle style) {
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(prop));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save " + buttonName, style)) {
                switch (buttonName) {
                    case "Dimensions":
                        presetControl.SaveDimensionsPreset(presetControl.dimensionsPreset);
                        break;
                    case "Speed":
                        presetControl.SaveSpeedPreset(presetControl.speedPreset);
                        break;
                    case "Steer":
                        presetControl.SaveSteerPreset(presetControl.steerPreset);
                        break;
                    case "Suspension":
                        presetControl.SaveSuspensionPreset(presetControl.suspensionPreset);
                        break;
                    case "Wheels":
                        presetControl.SaveWheelsPreset(presetControl.wheelsPreset);
                        break;
                    case "Jump":
                        presetControl.SaveJumpPreset(presetControl.jumpPreset);
                        break;
                    case "Gravity":
                        presetControl.SaveGravityPreset(presetControl.gravityPreset);
                        break;
                    case "Drift":
                        presetControl.SaveDriftPreset(presetControl.driftPreset);
                        break;
                    case "Boost":
                        presetControl.SaveBoostPreset(presetControl.boostPreset);
                        break;
                    case "Walls":
                        presetControl.SaveWallsPreset(presetControl.wallsPreset);
                        break;
                }
            }

            if (GUILayout.Button("Load " + buttonName, style)) {
                switch (buttonName) {
                    case "Dimensions":
                        presetControl.LoadDimensionsPreset(presetControl.dimensionsPreset);
                        break;
                    case "Speed":
                        presetControl.LoadSpeedPreset(presetControl.speedPreset);
                        break;
                    case "Steer":
                        presetControl.LoadSteerPreset(presetControl.steerPreset);
                        break;
                    case "Suspension":
                        presetControl.LoadSuspensionPreset(presetControl.suspensionPreset);
                        break;
                    case "Wheels":
                        presetControl.LoadWheelsPreset(presetControl.wheelsPreset);
                        break;
                    case "Jump":
                        presetControl.LoadJumpPreset(presetControl.jumpPreset);
                        break;
                    case "Gravity":
                        presetControl.LoadGravityPreset(presetControl.gravityPreset);
                        break;
                    case "Drift":
                        presetControl.LoadDriftPreset(presetControl.driftPreset);
                        break;
                    case "Boost":
                        presetControl.LoadBoostPreset(presetControl.boostPreset);
                        break;
                    case "Walls":
                        presetControl.LoadWallsPreset(presetControl.wallsPreset);
                        break;
                }

                SerializedObject kart = new SerializedObject(kpc.GetComponent<Kart>());
                EditorUtility.SetDirty(kpc.GetComponent<Kart>());
                kart.ApplyModifiedPropertiesWithoutUndo();
            }
            GUILayout.EndHorizontal();

        }
    }
}
#endif