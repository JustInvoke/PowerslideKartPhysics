// Copyright (c) 2022 Justin Couch / JustInvoke
using UnityEngine;
using System.Collections;

namespace PowerslideKartPhysics
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(KartWheel))]
    // This class creates tire marks as a wheel slides
    public class TireMarkMaker : MonoBehaviour
    {
        Transform tr;
        KartWheel wheel;
        Mesh mesh;
        int[] tris;
        Vector3[] verts;
        Vector2[] uvs;
        Color[] colors;

        Vector3 leftPoint = Vector3.zero;
        Vector3 rightPoint = Vector3.zero;
        Vector3 leftPointPrev = Vector3.zero;
        Vector3 rightPointPrev = Vector3.zero;

        bool creatingMark = false;
        bool continueMark = false;
        GameObject curMark;
        Transform curMarkTr;
        int curEdge = 0;
        float gapDelay = 0.0f;

        GroundSurfacePreset defaultSurface;
        GroundSurfacePreset curSurface;
        GroundSurfacePreset prevSurface;

        float continuousSlide = 0.0f;

        public float markHeight = 0.1f;
        public int markLength = 20;
        public float markGap = 0.2f;
        public float lifeTime = 5.0f;
        public float fadeRate = 1.0f;
        public float markOffset = 0.0f;

        public bool calculateNormals = true;
        public bool calculateTangents = false;

        public Material defaultMarkMaterial;
        public UnityEngine.Rendering.ShadowCastingMode markShadowCastMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        public UnityEngine.Rendering.LightProbeUsage markLightProbeMode = UnityEngine.Rendering.LightProbeUsage.BlendProbes;
        public UnityEngine.Rendering.ReflectionProbeUsage markReflectionProbeMode = UnityEngine.Rendering.ReflectionProbeUsage.BlendProbes;

        void Awake() {
            tr = transform;
            wheel = GetComponent<KartWheel>();
            defaultSurface = GroundSurfacePreset.CreateInstance<GroundSurfacePreset>();
            curSurface = GroundSurfacePreset.CreateInstance<GroundSurfacePreset>();
            prevSurface = GroundSurfacePreset.CreateInstance<GroundSurfacePreset>();
        }

        void Update() {
            if (wheel == null) { return; }

            // Check for continuous marking
            if (wheel.grounded) {
                if (wheel.surfaceProps != null) {
                    continuousSlide = wheel.surfaceProps.alwaysSlide ? Mathf.Min(0.9f, Mathf.Abs(wheel.rotationRate * 0.01f)) : 0.0f;
                }
                else {
                    continuousSlide = 0.0f;
                }
            }
            else {
                continuousSlide = 0.0f;
            }

            // Create mark
            if (wheel.grounded && (wheel.sliding || continuousSlide > 0)) {
                prevSurface = curSurface;
                curSurface = wheel.grounded ? wheel.surfaceProps : defaultSurface;

                if (!creatingMark) {
                    prevSurface = curSurface;
                    StartMark();
                }
                else if (curSurface != prevSurface) {
                    EndMark();
                }

                // Calculate segment points for mesh
                if (curMark != null) {
                    Vector3 pointDir = Vector3.ProjectOnPlane(Quaternion.AngleAxis(90f, wheel.contactNormal) * tr.forward, wheel.contactNormal).normalized * wheel.width * -0.5f;
                    leftPoint = curMarkTr.InverseTransformPoint(wheel.contactPoint + pointDir * (wheel.flippedSideFactor * Mathf.Sign(wheel.rotationRate) - markOffset * wheel.flippedSideFactor) + wheel.contactNormal * markHeight);
                    rightPoint = curMarkTr.InverseTransformPoint(wheel.contactPoint - pointDir * (wheel.flippedSideFactor * Mathf.Sign(wheel.rotationRate) + markOffset * wheel.flippedSideFactor) + wheel.contactNormal * markHeight);
                }
            }
            else if (creatingMark) {
                EndMark();
                continueMark = false;
            }

            // Update mark if it's short enough, otherwise end it
            if (curEdge < markLength && creatingMark) {
                UpdateMark();
            }
            else if (creatingMark) {
                EndMark();
            }
        }

        // Start creating a tire mark
        void StartMark() {
            creatingMark = true;
            curMark = new GameObject("Tire Mark");
            curMarkTr = curMark.transform;
            curMarkTr.parent = wheel.contactTr;
            curMark.AddComponent<TireMark>();
            MeshRenderer tempRend = curMark.AddComponent<MeshRenderer>();

            // Set renderer material based on ground surface type
            if (curSurface != null) {
                if (curSurface.tireMarkMaterial != null) {
                    tempRend.sharedMaterial = curSurface.tireMarkMaterial;
                }
                else {
                    tempRend.sharedMaterial = defaultMarkMaterial;
                }
            }
            else {
                tempRend.sharedMaterial = defaultMarkMaterial;
            }

            // Set renderer properties
            tempRend.shadowCastingMode = markShadowCastMode;
            tempRend.lightProbeUsage = markLightProbeMode;
            tempRend.reflectionProbeUsage = markReflectionProbeMode;

            // Set up mesh data
            mesh = curMark.AddComponent<MeshFilter>().mesh;
            verts = new Vector3[markLength * 2];
            tris = new int[markLength * 3];

            if (continueMark) {
                verts[0] = leftPointPrev;
                verts[1] = rightPointPrev;

                tris[0] = 0;
                tris[1] = 3;
                tris[2] = 1;
                tris[3] = 0;
                tris[4] = 2;
                tris[5] = 3;
            }

            uvs = new Vector2[verts.Length];
            uvs[0] = new Vector2(0.0f, 0.0f);
            uvs[1] = new Vector2(1.0f, 0.0f);
            uvs[2] = new Vector2(0.0f, 1.0f);
            uvs[3] = new Vector2(1.0f, 1.0f);

            colors = new Color[verts.Length];
            colors[0].a = 0.0f;
            colors[1].a = 0.0f;

            curEdge = 2;
            gapDelay = markGap;
        }

        // Add to a mark that is being created
        void UpdateMark() {
            // Add new vertices to a mark to extend it
            if (gapDelay == 0) {
                float maxSlide = Mathf.Clamp01(Mathf.Abs(F.MaxAbs(wheel.sliding ? 1.0f : 0.0f, continuousSlide)));
                float alpha = (curEdge < markLength - 2 && curEdge > 5 ? 1.0f : 0.0f) * Random.Range(maxSlide * 0.7f, maxSlide);
                gapDelay = markGap;
                curEdge += 2;

                verts[curEdge] = leftPoint;
                verts[curEdge + 1] = rightPoint;

                for (int i = curEdge + 2; i < verts.Length; i++) {
                    verts[i] = Mathf.Approximately(i * 0.5f, Mathf.Round(i * 0.5f)) ? leftPoint : rightPoint;
                    colors[i].a = 0.0f;
                }

                tris[curEdge * 3 - 3] = curEdge;
                tris[curEdge * 3 - 2] = curEdge + 3;
                tris[curEdge * 3 - 1] = curEdge + 1;
                tris[Mathf.Min(curEdge * 3, tris.Length - 1)] = curEdge;
                tris[Mathf.Min(curEdge * 3 + 1, tris.Length - 1)] = curEdge + 2;
                tris[Mathf.Min(curEdge * 3 + 2, tris.Length - 1)] = curEdge + 3;

                uvs[curEdge] = new Vector2(0.0f, curEdge * 0.5f);
                uvs[curEdge + 1] = new Vector2(1.0f, curEdge * 0.5f);

                colors[curEdge] = new Color(1.0f, 1.0f, 1.0f, alpha);
                colors[curEdge + 1] = colors[curEdge];

                mesh.SetVertices(verts);
                mesh.SetTriangles(tris, 0, false);
                mesh.SetUVs(0, uvs);
                mesh.SetColors(colors);
            }
            else // If not adding vertices, reposition last created vertices to keep up with the wheel
            {
                gapDelay = Mathf.Max(0.0f, gapDelay - Time.deltaTime);
                verts[curEdge] = leftPoint;
                verts[curEdge + 1] = rightPoint;

                for (int i = curEdge + 2; i < verts.Length; i++) {
                    verts[i] = Mathf.Approximately(i * 0.5f, Mathf.Round(i * 0.5f)) ? leftPoint : rightPoint;
                    colors[i].a = 0.0f;
                }

                mesh.SetVertices(verts);
            }

            // Renderer recalculations
            mesh.RecalculateBounds();

            if (calculateNormals || calculateTangents) {
                mesh.RecalculateNormals();
            }

            if (calculateTangents) {
                mesh.RecalculateTangents();
            }
        }

        // Stop creating a mark
        void EndMark() {
            creatingMark = false;
            leftPointPrev = verts[Mathf.RoundToInt(verts.Length * 0.5f)];
            rightPointPrev = verts[Mathf.RoundToInt(verts.Length * 0.5f + 1)];
            continueMark = wheel.grounded;

            TireMark mark = curMark.GetComponent<TireMark>();
            mark.lifeTime = lifeTime;
            mark.fadeRate = fadeRate;
            mark.mesh = mesh;
            mark.colors = colors;
            curMark = null;
            curMarkTr = null;
            mesh = null;
        }

        void OnDestroy() {
            if (creatingMark && curMark != null) {
                EndMark();
            }
        }
    }

    // Class for created tire mark instances
    public class TireMark : MonoBehaviour
    {
        [System.NonSerialized]
        public float lifeTime = -1.0f;
        [System.NonSerialized]
        public float fadeRate = 1.0f;
        bool fading = false;
        float alpha = 1.0f;
        [System.NonSerialized]
        public Mesh mesh;
        [System.NonSerialized]
        public Color[] colors;

        // Fade the tire mark and then destroy it
        void Update() {
            if (fading) {
                if (alpha <= 0) {
                    Destroy(gameObject);
                }
                else {
                    alpha -= fadeRate * Time.deltaTime;

                    for (int i = 0; i < colors.Length; i++) {
                        colors[i].a -= fadeRate * Time.deltaTime;
                    }

                    mesh.SetColors(colors);
                }
            }
            else {
                if (lifeTime > 0) {
                    lifeTime = Mathf.Max(0.0f, lifeTime - Time.deltaTime);
                }
                else if (lifeTime == 0) {
                    fading = true;
                }
            }
        }
    }
}