using System.Collections.Generic;
using UnityEngine;

public class OcTreeNode
{
    public Bounds Bounds;
    public List<int> TriangleIndices;
    public OcTreeNode[] Children;

    public OcTreeNode(Bounds bounds)
    {
        Bounds = bounds;
        TriangleIndices = new List<int>();
        Children = null;
    }

    public bool IsLeaf()
    {
        return Children == null;
    }

    public void Subdivide()
    {
        Children = new OcTreeNode[8];
        Vector3 size = Bounds.size / 2f;
        Vector3 center = Bounds.center;

        for (int i = 0; i < 8; i++)
        {
            Vector3 newCenter = center + new Vector3(
                size.x * ((i & 1) == 0 ? -0.5f : 0.5f),
                size.y * ((i & 2) == 0 ? -0.5f : 0.5f),
                size.z * ((i & 4) == 0 ? -0.5f : 0.5f)
            );

            Bounds childBounds = new(newCenter, size);
            Children[i] = new OcTreeNode(childBounds);
        }
    }
}

public class OcTree
{
    private readonly OcTreeNode root;
    private readonly int maxDepth;
    private readonly int maxTrianglesPerNode;

    public OcTree(Bounds bounds, int maxDepth, int maxTrianglesPerNode)
    {
        root = new OcTreeNode(bounds);
        this.maxDepth = maxDepth;
        this.maxTrianglesPerNode = maxTrianglesPerNode;
    }

    public void Insert(int triangleIndex, Bounds triangleBounds)
    {
        Insert(triangleIndex, triangleBounds, root, 0);
    }

    private void Insert(int triangleIndex, Bounds triangleBounds, OcTreeNode node, int depth)
    {
        if (depth < maxDepth && node.TriangleIndices.Count >= maxTrianglesPerNode)
        {
            if (node.IsLeaf())
            {
                node.Subdivide();
            }

            foreach (var child in node.Children)
            {
                if (child.Bounds.Intersects(triangleBounds))
                {
                    Insert(triangleIndex, triangleBounds, child, depth + 1);
                    return;
                }
            }
        }

        node.TriangleIndices.Add(triangleIndex);
    }

    public List<int> Query(Bounds queryBounds)
    {
        List<int> results = new();
        Query(queryBounds, root, results);
        return results;
    }

    private void Query(Bounds queryBounds, OcTreeNode node, List<int> results)
    {
        if (!node.Bounds.Intersects(queryBounds))
        {
            return;
        }

        results.AddRange(node.TriangleIndices);

        if (!node.IsLeaf())
        {
            foreach (var child in node.Children)
            {
                Query(queryBounds, child, results);
            }
        }
    }
}
