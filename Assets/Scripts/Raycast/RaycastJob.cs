using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct RaycastJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<Vector3> Vertices;
    [ReadOnly] public NativeArray<int> Faces;
    public Ray Ray;
    public NativeArray<bool> HitResults;
    public NativeArray<float> HitDistances;

    public void Execute(int index)
    {
        int faceIndex = index * 3;
        Vector3 v0 = Vertices[Faces[faceIndex]];
        Vector3 v1 = Vertices[Faces[faceIndex + 1]];
        Vector3 v2 = Vertices[Faces[faceIndex + 2]];

        if (RaycastHelper.RayIntersects(Ray.origin, Ray.direction, v0, v1, v2, out float distance))
        {
            HitResults[index] = true;
            HitDistances[index] = distance;
        }
        else
        {
            HitResults[index] = false;
            HitDistances[index] = float.MaxValue;
        }
    }
}
