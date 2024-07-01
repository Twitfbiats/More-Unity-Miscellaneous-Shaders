using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountTriangle : MonoBehaviour
{
    public MeshFilter meshFilter;

    // Start is called before the first frame update
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        CountTriangleForEachVertex();
    }

    public void CountTriangleForEachVertex()
    {
        var mesh = meshFilter.mesh;
        var triangles = mesh.triangles;
        var vertices = mesh.vertices;

        var vertexToTriangleCount = new Dictionary<Vector3, int>();

        for (int i=0;i<triangles.Length;i+=3)
        {
            var v0 = vertices[triangles[i]];
            var v1 = vertices[triangles[i+1]];
            var v2 = vertices[triangles[i+2]];

            if (vertexToTriangleCount.ContainsKey(v0))
            {
                vertexToTriangleCount[v0]++;
            }
            else
            {
                vertexToTriangleCount[v0] = 1;
            }

            if (vertexToTriangleCount.ContainsKey(v1))
            {
                vertexToTriangleCount[v1]++;
            }
            else
            {
                vertexToTriangleCount[v1] = 1;
            }

            if (vertexToTriangleCount.ContainsKey(v2))
            {
                vertexToTriangleCount[v2]++;
            }
            else
            {
                vertexToTriangleCount[v2] = 1;
            }
        }

        int max = 0;
        foreach (var pair in vertexToTriangleCount)
        {
            if (pair.Value > max)
            {
                max = pair.Value;
            }
        }
        Debug.Log("Max triangle count for a vertex: " + max);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
