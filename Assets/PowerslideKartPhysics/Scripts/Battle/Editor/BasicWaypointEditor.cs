using UnityEngine;
using UnityEditor;

namespace PowerslideKartPhysics
{
    [CustomEditor(typeof(BasicWaypoint))]
    [CanEditMultipleObjects]
    public class BasicWaypointEditor : Editor
    {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            if (GUILayout.Button("Snap to ground")) {
                foreach (var waypoint in serializedObject.targetObjects) {
                    (waypoint as BasicWaypoint).SnapToGround();
                }
            }
        }
    }
}
