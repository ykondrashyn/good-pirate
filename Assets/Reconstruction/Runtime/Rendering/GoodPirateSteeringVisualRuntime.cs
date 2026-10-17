using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GoodPirate113.Reconstruction.Rendering
{
    public static class GoodPirateSteeringSkinning
    {
        public static void EnsureTwoBoneBlending(SkinnedMeshRenderer renderer)
        {
            QualitySettings.skinWeights = SkinWeights.TwoBones;
            if (renderer != null)
            {
                renderer.quality = SkinQuality.Bone2;
            }
        }
    }

    public static class GoodPirateSteeringMesh
    {
        public const int DomeArcSegments = 16;
        public const int DomeRadialSegments = 8;

        private const string CorrectedMeshSuffix = " Web Corrected Dome";

        public static Mesh CreateCorrectedTeardrop(Mesh source)
        {
            if (source == null ||
                source.name.EndsWith(CorrectedMeshSuffix,
                    StringComparison.Ordinal) ||
                source.subMeshCount != 1)
            {
                return null;
            }

            var vertices = source.vertices;
            var textureCoordinates = source.uv;
            var boneWeights = source.boneWeights;
            var bindPoses = source.bindposes;
            if (vertices.Length == 0 ||
                textureCoordinates.Length != vertices.Length ||
                boneWeights.Length != vertices.Length ||
                bindPoses.Length < 2)
            {
                return null;
            }

            var center = source.bounds.center;
            var movingEndpoint = bindPoses[1].inverse
                .MultiplyPoint3x4(Vector3.zero);
            var movingDirection = movingEndpoint - center;
            movingDirection.z = 0f;
            if (movingDirection.sqrMagnitude <= Mathf.Epsilon)
            {
                return null;
            }

            var domeDirection = -movingDirection.normalized;
            var perpendicular = new Vector3(-domeDirection.y,
                domeDirection.x, 0f);
            var radius = CalculateRadius(vertices, center, perpendicular);
            if (radius <= Mathf.Epsilon)
            {
                return null;
            }

            var centerVertexCount = DomeArcSegments;
            var ringVertexCount = DomeArcSegments + 1;
            var addedVertexCount = centerVertexCount +
                DomeRadialSegments * ringVertexCount;
            var rebuiltVertices = new Vector3[
                vertices.Length + addedVertexCount];
            var rebuiltTextureCoordinates = new Vector2[
                textureCoordinates.Length + addedVertexCount];
            var rebuiltBoneWeights = new BoneWeight[
                boneWeights.Length + addedVertexCount];
            Array.Copy(vertices, rebuiltVertices, vertices.Length);
            Array.Copy(textureCoordinates, rebuiltTextureCoordinates,
                textureCoordinates.Length);
            Array.Copy(boneWeights, rebuiltBoneWeights, boneWeights.Length);

            var firstAddedVertex = vertices.Length;
            for (var sector = 0; sector < DomeArcSegments; sector++)
            {
                var vertexIndex = firstAddedVertex + sector;
                rebuiltVertices[vertexIndex] = center;
                rebuiltTextureCoordinates[vertexIndex] = new Vector2(
                    DomeTextureU(sector + 1), 1f);
                rebuiltBoneWeights[vertexIndex] = RootBoneWeight();
            }

            for (var ring = 1; ring <= DomeRadialSegments; ring++)
            {
                var radialFraction = ring / (float)DomeRadialSegments;
                var ringRadius = radius * Mathf.Sin(
                    Mathf.PI * 0.5f * radialFraction);
                var textureV = 1f - radialFraction;
                for (var arc = 0; arc <= DomeArcSegments; arc++)
                {
                    var angle = -Mathf.PI * 0.5f +
                        Mathf.PI * arc / DomeArcSegments;
                    var vertexIndex = RingVertexIndex(
                        firstAddedVertex, ring, arc);
                    rebuiltVertices[vertexIndex] = center +
                        domeDirection * (Mathf.Cos(angle) * ringRadius) +
                        perpendicular * (Mathf.Sin(angle) * ringRadius);
                    rebuiltTextureCoordinates[vertexIndex] =
                        new Vector2(DomeTextureU(arc), textureV);
                    rebuiltBoneWeights[vertexIndex] = RootBoneWeight();
                }
            }

            var rebuiltTriangles = new List<int>();
            var sourceTriangles = source.GetTriangles(0);
            var tolerance = radius * 0.0001f;
            for (var triangle = 0;
                 triangle < sourceTriangles.Length;
                 triangle += 3)
            {
                var first = sourceTriangles[triangle];
                var second = sourceTriangles[triangle + 1];
                var third = sourceTriangles[triangle + 2];
                if (IsDomeVertex(vertices[first], center,
                        domeDirection, tolerance) &&
                    IsDomeVertex(vertices[second], center,
                        domeDirection, tolerance) &&
                    IsDomeVertex(vertices[third], center,
                        domeDirection, tolerance))
                {
                    continue;
                }
                rebuiltTriangles.Add(first);
                rebuiltTriangles.Add(second);
                rebuiltTriangles.Add(third);
            }

            for (var sector = 0; sector < DomeArcSegments; sector++)
            {
                rebuiltTriangles.Add(firstAddedVertex + sector);
                rebuiltTriangles.Add(RingVertexIndex(
                    firstAddedVertex, 1, sector));
                rebuiltTriangles.Add(RingVertexIndex(
                    firstAddedVertex, 1, sector + 1));
            }

            for (var ring = 1; ring < DomeRadialSegments; ring++)
            {
                for (var sector = 0; sector < DomeArcSegments; sector++)
                {
                    var innerCurrent = RingVertexIndex(
                        firstAddedVertex, ring, sector);
                    var innerNext = RingVertexIndex(
                        firstAddedVertex, ring, sector + 1);
                    var outerCurrent = RingVertexIndex(
                        firstAddedVertex, ring + 1, sector);
                    var outerNext = RingVertexIndex(
                        firstAddedVertex, ring + 1, sector + 1);
                    rebuiltTriangles.Add(innerCurrent);
                    rebuiltTriangles.Add(outerCurrent);
                    rebuiltTriangles.Add(outerNext);
                    rebuiltTriangles.Add(innerCurrent);
                    rebuiltTriangles.Add(outerNext);
                    rebuiltTriangles.Add(innerNext);
                }
            }

            var rebuilt = UnityEngine.Object.Instantiate(source);
            rebuilt.name = source.name + CorrectedMeshSuffix;
            rebuilt.vertices = rebuiltVertices;
            rebuilt.uv = rebuiltTextureCoordinates;
            rebuilt.boneWeights = rebuiltBoneWeights;
            rebuilt.SetTriangles(rebuiltTriangles, 0, true);
            rebuilt.RecalculateBounds();
            return rebuilt;
        }

        private static float CalculateRadius(Vector3[] vertices,
            Vector3 center, Vector3 perpendicular)
        {
            var radius = 0f;
            foreach (var vertex in vertices)
            {
                radius = Mathf.Max(radius, Mathf.Abs(Vector3.Dot(
                    vertex - center, perpendicular)));
            }
            return radius;
        }

        private static bool IsDomeVertex(Vector3 vertex,
            Vector3 center, Vector3 domeDirection, float tolerance)
        {
            return Vector3.Dot(vertex - center, domeDirection) >=
                -tolerance;
        }

        private static float DomeTextureU(int arc)
        {
            return 0.5f - 0.5f * arc / DomeArcSegments;
        }

        private static int RingVertexIndex(int firstAddedVertex,
            int ring, int arc)
        {
            return firstAddedVertex + DomeArcSegments +
                (ring - 1) * (DomeArcSegments + 1) + arc;
        }

        private static BoneWeight RootBoneWeight()
        {
            return new BoneWeight
            {
                boneIndex0 = 0,
                weight0 = 1f
            };
        }
    }

    [DefaultExecutionOrder(10000)]
    public sealed class GoodPirateSteeringVisualRuntime : MonoBehaviour
    {
        private const string TouchPadTypeName = "TouchPad, Assembly-CSharp";

        private static readonly BindingFlags InstanceFields =
            BindingFlags.Instance | BindingFlags.Public |
            BindingFlags.NonPublic;

        private SkinnedMeshRenderer steeringRenderer;
        private Mesh sourceMesh;
        private Mesh correctedMesh;
        private Bounds sourceBounds;

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void Start()
        {
            Refresh();
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            ClearBinding();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Refresh();
        }

        public void Refresh()
        {
            ClearBinding();
            var type = Type.GetType(TouchPadTypeName, false);
            var touchPad = type == null ? null : FindSceneComponent(type);
            var visualField = type?.GetField("controlUiTrans", InstanceFields);
            var visualRoot = touchPad == null
                ? null
                : visualField?.GetValue(touchPad) as Transform;
            steeringRenderer = visualRoot == null
                ? null
                : visualRoot.GetComponentInChildren<
                    SkinnedMeshRenderer>(true);
            if (steeringRenderer == null)
            {
                return;
            }

            GoodPirateSteeringSkinning.EnsureTwoBoneBlending(
                steeringRenderer);
            sourceMesh = steeringRenderer.sharedMesh;
            correctedMesh = GoodPirateSteeringMesh
                .CreateCorrectedTeardrop(sourceMesh);
            if (correctedMesh == null)
            {
                return;
            }

            sourceBounds = steeringRenderer.localBounds;
            steeringRenderer.sharedMesh = correctedMesh;
            steeringRenderer.localBounds = sourceBounds;
        }

        private void ClearBinding()
        {
            if (steeringRenderer != null &&
                steeringRenderer.sharedMesh == correctedMesh)
            {
                steeringRenderer.sharedMesh = sourceMesh;
                steeringRenderer.localBounds = sourceBounds;
            }
            if (correctedMesh != null)
            {
                Destroy(correctedMesh);
            }
            steeringRenderer = null;
            sourceMesh = null;
            correctedMesh = null;
            sourceBounds = default;
        }

        private static Component FindSceneComponent(Type type)
        {
            foreach (var candidate in Resources.FindObjectsOfTypeAll(type))
            {
                if (candidate is Component component &&
                    component.gameObject.scene.IsValid())
                {
                    return component;
                }
            }
            return null;
        }
    }
}
