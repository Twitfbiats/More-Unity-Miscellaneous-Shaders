using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MyMeshStructure : MonoBehaviour
{
    public MeshFilter meshFilter;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public bool meshFilterBool = false;
    public bool skinnedMeshRendererBool = false;
    public Vector3[] basePositions;
    public Vector3[] baseNormals;
    public Vector2[] baseUVs;
    public int[] baseTriangles;
    public Vector3[] decimatedPositions;
    public Vector3[] decimatedNormals;
    public Vector2[] decimatedUVs;
    public int[] decimatedTriangles;
    public List<Vertex> vertices;
    public List<Triangle> triangles;
    // Start is called before the first frame update
    void Awake()
    {
        if (TryGetComponent<MeshFilter>(out meshFilter))
        {
            meshFilterBool = true;
            basePositions = meshFilter.mesh.vertices;
            baseNormals = meshFilter.mesh.normals;
            baseUVs = meshFilter.mesh.uv;
            baseTriangles = meshFilter.mesh.triangles;
        }
        else if (TryGetComponent<SkinnedMeshRenderer>(out skinnedMeshRenderer))
        {
            skinnedMeshRendererBool = true;
            basePositions = skinnedMeshRenderer.sharedMesh.vertices;
            baseNormals = skinnedMeshRenderer.sharedMesh.normals;
            baseUVs = skinnedMeshRenderer.sharedMesh.uv;
            baseTriangles = skinnedMeshRenderer.sharedMesh.triangles;
        }
        else
        {
            Debug.LogError("No MeshFilter or SkinnedMeshRenderer found on the GameObject");
        }
    }

    public void ResetMesh()
    {
        if (meshFilterBool)
        {
            meshFilter.mesh.Clear(false);
            meshFilter.mesh.vertices = basePositions;
            meshFilter.mesh.normals = baseNormals;
            meshFilter.mesh.uv = baseUVs;
            meshFilter.mesh.triangles = baseTriangles;
        }
        else if (skinnedMeshRendererBool)
        {
            skinnedMeshRenderer.sharedMesh.Clear(false);
            skinnedMeshRenderer.sharedMesh.vertices = basePositions;
            skinnedMeshRenderer.sharedMesh.normals = baseNormals;
            skinnedMeshRenderer.sharedMesh.uv = baseUVs;
            skinnedMeshRenderer.sharedMesh.triangles = baseTriangles;
        }
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

    public void VertexMerging(int faceCount)
    {
        GenerateMeshStructure();
        Debug.Log("Current Face Count: " + triangles.Count);

        Vertex vcore, ncore;
        List<Triangle> unhandledTriangles, removedTriangles;
        Triangle tempTriangle;
        while (true)
        {
            if (triangles.Count <= faceCount + 1)
            {
                if (triangles.Count == faceCount + 1)
                {
                    vcore = vertices[Random.Range(0, vertices.Count)];
                    tempTriangle = vcore.triangles[0];

                    Vertex n1 = null, n2 = null;
                    for (int i=0;i<3;i++) if (tempTriangle.vertices[i] != vcore)
                    {
                        if (n1 == null) n1 = tempTriangle.vertices[i];
                        else n2 = tempTriangle.vertices[i];
                    }

                    vcore.position = (n1.position + n2.position) / 2;
                    vcore.normal = (n1.normal + n2.normal) / 2;
                    vcore.uv = (n1.uv + n2.uv) / 2;
                    tempTriangle.vertices.ForEach(vertex => vertex.triangles.Remove(tempTriangle));
                    triangles.Remove(tempTriangle);

                    tempTriangle.vertices.ForEach(vertex => 
                    {
                        if (vertex.triangles.Count == 0) vertices.Remove(vertex);
                    });
                } else break;
            }
            else
            {
                vcore = vertices[Random.Range(0, vertices.Count)];
                unhandledTriangles = vcore.triangles;

                tempTriangle = unhandledTriangles[0];
                ncore = null;
                for (int i=0;i<3;i++) if (tempTriangle.vertices[i] != vcore)
                {
                    ncore = tempTriangle.vertices[i];
                    break;
                }

                removedTriangles = new List<Triangle>();
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
            }
        }
        Debug.Log("Vertex Count: " + vertices.Count + "-----Final Vertex Count: " + vertices.Count);
        Debug.Log("Triangle Count: " + triangles.Count + "-----Final Triangle Count: " + triangles.Count);

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

        if (meshFilterBool)
        {
            decimatedPositions = finalVerticesForMesh;
            decimatedNormals = finalNormalsForMesh;
            decimatedUVs = finalUVsForMesh;
            decimatedTriangles = finalTrianglesForMesh;
        }
        else if (skinnedMeshRendererBool)
        {
            skinnedMeshRenderer.sharedMesh.Clear(false);
            skinnedMeshRenderer.sharedMesh.vertices = finalVerticesForMesh;
            skinnedMeshRenderer.sharedMesh.normals = finalNormalsForMesh;
            skinnedMeshRenderer.sharedMesh.uv = finalUVsForMesh;
            skinnedMeshRenderer.sharedMesh.triangles = finalTrianglesForMesh;
        }
    }

    public void SwapMesh()
    {
        if (meshFilterBool)
        {
            meshFilter.mesh.Clear(false);
            meshFilter.mesh.vertices = decimatedPositions;
            meshFilter.mesh.normals = decimatedNormals;
            meshFilter.mesh.uv = decimatedUVs;
            meshFilter.mesh.triangles = decimatedTriangles;
        }
        else if (skinnedMeshRendererBool)
        {
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // public int expectedFaceCount = 0;
    // public int prevExpectedFaceCount = 0;
    // private void FixedUpdate() 
    // {
        
    //     if (expectedFaceCount != prevExpectedFaceCount)
    //     {
    //         VertexMerging(expectedFaceCount);
    //         prevExpectedFaceCount = expectedFaceCount;
    //     }
    // }
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
