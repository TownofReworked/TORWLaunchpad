using Reactor.Utilities.Attributes;
using System.Collections.Generic;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;

namespace LaunchpadReloaded.Components;

[RegisterInIl2Cpp]
public class OgLightSource(nint ptr) : MonoBehaviour(ptr)
{
    private void Start()
    {
        filter.useTriggers = true;
        filter.layerMask = Constants.ShadowMask;
        filter.useLayerMask = true;
        requiredDels = new Vector2[MinRays];
        for (var i = 0; i < requiredDels.Length; i++)
        {
            requiredDels[i] = Vector2.left.Rotate(i / (float)requiredDels.Length * 360f);
        }
        myMesh = new Mesh();
        myMesh.MarkDynamic();
        myMesh.name = "ShadowMesh";
        var gObject = new GameObject("LightChild") { layer = 10 };
        gObject.AddComponent<MeshFilter>().mesh = myMesh;
        Renderer renderer = gObject.AddComponent<MeshRenderer>();
        Material = new Material(Shader.Find("Hidden/LightCutaway"));
        renderer.sharedMaterial = Material;
        child = gObject;
        CalculateLight();
    }

    private void CalculateLight()
    {
        vertCount = 0;
        var position = transform.position;
        position.z -= 7f;
        child.transform.position = position;
        Vector2 vector = position;
        Material.SetFloat(Radius, LightRadius);
        var num = Physics2D.OverlapCircleNonAlloc(vector, LightRadius, hits, Constants.ShadowMask);
        for (var i = 0; i < num; i++)
        {
            var collider2D = hits[i];
            if (collider2D.isTrigger) continue;
            var edgeCollider2D = collider2D.TryCast<EdgeCollider2D>();
            if (edgeCollider2D != null)
            {
                Vector2[] points = edgeCollider2D.points;
                foreach (var t in points)
                {
                    Vector2 vector2 = edgeCollider2D.transform.TransformPoint(t);
                    del.x = vector2.x - vector.x;
                    del.y = vector2.y - vector.y;
                    TestBothSides(vector);
                }
            }
            else
            {
                var polygonCollider2D = collider2D.TryCast<PolygonCollider2D>();
                if (polygonCollider2D != null)
                {
                    Vector2[] points2 = polygonCollider2D.points;
                    foreach (var t in points2)
                    {
                        Vector2 vector3 = polygonCollider2D.transform.TransformPoint(t);
                        del.x = vector3.x - vector.x;
                        del.y = vector3.y - vector.y;
                        TestBothSides(vector);
                    }
                }
                else
                {
                    var boxCollider2D = collider2D.TryCast<BoxCollider2D>();
                    if (boxCollider2D == null)
                    {
                        continue;
                    }

                    var vector4 = boxCollider2D.size / 2f;
                    Vector2 vector5 = boxCollider2D.transform.TransformPoint(boxCollider2D.offset - vector4) - (Vector3)vector;
                    Vector2 vector6 = boxCollider2D.transform.TransformPoint(boxCollider2D.offset + vector4) - (Vector3)vector;
                    del.x = vector5.x;
                    del.y = vector5.y;
                    TestBothSides(vector);
                    del.x = vector6.x;
                    TestBothSides(vector);
                    del.y = vector6.y;
                    TestBothSides(vector);
                    del.x = vector5.x;
                    TestBothSides(vector);
                }
            }
        }
        var num2 = LightRadius * 1.05f;
        foreach (var t in requiredDels)
        {
            var vector7 = num2 * t;
            CreateVert(vector, ref vector7);
        }
        verts.Sort(0, vertCount, AngleComparer.Instance);
        myMesh.Clear();
        if (vec == null || vec.Length < vertCount + 1)
        {
            vec = new Vector3[vertCount + 1];
            uvs = new Vector2[vec.Length];
        }
        vec[0] = Vector3.zero;
        uvs[0] = new Vector2(vec[0].x, vec[0].y);
        for (var m = 0; m < vertCount; m++)
        {
            var num3 = m + 1;
            vec[num3] = verts[m].Position;
            uvs[num3] = new Vector2(vec[num3].x, vec[num3].y);
        }
        var num4 = vertCount * 3;
        if (num4 > triangles.Length)
        {
            triangles = new int[num4];
            Debug.LogWarning("Resized triangles to: " + num4);
        }
        var num5 = 0;
        for (var n = 0; n < triangles.Length; n += 3)
        {
            if (n < num4)
            {
                triangles[n] = 0;
                triangles[n + 1] = num5 + 1;
                if (n == num4 - 3)
                {
                    triangles[n + 2] = 1;
                }
                else
                {
                    triangles[n + 2] = num5 + 2;
                }
                num5++;
            }
            else
            {
                triangles[n] = 0;
                triangles[n + 1] = 0;
                triangles[n + 2] = 0;
            }
        }
        myMesh.vertices = vec;
        myMesh.uv = uvs;
        myMesh.SetIndices(triangles, 0, 0);
    }

