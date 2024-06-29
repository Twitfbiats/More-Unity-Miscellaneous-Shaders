using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Morph : MonoBehaviour
{
    MeshFilter meshFilter;
    public float lineLength = 0.1f;
    public int[] triangles;
    public Vector3[] vertices;
    public Vector3[] verticesTemp;
    public List<ListWrapper> sameVertices = new List<ListWrapper>();
    public List<Vector3> center = new List<Vector3>();
    public List<Vector3> centerNormal = new();

    // Start is called before the first frame update
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();   
        meshFilter.mesh.vertices.CopyTo(vertices, 0);
        meshFilter.mesh.vertices.CopyTo(verticesTemp, 0);
        meshFilter.mesh.triangles.CopyTo(triangles, 0);
        //StartCoroutine(Handle());
        //FindSameVertices();
    }

    public IEnumerator Handle()
    {
        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;

        // for each triangles, draw a line at the center of the triangle
        // with direction of normal
        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            Vector3 v1 = vertices[mesh.triangles[i]];
            Vector3 v2 = vertices[mesh.triangles[i + 1]];
            Vector3 v3 = vertices[mesh.triangles[i + 2]];
            center.Add((v1 + v2 + v3) / 3);
            
            centerNormal.Add(Vector3.Cross(v2 - v1, v3 - v1).normalized);
        }

        while (true)
        {
            for (int i = 0; i < center.Count; i++)
            {
                Debug.DrawLine(center[i], center[i] + centerNormal[i] * lineLength, Color.red);
            }

            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public void FindSameVertices()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            ListWrapper same = new ListWrapper();
            for (int j = 0; j < triangles.Length; j++)
            {
                if (vertices[i] == vertices[triangles[j]])
                {
                    Triangle t = new Triangle();
                    t.index = j/3;
                    t.sameVerticeCoord = vertices[i];
                    t.sameVerticeIndex = i;
                    t.sameVerticeIndexInTriangle = j%3;
                    
                    same.list.Add(t);
                }
            }
            sameVertices.Add(same);
        }
    }

    int currentLevel = 0;
    public List<int> handledIndices = new List<int>();
    public Tree tree = new Tree();

    void CreateTree()
    {
        while (handledIndices.Count < vertices.Length)
        {

        }
    }
    void CreateNodeInTree(int index, int level)
    {
        if (handledIndices.Contains(index))
        {
            return;
        }

    }

    List<Node> BuildNode()
    {
        List<Node> nodes = new List<Node>(vertices.Length);
        for (int i=0;i<vertices.Length;i++)
        {
            Node node = new Node
            {
                vertexIndex = i
            };

            for (int j=0;j<triangles.Length;j+=3)
            {
                if (triangles[j] == i || triangles[j+1] == i || triangles[j+2] == i)
                {
                    for (int k=j;k<j+3;k++)
                    {
                        if (triangles[k] != i && !node.connectedIndices.Contains(triangles[k]))
                        {
                            node.connectedIndices.Add(triangles[k]);
                        }
                    }
                }
            }
            nodes[i] = node;
        }

        nodes.ForEach
        (
            node => 
            {
                node.connectedIndices.ForEach(index => 
                {
                    node.connectedNodes.Add(nodes[index]);
                });
            }
        );

        return nodes;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate() 
    {
        
    }
}

[System.Serializable]
public class ListWrapper
{
    public List<Triangle> list = new List<Triangle>();
}

[System.Serializable]
public class Triangle
{
    public int index;
    public Vector3 sameVerticeCoord;
    public int sameVerticeIndex;
    public int sameVerticeIndexInTriangle;
}

public class Node
{
    public int vertexIndex;
    public List<int> connectedIndices = new List<int>();
    public List<Node> connectedNodes = new List<Node>();
    public Node parent;
}

public class Tree
{
    public List<Level> levels = new List<Level>();
}

public class Level
{
    public List<Node> nodes = new List<Node>();
}