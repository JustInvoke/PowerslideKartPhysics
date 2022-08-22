using UnityEditor;
using UnityEngine;

namespace PowerslideKartPhysics
{
    [CustomEditor(typeof(BattleWaypoint))]
    [CanEditMultipleObjects]
    public class BattleWaypointEditor : BasicWaypointEditor
    {
        public override void OnInspectorGUI() {
            foreach (var waypoint in serializedObject.targetObjects) {
                BattleWaypoint point = (waypoint as BattleWaypoint);
                point.radius = point.GetComponent<SphereCollider>().radius;
            }
            base.OnInspectorGUI();
            if (GUILayout.Button("Calculate connections")) {
                foreach (var waypoint in serializedObject.targetObjects) {
                    (waypoint as BattleWaypoint).CalculateConnections();
                }
                SceneView.RepaintAll();
            }
        }
    }
}