    private void TestBothSides(Vector2 myPos)
    {
        var num = Length(del.x, del.x);
        tan.x = -del.y / num * tol;
        tan.y = del.x / num * tol;
        side.x = del.x + tan.x;
        side.y = del.y + tan.y;
        CreateVert(myPos, ref side);
        side.x = del.x - tan.x;
        side.y = del.y - tan.y;
        CreateVert(myPos, ref side);
    }

    private void CreateVert(Vector2 myPos, ref Vector2 ddel)
    {
        var num = LightRadius * 1.5f;
        var num2 = Physics2D.Raycast(myPos, ddel, filter, buffer, num);
        if (num2 > 0)
        {
            lightHits.Clear();
            var raycastHit2D = default(RaycastHit2D);
            Collider2D? collider2D = null;
            for (var i = 0; i < num2; i++)
            {
                var raycastHit2D2 = buffer[i];
                var collider = raycastHit2D2.collider;
                if (OneWayShadows.TryGetValue(collider.gameObject, out var oneWayShadows) &&
                    oneWayShadows.RoomCollider.OverlapPoint(transform.position)) continue;
                lightHits.Add(raycastHit2D2);
                raycastHit2D = raycastHit2D2;
                collider2D = collider;
                break;
            }
            foreach (var raycastHit2D3 in lightHits)
            {
                if (raycastHit2D3.distance <= LightRadius && NoShadows.TryGetValue(raycastHit2D3.collider.gameObject, out var noShadowBehaviour))
                {
                    noShadowBehaviour.didHit = true;
                }
            }
            if (collider2D && !collider2D!.isTrigger)
            {
                var point = raycastHit2D.point;
                GetEmptyVert().Complete(point.x - myPos.x, point.y - myPos.y);
                return;
            }
        }
        var normalized = ddel.normalized;
        GetEmptyVert().Complete(normalized.x * num, normalized.y * num);
    }

    private VertInfo GetEmptyVert()
    {
        if (vertCount < verts.Count)
        {
            var list = verts;
            var num = vertCount;
            vertCount = num + 1;
            return list[num];
        }
        var vertInfo = new VertInfo();
        verts.Add(vertInfo);
        vertCount = verts.Count;
        return vertInfo;
    }

    private static float Length(float x, float y)
    {
        return Mathf.Sqrt(x * x + y * y);
    }

    public static float PseudoAngle(float dx, float dy)
    {
        if (dx < 0f)
        {
            var num = -dx;
            var num2 = dy > 0f ? dy : -dy;
            return 2f - dy / (num + num2);
        }
        var num3 = dy > 0f ? dy : -dy;
        return dy / (dx + num3);
    }

    public static Dictionary<GameObject, NoShadowBehaviour> NoShadows = new();

    public static Dictionary<GameObject, OneWayShadows> OneWayShadows = new();

    private GameObject child;

    private Vector2[] requiredDels;

    private Mesh myMesh;

    public int MinRays = 24;

    public float LightRadius = 3f;

    public Material Material;

    private List<VertInfo> verts = new(256);

    private int vertCount;

    private Il2CppStructArray<RaycastHit2D> buffer = new RaycastHit2D[50];

    private Il2CppReferenceArray<Collider2D> hits = new Collider2D[100];

    private ContactFilter2D filter;

    private Vector3[]? vec;

    private Vector2[] uvs;

    private int[] triangles = new int[1800];

    public float tol = 0.05f;

    private Vector2 del;

    private Vector2 tan;

    private Vector2 side;

    private List<RaycastHit2D> lightHits = [];

    private static readonly int Radius = Shader.PropertyToID("_LightRadius");

    private class VertInfo
    {
        internal void Complete(float x, float y)
        {
            Position.x = x;
            Position.y = y;
            Angle = PseudoAngle(y, x);
        }

        internal void Complete(Vector2 point)
        {
            Position.x = point.x;
            Position.y = point.y;
            Angle = PseudoAngle(point.y, point.x);
        }

        public float Angle;

        public Vector3 Position;
    }

    private class AngleComparer : IComparer<VertInfo>
    {
        public int Compare(VertInfo x, VertInfo y)
        {
            if (x.Angle > y.Angle)
            {
                return 1;
            }
            if (x.Angle >= y.Angle)
            {
                return 0;
            }
            return -1;
        }

        public static readonly AngleComparer Instance = new();
    }
}