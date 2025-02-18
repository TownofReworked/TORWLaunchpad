using Reactor.Utilities.Attributes;
using System.Collections.Generic;
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
        var gameObject = new GameObject("LightChild") { layer = 10 };
        gameObject.AddComponent<MeshFilter>().mesh = myMesh;
        Renderer renderer = gameObject.AddComponent<MeshRenderer>();
        Material = new Material(Shader.Find("Hidden/LightCutaway"));
        renderer.sharedMaterial = Material;
        child = gameObject;
    }

    private void Update()
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
            if (collider2D == null || collider2D.isTrigger) continue;
            var edgeCollider2D = collider2D as EdgeCollider2D;
            if (edgeCollider2D)
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
                var polygonCollider2D = collider2D as PolygonCollider2D;
                if (polygonCollider2D)
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
                    var boxCollider2D = collider2D as BoxCollider2D;
                    if (!boxCollider2D) continue;
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

    // Token: 0x0600077C RID: 1916 RVA: 0x0002BBE8 File Offset: 0x00029DE8
    private void TestBothSides(Vector2 myPos)
    {
        var num = length(del.x, del.x);
        tan.x = -del.y / num * tol;
        tan.y = del.x / num * tol;
        side.x = del.x + tan.x;
        side.y = del.y + tan.y;
        CreateVert(myPos, ref side);
        side.x = del.x - tan.x;
        side.y = del.y - tan.y;
        CreateVert(myPos, ref side);
    }

    private void CreateVert(Vector2 myPos, ref Vector2 del)
    {
        var num = LightRadius * 1.5f;
        var num2 = Physics2D.Raycast(myPos, del, filter, buffer, num);
        if (num2 > 0)
        {
            lightHits.Clear();
            var raycastHit2D = default(RaycastHit2D);
            Collider2D collider2D = null;
            for (var i = 0; i < num2; i++)
            {
                var raycastHit2D2 = buffer[i];
                var collider = raycastHit2D2.collider;
                if (collider == null || collider.isTrigger) continue;

                OneWayShadows oneWayShadows;
                if (OneWayShadows.TryGetValue(collider.gameObject, out oneWayShadows) &&
                    oneWayShadows.RoomCollider.OverlapPoint(transform.position)) continue;
                lightHits.Add(raycastHit2D2);
                raycastHit2D = raycastHit2D2;
                collider2D = collider;
                break;
            }
            for (var j = 0; j < lightHits.Count; j++)
            {
                var raycastHit2D3 = lightHits[j];
                if (raycastHit2D3.collider != null && raycastHit2D3.distance <= LightRadius)
                {
                    if (NoShadows.TryGetValue(raycastHit2D3.collider.gameObject, out NoShadowBehaviour noShadowBehaviour) && noShadowBehaviour != null)
                    {
                        noShadowBehaviour!.didHit = true;
                    }
                }
            }
            if (collider2D && !collider2D!.isTrigger)
            {
                var point = raycastHit2D.point;
                GetEmptyVert().Complete(point.x - myPos.x, point.y - myPos.y);
                return;
            }
        }
        var normalized = del.normalized;
        GetEmptyVert().Complete(normalized.x * num, normalized.y * num);
    }

    // Token: 0x0600077E RID: 1918 RVA: 0x0002BE6C File Offset: 0x0002A06C
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

    // Token: 0x0600077F RID: 1919 RVA: 0x0002BECD File Offset: 0x0002A0CD
    private static float length(float x, float y)
    {
        return Mathf.Sqrt(x * x + y * y);
    }

    // Token: 0x06000780 RID: 1920 RVA: 0x0002BEDC File Offset: 0x0002A0DC
    public static float pseudoAngle(float dx, float dy)
    {
        if (dx < 0f)
        {
            var num = -dx;
            var num2 = (dy > 0f) ? dy : (-dy);
            return 2f - dy / (num + num2);
        }
        var num3 = (dy > 0f) ? dy : (-dy);
        return dy / (dx + num3);
    }

    // Token: 0x040007D7 RID: 2007
    public static Dictionary<GameObject, NoShadowBehaviour> NoShadows = new();

    // Token: 0x040007D8 RID: 2008
    public static Dictionary<GameObject, OneWayShadows> OneWayShadows = new();

    // Token: 0x040007D9 RID: 2009
    private GameObject child;

    // Token: 0x040007DA RID: 2010
    private Vector2[] requiredDels;

    // Token: 0x040007DB RID: 2011
    private Mesh myMesh;

    // Token: 0x040007DC RID: 2012
    public int MinRays = 24;

    // Token: 0x040007DD RID: 2013
    public float LightRadius = 3f;

    // Token: 0x040007DE RID: 2014
    public Material Material;

    // Token: 0x040007DF RID: 2015
    private List<VertInfo> verts = new List<VertInfo>(256);

    // Token: 0x040007E0 RID: 2016
    private int vertCount;

    // Token: 0x040007E1 RID: 2017
    private RaycastHit2D[] buffer = new RaycastHit2D[50];

    // Token: 0x040007E2 RID: 2018
    private Collider2D[] hits = new Collider2D[100];

    // Token: 0x040007E3 RID: 2019
    private ContactFilter2D filter;

    // Token: 0x040007E4 RID: 2020
    private Vector3[] vec;

    // Token: 0x040007E5 RID: 2021
    private Vector2[] uvs;

    // Token: 0x040007E6 RID: 2022
    private int[] triangles = new int[1800];

    // Token: 0x040007E7 RID: 2023
    public float tol = 0.05f;

    // Token: 0x040007E8 RID: 2024
    private Vector2 del;

    // Token: 0x040007E9 RID: 2025
    private Vector2 tan;

    // Token: 0x040007EA RID: 2026
    private Vector2 side;

    // Token: 0x040007EB RID: 2027
    private List<RaycastHit2D> lightHits = new List<RaycastHit2D>();
    private static readonly int Radius = Shader.PropertyToID("_LightRadius");

    // Token: 0x020002D1 RID: 721
    private class VertInfo
    {
        // Token: 0x06000FB9 RID: 4025 RVA: 0x00049DD1 File Offset: 0x00047FD1
        internal void Complete(float x, float y)
        {
            Position.x = x;
            Position.y = y;
            Angle = pseudoAngle(y, x);
        }

        // Token: 0x06000FBA RID: 4026 RVA: 0x00049DF8 File Offset: 0x00047FF8
        internal void Complete(Vector2 point)
        {
            Position.x = point.x;
            Position.y = point.y;
            Angle = pseudoAngle(point.y, point.x);
        }

        // Token: 0x0400107C RID: 4220
        public float Angle;

        // Token: 0x0400107D RID: 4221
        public Vector3 Position;
    }

    // Token: 0x020002D2 RID: 722
    private class AngleComparer : IComparer<VertInfo>
    {
        // Token: 0x06000FBC RID: 4028 RVA: 0x00049E33 File Offset: 0x00048033
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

        // Token: 0x0400107E RID: 4222
        public static readonly AngleComparer Instance = new AngleComparer();
    }

    // Token: 0x020002D3 RID: 723
    private class HitDepthComparer : IComparer<RaycastHit2D>
    {
        // Token: 0x06000FBF RID: 4031 RVA: 0x00049E62 File Offset: 0x00048062
        public int Compare(RaycastHit2D x, RaycastHit2D y)
        {
            if (x.fraction <= y.fraction)
            {
                return -1;
            }
            return 1;
        }

        // Token: 0x0400107F RID: 4223
        public static readonly HitDepthComparer Instance = new HitDepthComparer();
    }
}