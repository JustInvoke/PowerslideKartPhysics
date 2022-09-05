using UnityEngine;
using UnityEditor;

namespace PowerslideKartPhysics
{
    [CustomEditor(typeof(ItemGiver))]
    [CanEditMultipleObjects]
    public class ItemGiverEditor : Editor
    {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            if (GUILayout.Button("Snap to ground")) {
                foreach (var giver in serializedObject.targetObjects) {
                    (giver as ItemGiver).SnapToGround();
                }
            }
        }
    }
}
