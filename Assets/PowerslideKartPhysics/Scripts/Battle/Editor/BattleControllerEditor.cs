using UnityEngine;
using UnityEditor;

namespace PowerslideKartPhysics
{
    [CustomEditor(typeof(BattleController))]
    [CanEditMultipleObjects]
    public class BattleControllerEditor : Editor
    {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            if (GUILayout.Button("Snap waypoints to ground")) {
                foreach (var controller in serializedObject.targetObjects) {
                    (controller as BattleController).SnapWaypointsToGround();
                }
            }

            if (GUILayout.Button("Calculate waypoint connections")) {
                foreach (var controller in serializedObject.targetObjects) {
                    (controller as BattleController).CalculateWaypointConnections();
                }
                SceneView.RepaintAll();
            }
        }
    }
}
