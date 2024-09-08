using UnityEngine;

public class RaycastHelper
{
    public static bool RayIntersects(Vector3 rayOrigin, Vector3 rayDirection, Vector3 v0, Vector3 v1, Vector3 v2, out float distance)
    {
        distance = 0f;
        Vector3 edge1 = v1 - v0;
        Vector3 edge2 = v2 - v0;
        Vector3 h = Vector3.Cross(rayDirection, edge2);
        float a = Vector3.Dot(edge1, h);

        if (a > -0.0001f && a < 0.0001f)
        {
            distance = 0f;
            return false; // Ray is parallel to the triangle.
        }

        float f = 1.0f / a;
        Vector3 s = rayOrigin - v0;
        float u = f * Vector3.Dot(s, h);

        if (u < 0.0f || u > 1.0f)
        {
            return false;
        }

        Vector3 q = Vector3.Cross(s, edge1);
        float v = f * Vector3.Dot(rayDirection, q);

        if (v < 0.0f || u + v > 1.0f)
        {
            return false;
        }

        // At this stage, we can compute t to find out where the intersection point is on the line.
        distance = f * Vector3.Dot(edge2, q);

        return distance > 0.0001f; // Ray intersection
    }
}
