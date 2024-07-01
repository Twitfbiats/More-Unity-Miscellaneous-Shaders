using System.Collections.Generic;
using UnityEngine;

class PerPrimitive : MonoBehaviour
{
    public Material material;
    public MeshFilter meshFilter;
    public ComputeBuffer computeBuffer;
    public List<TriangleData> triangleDatas = new List<TriangleData>();

    void Start()
    {
        material = GetComponent<Renderer>().material;
        meshFilter = GetComponent<MeshFilter>();
        
        AddTriangleData();
    }

    void AddTriangleData()
    {
        var mesh = meshFilter.mesh;
        var triangles = mesh.triangles;
        var vertices = mesh.vertices;

        for (int i=0;i<triangles.Length;i+=3)
        {
            var triangleData = new TriangleData();
            triangleData.v0 = vertices[triangles[i]];
            triangleData.v1 = vertices[triangles[i+1]];
            triangleData.v2 = vertices[triangles[i+2]];
            triangleDatas.Add(triangleData);
        }

        computeBuffer = new ComputeBuffer(triangleDatas.Count, 3 * sizeof(float) * 3);
        computeBuffer.SetData(triangleDatas.ToArray());

        material.SetBuffer("_TriangleData", computeBuffer);
    }
}

public struct TriangleData
{
    public Vector3 v0;
    public Vector3 v1;
    public Vector3 v2;
}