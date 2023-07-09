// Copyright (c) 2023 Justin Couch / JustInvoke
using UnityEngine;

namespace PowerslideKartPhysics
{
    [DisallowMultipleComponent]
    public class KartSteeringWheel : MonoBehaviour
    {
        Transform tr;
        Kart kartParent;
        public float steerAmount = 50f;
        public Vector3 rotateAxis = Vector3.forward;
        public float angleOffset = 0.0f;
        Quaternion baseRot = Quaternion.identity;
        public KartWheel attachWheel;

        void Awake() {
            tr = transform;
            kartParent = tr.GetTopmostParentComponent<Kart>();
            baseRot = tr.localRotation;
        }

        void Update() {
            if (kartParent == null) { return; }
            tr.localRotation = baseRot * Quaternion.AngleAxis((attachWheel != null ? attachWheel.visualSteerAngle : kartParent.GetVisualSteer() * steerAmount) + angleOffset, rotateAxis);
        }
    }
}
