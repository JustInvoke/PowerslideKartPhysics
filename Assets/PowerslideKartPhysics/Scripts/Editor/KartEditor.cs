// Copyright (c) 2022 Justin Couch / JustInvoke
#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

namespace PowerslideKartPhysics
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Kart))]
    // Custom editor for the Kart class
    public class KartEditor : Editor
    {
        static bool showDimensions = false;
        static bool showSpeed = false;
        static bool showSteer = false;
        static bool showSuspension = false;
        static bool showWheels = false;
        static bool showJump = false;
        static bool showGravity = false;
        static bool showDrift = false;
        static bool showBoost = false;
        static bool showWalls = false;
        static bool showEvents = false;

        public override void OnInspectorGUI() {
            serializedObject.Update();

            // Organize variables into foldouts
            bool hiding = false;
            SerializedProperty prop = serializedObject.GetIterator();
            if (prop.NextVisible(true)) {
                do {
                    DrawSection(prop, "Dimensions", "rotationRateFactor", "maxSpeed", ref showDimensions, ref hiding, EditorStyles.foldoutHeader);
                    DrawSection(prop, "Speed", "maxSpeed", "steerRate", ref showSpeed, ref hiding, EditorStyles.foldoutHeader);
                    DrawSection(prop, "Steer", "steerRate", "springForce", ref showSteer, ref hiding, EditorStyles.foldoutHeader);
                    DrawSection(prop, "Suspension", "springForce", "wheels", ref showSuspension, ref hiding, EditorStyles.foldoutHeader);
                    DrawSection(prop, "Wheels", "wheels", "canJump", ref showWheels, ref hiding, EditorStyles.foldoutHeader);
                    DrawSection(prop, "Jump", "canJump", "gravityAdd", ref showJump, ref hiding, EditorStyles.foldoutHeader);
                    DrawSection(prop, "Gravity", "gravityAdd", "canDrift", ref showGravity, ref hiding, EditorStyles.foldoutHeader);
                    DrawSection(prop, "Drift", "canDrift", "boostType", ref showDrift, ref hiding, EditorStyles.foldoutHeader);

                    // Show certain boost variables based on the boost type
                    if (prop.name == "boostType" && prop.propertyType == SerializedPropertyType.Enum) {
                        showBoost = EditorGUILayout.Foldout(showBoost, "Boost", false, EditorStyles.foldoutHeader);
                        hiding = true;
                        DrawBoostProps((KartBoostType)prop.enumValueIndex);
                    }

                    if (prop.name == "wallFriction") {
                        hiding = false;
                    }

                    DrawSection(prop, "Walls", "wallFriction", "jumpEvent", ref showWalls, ref hiding, EditorStyles.foldoutHeader);

                    if (prop.name == "jumpEvent") {
                        showEvents = EditorGUILayout.Foldout(showEvents, "Events", false, EditorStyles.foldoutHeader);
                        if (!showEvents) {
                            hiding = true;
                        }
                    }


                    if (!hiding) {
                        if (prop.name == "gravityAdd") {
                            GUIStyle noticeLabel = new GUIStyle(GUI.skin.label);
                            noticeLabel.fontStyle = FontStyle.BoldAndItalic;
                            EditorGUILayout.HelpBox("Custom gravity stacks on top of global physics gravity. Consider disabling rigidbody 'use gravity' and adjusting 'gravity add' to compensate.", MessageType.Info, true);
                        }
                        EditorGUILayout.PropertyField(prop, true);
                    }
                }
                while (prop.NextVisible(false));
            }

            serializedObject.ApplyModifiedProperties();
        }

        // Draws a foldout category (technically, sets hide/show state based on variable names)
        void DrawSection(SerializedProperty prop, string header, string startProp, string endProp, ref bool show, ref bool hide, GUIStyle style) {
            if (prop.name == startProp) {
                show = EditorGUILayout.Foldout(show, header, false, style);
                if (!show) {
                    hide = true;
                }
            }

            if (!show && prop.name == endProp) {
                hide = false;
            }
        }

        // Certain boost variables are hidden based on the boost type
        void DrawBoostProps(KartBoostType boostType) {
            if (showBoost) {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("boostType"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("canBoost"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("boostSpeedAdd"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("boostAccelAdd"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("boostDrive"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("boostPower"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("boostRate"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("boostBurnRate"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("boostGroundPush"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("boostAirPush"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("airLandBoost"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("boostReserveLimit"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("brakeCancelsBoost"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("wallCollisionCancelsBoost"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("boostWheelie"));

                EditorGUI.indentLevel++;
                if (boostType == KartBoostType.DriftAuto) {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("maxBoosts"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("autoBoostInterval"));
                }
                else if (boostType == KartBoostType.DriftManual) {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("maxBoosts"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("driftManualBoostLimit"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("driftManualFailCancel"));
                }
                else if (boostType == KartBoostType.Manual) {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("boostAmount"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("boostAmountLimit"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("driftBoostAdd"));
                }
                EditorGUI.indentLevel--;
            }
        }
    }
}
#endif