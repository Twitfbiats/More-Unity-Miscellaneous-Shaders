using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MyMeshStructure : MonoBehaviour
{
    public MeshFilter meshFilter;
    public List<Vertex> vertices;
    public List<Triangle> triangles;
    // Start is called before the first frame update
    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        GenerateMeshStructure();
    }

    public void GenerateMeshStructure()
    {
        vertices = new List<Vertex>();
        triangles = new List<Triangle>();
        for (int i = 0; i < meshFilter.mesh.vertices.Length; i++)
        {
            Vertex vertex = new Vertex();
            vertex.position = meshFilter.mesh.vertices[i];
            vertex.normal = meshFilter.mesh.normals[i];
            vertex.uv = meshFilter.mesh.uv[i];
            vertices.Add(vertex);
        }

        for (int i = 0; i < meshFilter.mesh.triangles.Length; i += 3)
        {
            Triangle triangle = new Triangle();
            triangle.vertices = new List<Vertex>();
            for (int j = 0; j < 3; j++)
            {
                triangle.vertices.Add(vertices[meshFilter.mesh.triangles[i + j]]);
                vertices[meshFilter.mesh.triangles[i + j]].triangles.Add(triangle);
            }
            triangles.Add(triangle);
        }
    }

    public void VertexMerging(int loop)
    {
        List<Vertex> finalVertices = new List<Vertex>();
        List<Triangle> finalTriangles = new List<Triangle>();
        vertices.ForEach(vertex => finalVertices.Add(vertex));
        triangles.ForEach(triangle => finalTriangles.Add(triangle));

        Debug.Log("Current Face Count: " + finalTriangles.Count);

        int loopCount = 0;
        int tempIndex;
        while (loopCount++ < loop)
        {
            Debug.Log("iteration " + (loopCount - 1));
            Vertex vcore = finalVertices[Random.Range(0, finalVertices.Count)];
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
                    if (!finalTriangles.Remove(unhandledTriangles[i])) Debug.Log("Final triangles remove unhandled triangles error");
                    if (!unhandledTriangles.Remove(unhandledTriangles[i--])) Debug.Log("Unhandled triangle remove unhandled triangle error");
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
                tempIndex = triangle.vertices.IndexOf(vcore);
                if (tempIndex == -1)
                {
                    triangle.vertices.ForEach(vertex => Debug.Log("All vertices: " + vertex.ToString()));
                    Debug.Log("vcore: " + vcore.ToString());
                    return;
                }
                triangle.vertices[triangle.vertices.IndexOf(vcore)] = ncore;
                ncore.triangles.Add(triangle);
            });
            vcore.triangles = new List<Triangle>();
            removedTriangles.ForEach(triangle => triangle.vertices.ForEach(vertex => {if (vertex.triangles.Count == 0) finalVertices.Remove(vertex);}));

            Debug.Log("Vertex Count: " + vertices.Count + "-----Final Vertex Count: " + finalVertices.Count);
            Debug.Log("Triangle Count: " + triangles.Count + "-----Final Triangle Count: " + finalTriangles.Count);
        }

        Vector3[] finalVerticesForMesh = new Vector3[finalVertices.Count];
        Vector3[] finalNormalsForMesh = new Vector3[finalVertices.Count];
        Vector2[] finalUVsForMesh = new Vector2[finalVertices.Count];
        int[] finalTrianglesForMesh = new int[finalTriangles.Count * 3];
        for (int i=0;i<finalVertices.Count;i++)
        {
            finalVerticesForMesh[i] = finalVertices[i].position;
            finalNormalsForMesh[i] = finalVertices[i].normal;
            finalUVsForMesh[i] = finalVertices[i].uv;
        }
        for (int i=0;i<finalTriangles.Count;i++)
        {
            finalTrianglesForMesh[i * 3] = finalVertices.IndexOf(finalTriangles[i].vertices[0]);
            finalTrianglesForMesh[i * 3 + 1] = finalVertices.IndexOf(finalTriangles[i].vertices[1]);
            finalTrianglesForMesh[i * 3 + 2] = finalVertices.IndexOf(finalTriangles[i].vertices[2]);
        }

        // checking for possible errors
        for (int i=0;i<finalTrianglesForMesh.Length;i++)
        {
            if (finalTrianglesForMesh[i] < 0 || finalTrianglesForMesh[i] >= finalVertices.Count)
            {
                Debug.LogError("Error in finalTrianglesForMesh: " + i + "-----" + finalTrianglesForMesh[i]);
                return;
            }
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
