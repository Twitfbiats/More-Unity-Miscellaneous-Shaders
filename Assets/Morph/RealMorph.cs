using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealMorph : MonoBehaviour
{
    public MeshFilter mesh1;
    public MeshFilter mesh2;
    public Vector3[] vertices1;
    public Vector3[] vertices2;
    public Vector3[] vertices2AfterScalingAndRotating;
    public float scalingFactor = 90f;
    public Vector3 rotationFactor = new Vector3(0, 0, 0);
    public Vector2[] uv2;
    public int[] triangles1;
    public int[] triangles2;
    public ComputeBuffer computeBuffer;
    public Material material;
    // Start is called before the first frame update
    void Awake()
    {
        vertices1 = mesh1.mesh.vertices;
        vertices2 = mesh2.mesh.vertices;
        triangles1 = mesh1.mesh.triangles;
        triangles2 = mesh2.mesh.triangles;
        uv2 = mesh2.mesh.uv;
        CountTriangleOfBothMesh();
    }

    void Start()
    {
        ScaleVertices2(scalingFactor);
        StoreDataForEachTriangle();
    }

    public void ScaleVertices2(float scale)
    {
        vertices2AfterScalingAndRotating = new Vector3[vertices2.Length];
        for (int i = 0; i < vertices2.Length; i++)
        {
            vertices2AfterScalingAndRotating[i] = Matrix4x4.Scale(new Vector3(scale, scale, scale)).MultiplyPoint3x4(vertices2[i]);
            vertices2AfterScalingAndRotating[i] = Matrix4x4.Rotate(Quaternion.Euler(rotationFactor)).MultiplyPoint3x4(vertices2AfterScalingAndRotating[i]);
        }
    }

    public void CountTriangleOfBothMesh()
    {
        var count1 = triangles1.Length / 3;
        var count2 = triangles2.Length / 3;
        Debug.Log("Triangle count of mesh1: " + count1);
        Debug.Log("Triangle count of mesh2: " + count2);
    }

    public List<PerTriangleData> perTriangleDatas = new List<PerTriangleData>();
    public void StoreDataForEachTriangle()
    {
        for (int i=0;i<triangles1.Length;i+=3)
        {
            perTriangleDatas.Add
            (
                new PerTriangleData
                (
                    vertices2AfterScalingAndRotating[triangles2[i]],
                    vertices2AfterScalingAndRotating[triangles2[i+1]],
                    vertices2AfterScalingAndRotating[triangles2[i+2]],
                    uv2[triangles2[i]],
                    uv2[triangles2[i+1]],
                    uv2[triangles2[i+2]]
                )
            );
        }

        computeBuffer = new ComputeBuffer(perTriangleDatas.Count, 3 * sizeof(float) * 3 + 2 * sizeof(float) * 3);
        computeBuffer.SetData(perTriangleDatas.ToArray());

        material.SetBuffer("_PerTriangleData", computeBuffer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public struct PerTriangleData
{
    public Vector3 v0;
    public Vector3 v1;
    public Vector3 v2;
    public Vector2 uv0;
    public Vector2 uv1;
    public Vector2 uv2;

    public PerTriangleData(Vector3 v0, Vector3 v1, Vector3 v2, Vector2 uv0, Vector2 uv1, Vector2 uv2)
    {
        this.v0 = v0;
        this.v1 = v1;
        this.v2 = v2;
        this.uv0 = uv0;
        this.uv1 = uv1;
        this.uv2 = uv2;
    }
}
