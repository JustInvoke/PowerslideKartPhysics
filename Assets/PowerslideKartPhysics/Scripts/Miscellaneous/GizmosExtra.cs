// Copyright (c) 2022 Justin Couch / JustInvoke
using UnityEngine;
using System.Collections;

namespace PowerslideKartPhysics
{
    // Static class with extra gizmo drawing functions
    public static class GizmosExtra
    {
        // Draws a wire cylinder like DrawWireCube and DrawWireSphere
        // pos = position, dir = direction of the caps, radius = radius, height = height or length, steps = number of line segments to draw around a cap
        public static void DrawWireCylinder(Vector3 pos, Vector3 dir, float radius, float height, int steps) {
            float halfHeight = height * 0.5f;
            Quaternion quat = Quaternion.LookRotation(dir, new Vector3(-dir.y, dir.x, 0.0f));

            Gizmos.DrawLine(pos + quat * new Vector3(radius, 0.0f, halfHeight), pos + quat * new Vector3(radius, 0.0f, -halfHeight));
            Gizmos.DrawLine(pos + quat * new Vector3(-radius, 0.0f, halfHeight), pos + quat * new Vector3(-radius, 0.0f, -halfHeight));
            Gizmos.DrawLine(pos + quat * new Vector3(0.0f, radius, halfHeight), pos + quat * new Vector3(0.0f, radius, -halfHeight));
            Gizmos.DrawLine(pos + quat * new Vector3(0.0f, -radius, halfHeight), pos + quat * new Vector3(0.0f, -radius, -halfHeight));

            Vector3 circle0Point0;
            Vector3 circle0Point1;
            Vector3 circle1Point0;
            Vector3 circle1Point1;

            steps = Mathf.Max(steps, 3);
            float interval = Mathf.PI * 2.0f / steps;
            for (float i = 0; i < Mathf.PI * 2.0f; i += interval) {
                circle0Point0 = pos + quat * new Vector3(Mathf.Sin(i) * radius, Mathf.Cos(i) * radius, halfHeight);
                circle0Point1 = pos + quat * new Vector3(Mathf.Sin(i + interval) * radius, Mathf.Cos(i + interval) * radius, halfHeight);
                Gizmos.DrawLine(circle0Point0, circle0Point1);

                circle1Point0 = pos + quat * new Vector3(Mathf.Sin(i) * radius, Mathf.Cos(i) * radius, -halfHeight);
                circle1Point1 = pos + quat * new Vector3(Mathf.Sin(i + interval) * radius, Mathf.Cos(i + interval) * radius, -halfHeight);
                Gizmos.DrawLine(circle1Point0, circle1Point1);
            }
        }

        // Same as above but with a default of 20 line segments drawn for both caps
        public static void DrawWireCylinder(Vector3 pos, Vector3 dir, float radius, float height) {
            DrawWireCylinder(pos, dir, radius, height, 20);
        }
    }
}