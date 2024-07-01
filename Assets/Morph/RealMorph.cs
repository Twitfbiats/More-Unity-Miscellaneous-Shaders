using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealMorph : MonoBehaviour
{
    public MeshFilter mesh1;
    public MeshFilter mesh2;
    public Vector3[] vertices1;
    public Vector3[] vertices2;
    public int[] triangles1;
    public int[] triangles2;
    // Start is called before the first frame update
    void Start()
    {
        vertices1 = mesh1.mesh.vertices;
        vertices2 = mesh2.mesh.vertices;
        triangles1 = mesh1.mesh.triangles;
        triangles2 = mesh2.mesh.triangles;
        CountTriangleOfBothMesh();
    }

    public void CountTriangleOfBothMesh()
    {
        var count1 = triangles1.Length / 3;
        var count2 = triangles2.Length / 3;
        Debug.Log("Triangle count of mesh1: " + count1);
        Debug.Log("Triangle count of mesh2: " + count2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
