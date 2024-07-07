using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyMeshStructure : MonoBehaviour
{
    public MeshFilter meshFilter;
    public List<Vertex> vertices;
    public List<Triangle> triangles;
    // Start is called before the first frame update
    void Start()
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
            }
            triangles.Add(triangle);
        }
    }

    public void Reduce2Face(int loop)
    {
        List<Vertex> finalVertices = new List<Vertex>();
        List<Triangle> finalTriangles = new List<Triangle>();
        vertices.ForEach(vertex => finalVertices.Add(vertex));
        triangles.ForEach(triangle => finalTriangles.Add(triangle));

        int expectedFaceCount = finalTriangles.Count - 2 * loop;
        Vertex tempVertex;
        while (finalTriangles.Count != expectedFaceCount)
        {
            List<Vertex> unhandledVertices = new List<Vertex>();
            Vertex vertex = finalVertices[Random.Range(0, finalVertices.Count)];
            vertex.triangles.ForEach(triangle => 
            {
                triangle.vertices.ForEach(v => 
                {
                    if (!unhandledVertices.Contains(v) && v != vertex)
                    {
                        unhandledVertices.Add(v);
                        v.triangles.Remove(triangle);
                    }
                });
            });
            finalVertices.Remove(vertex);
            vertex.triangles.ForEach(triangle => finalTriangles.Remove(triangle));

            List<bool> marks = new List<bool>(unhandledVertices.Count);
            marks.ForEach(mark => mark = false);

            int expectedFaceRemoved = vertex.triangles.Count - 2;
            int totalHandleFace = 0;

            while (totalHandleFace != expectedFaceRemoved)
            {
                int pickedIndex = Random.Range(0, unhandledVertices.Count);
                while (marks[pickedIndex])
                {
                    pickedIndex = Random.Range(0, unhandledVertices.Count);
                }
                Vertex pickedOne = unhandledVertices[pickedIndex]; marks[pickedIndex] = true;
                Triangle newTriangle = new Triangle() {vertices = new List<Vertex>()}; 
                newTriangle.vertices.Add(vertex);

                for (int i=0;i<pickedOne.triangles.Count;i++)
                {
                    pickedOne.triangles[i].vertices.ForEach(v => 
                    {
                        if (unhandledVertices.Contains(v) && !marks[unhandledVertices.IndexOf(v)])
                        {
                            newTriangle.vertices.Add(v);
                        }
                    });

                    if (newTriangle.vertices.Count == 3) break;
                }
                newTriangle.vertices.ForEach(v => v.triangles.Add(newTriangle));
                finalTriangles.Add(newTriangle);
                totalHandleFace++;
            }
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

        meshFilter.mesh.Clear();
        meshFilter.mesh.vertices = finalVerticesForMesh;
        meshFilter.mesh.normals = finalNormalsForMesh;
        meshFilter.mesh.uv = finalUVsForMesh;
        meshFilter.mesh.triangles = finalTrianglesForMesh;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int faceReductionCount = 0;
    public int prevFaceReductionCount = 0;
    private void FixedUpdate() 
    {
        if (faceReductionCount != prevFaceReductionCount)
        {
            Reduce2Face(faceReductionCount / 2);
            prevFaceReductionCount = faceReductionCount;
        }
    }
}

public class Vertex
{
    public Vector3 position;
    public Vector3 normal;
    public Vector2 uv;
    public List<Triangle> triangles;
}

public class Triangle
{
    public List<Vertex> vertices;
}
