using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class RayHitSystem : MonoBehaviour
{
    private readonly List<NativeArray<Vector3>> allVertices = new();
    private readonly List<NativeArray<int>> allFaces = new();
    private readonly List<Transform> allTransforms = new(); // Store transforms of spheres
    private OcTree octree;

    private void Start()
    {
        // Initialize Octree with a defined space
        Bounds bounds = new(Vector3.zero, new Vector3(200, 200, 200));
        octree = new OcTree(bounds, 5, 8);

        // Find all MeshFilters in the scene
        MeshFilter[] meshFilters = FindObjectsByType<MeshFilter>(FindObjectsSortMode.InstanceID);

        foreach (var meshFilter in meshFilters)
        {
            Mesh mesh = meshFilter.mesh;
            if (mesh == null) continue; // Skip if there's no mesh

            // Convert mesh data to NativeArrays for Job System use
            NativeArray<Vector3> vertices = new(mesh.vertices, Allocator.Persistent);
            NativeArray<int> faces = new(mesh.triangles, Allocator.Persistent);

            allVertices.Add(vertices);
            allFaces.Add(faces);
            allTransforms.Add(meshFilter.transform); // Store the transform of the current sphere

            // Insert each triangle into the Octree
            for (int j = 0; j < mesh.triangles.Length / 3; j++)
            {
                int faceIndex = j * 3;
                Vector3 v0 = meshFilter.transform.TransformPoint(vertices[faces[faceIndex]]);
                Vector3 v1 = meshFilter.transform.TransformPoint(vertices[faces[faceIndex + 1]]);
                Vector3 v2 = meshFilter.transform.TransformPoint(vertices[faces[faceIndex + 2]]);

                Bounds triangleBounds = new();
                triangleBounds.Encapsulate(v0);
                triangleBounds.Encapsulate(v1);
                triangleBounds.Encapsulate(v2);

                octree.Insert(j, triangleBounds);
            }
        }

        Debug.Log("Rayhit system is ready");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Check ray hits against all sphere meshes
            for (int i = 0; i < allFaces.Count; i++)
            {
                NativeArray<int> faces = allFaces[i];
                NativeArray<Vector3> vertices = allVertices[i];
                Transform transform = allTransforms[i];

                if (RayHitTest(faces, vertices, transform, ray, out float hitDistance))
                {
                    Debug.Log($"Ray hit sphere at distance: {hitDistance}");
                }
            }
        }
    }

    private bool RayHitTest(NativeArray<int> faces, NativeArray<Vector3> vertices, Transform transform, Ray ray, out float distance)
    {
        distance = float.MaxValue;
        bool hit = false;

        for (int i = 0; i < faces.Length; i += 3)
        {
            Vector3 v0 = transform.TransformPoint(vertices[faces[i]]);
            Vector3 v1 = transform.TransformPoint(vertices[faces[i + 1]]);
            Vector3 v2 = transform.TransformPoint(vertices[faces[i + 2]]);

            if (RayIntersectsTriangle(ray, v0, v1, v2, out float dist))
            {
                if (dist < distance)
                {
                    distance = dist;
                    hit = true;
                }
            }
        }

        return hit;
    }

    private bool RayIntersectsTriangle(Ray ray, Vector3 v0, Vector3 v1, Vector3 v2, out float t)
    {
        t = 0;
        Vector3 edge1 = v1 - v0;
        Vector3 edge2 = v2 - v0;
        Vector3 h = Vector3.Cross(ray.direction, edge2);
        float a = Vector3.Dot(edge1, h);

        if (a > -Mathf.Epsilon && a < Mathf.Epsilon)
        {
            return false; // Ray is parallel to the triangle
        }

        float f = 1.0f / a;
        Vector3 s = ray.origin - v0;
        float u = f * Vector3.Dot(s, h);

        if (u < 0.0f || u > 1.0f)
        {
            return false;
        }

        Vector3 q = Vector3.Cross(s, edge1);
        float v = f * Vector3.Dot(ray.direction, q);

        if (v < 0.0f || u + v > 1.0f)
        {
            return false;
        }

        // Compute t to find out where the intersection point is on the line
        t = f * Vector3.Dot(edge2, q);

        if (t > Mathf.Epsilon)
        {
            return true;
        }
        else
        {
            return false; // Line intersection but not a ray intersection
        }
    }

    private void OnDestroy()
    {
        foreach (var vertices in allVertices)
        {
            vertices.Dispose();
        }

        foreach (var faces in allFaces)
        {
            faces.Dispose();
        }
    }
}
