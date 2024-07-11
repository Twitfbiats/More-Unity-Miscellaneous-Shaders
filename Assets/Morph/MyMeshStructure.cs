using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MyMeshStructure : MonoBehaviour
{
    public MeshFilter meshFilter;
    public Vector3[] basePositions;
    public Vector3[] baseNormals;
    public Vector2[] baseUVs;
    public int[] baseTriangles;
    public List<Vertex> vertices;
    public List<Triangle> triangles;
    // Start is called before the first frame update
    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        basePositions = new Vector3[meshFilter.mesh.vertices.Length];
        baseNormals = new Vector3[meshFilter.mesh.normals.Length];
        baseUVs = new Vector2[meshFilter.mesh.uv.Length];
        baseTriangles = new int[meshFilter.mesh.triangles.Length];
        meshFilter.mesh.vertices.CopyTo(basePositions, 0);
        meshFilter.mesh.normals.CopyTo(baseNormals, 0);
        meshFilter.mesh.uv.CopyTo(baseUVs, 0);
        meshFilter.mesh.triangles.CopyTo(baseTriangles, 0);
    }

    public void GenerateMeshStructure()
    {
        vertices = new List<Vertex>();
        triangles = new List<Triangle>();
        for (int i = 0; i < basePositions.Length; i++)
        {
            Vertex vertex = new Vertex();
            vertex.position = basePositions[i];
            vertex.normal = baseNormals[i];
            vertex.uv = baseUVs[i];
            vertices.Add(vertex);
        }

        int ipj;
        for (int i = 0; i < baseTriangles.Length; i += 3)
        {
            Triangle triangle = new Triangle();
            triangle.vertices = new List<Vertex>();
            for (int j = 0; j < 3; j++)
            {
                ipj = i + j;
                triangle.vertices.Add(vertices[baseTriangles[ipj]]);
                vertices[baseTriangles[ipj]].triangles.Add(triangle);
            }
            triangles.Add(triangle);
        }
    }

    public void VertexMerging(int loop)
    {
        GenerateMeshStructure();
        Debug.Log("Current Face Count: " + triangles.Count);

        int loopCount = 0;
        while (loopCount++ < loop)
        {
            Debug.Log("iteration " + (loopCount - 1));
            Vertex vcore = vertices[Random.Range(0, vertices.Count)];
            List<Triangle> unhandledTriangles = vcore.triangles;

            Triangle tempTriangle = unhandledTriangles[0];
            Vertex ncore = null;
            for (int i=0;i<3;i++) if (tempTriangle.vertices[i] != vcore)
            {
                ncore = tempTriangle.vertices[i];
                break;
            }

            List<Triangle> removedTriangles = new List<Triangle>();
            for (int i=0;i<unhandledTriangles.Count;i++)
            {
                if (unhandledTriangles[i].vertices.Contains(ncore))
                {
                    removedTriangles.Add(unhandledTriangles[i]);
                    triangles.Remove(unhandledTriangles[i]);
                    unhandledTriangles.Remove(unhandledTriangles[i--]);
                }
            }
            removedTriangles.ForEach(triangle => 
            {
                triangle.vertices.ForEach(vertex => 
                {
                    vertex.triangles.Remove(triangle);
                });
            });

            unhandledTriangles.ForEach(triangle => 
            {
                triangle.vertices[triangle.vertices.IndexOf(vcore)] = ncore;
                ncore.triangles.Add(triangle);
            });
            vcore.triangles = new List<Triangle>();
            
            removedTriangles.ForEach(triangle => 
                triangle.vertices.ForEach(vertex => 
                {
                    if (vertex.triangles.Count == 0) vertices.Remove(vertex);
                })
            );

            Debug.Log("Vertex Count: " + vertices.Count + "-----Final Vertex Count: " + vertices.Count);
            Debug.Log("Triangle Count: " + triangles.Count + "-----Final Triangle Count: " + triangles.Count);
        }

        Vector3[] finalVerticesForMesh = new Vector3[vertices.Count];
        Vector3[] finalNormalsForMesh = new Vector3[vertices.Count];
        Vector2[] finalUVsForMesh = new Vector2[vertices.Count];
        int[] finalTrianglesForMesh = new int[triangles.Count * 3];
        for (int i=0;i<vertices.Count;i++)
        {
            finalVerticesForMesh[i] = vertices[i].position;
            finalNormalsForMesh[i] = vertices[i].normal;
            finalUVsForMesh[i] = vertices[i].uv;
        }
        for (int i=0;i<triangles.Count;i++)
        {
            finalTrianglesForMesh[i * 3] = vertices.IndexOf(triangles[i].vertices[0]);
            finalTrianglesForMesh[i * 3 + 1] = vertices.IndexOf(triangles[i].vertices[1]);
            finalTrianglesForMesh[i * 3 + 2] = vertices.IndexOf(triangles[i].vertices[2]);
        }

        meshFilter.mesh.Clear(false);
        meshFilter.mesh.vertices = finalVerticesForMesh;
        meshFilter.mesh.normals = finalNormalsForMesh;
        meshFilter.mesh.uv = finalUVsForMesh;
        meshFilter.mesh.triangles = finalTrianglesForMesh;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int loopCount = 0;
    public int prevLoopCount = 0;
    private void FixedUpdate() 
    {
        
        if (loopCount != prevLoopCount)
        {
            VertexMerging(loopCount);
            prevLoopCount = loopCount;
        }
    }
}

public class Vertex
{
    public Vector3 position;
    public Vector3 normal;
    public Vector2 uv;
    public List<Triangle> triangles = new List<Triangle>();

    public String ToString()
    {
        return "Position: " + position.ToString() + "-----Normal: " + normal.ToString() + "-----UV: " + uv.ToString() + "-----Triangle Count: " + triangles.Count;
    }
}

public class Triangle
{
    public List<Vertex> vertices;
}
