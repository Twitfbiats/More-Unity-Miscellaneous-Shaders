using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealMorph : MonoBehaviour
{
    public MeshFilter mesh1;
    public MeshFilter mesh2;
    public MyMeshStructure myMeshStructure;
    public Vector3[] vertices1;
    public Vector3[] vertices2;
    public Vector3[] vertices2AfterScalingAndRotating;
    public Vector2[] uv2;
    public int[] triangles1;
    public int[] triangles2;
    public Vector3[] triangles2MidPoint;
    public bool[] triangles2MidPointHandled;
    public float triangle1TravelDistance = 100f;
    public ComputeBuffer computeBuffer;
    public Material morphMaterial;
    public Material normalMaterial;
    // Start is called before the first frame update
    void Awake()
    {
        myMeshStructure = GetComponent<MyMeshStructure>();
        vertices2 = mesh2.mesh.vertices;
        triangles2 = mesh2.mesh.triangles;
        triangles2MidPoint = new Vector3[triangles2.Length / 3];
        uv2 = mesh2.mesh.uv;
    }

    void Start()
    {

    }

    public void StartMorphing()
    {
        myMeshStructure.ResetMesh();
        vertices1 = mesh1.mesh.vertices;
        triangles1 = mesh1.mesh.triangles;
        triangles2MidPointHandled = new bool[triangles2MidPoint.Length];
        for (int i = 0; i < triangles2MidPointHandled.Length; i++) triangles2MidPointHandled[i] = false;
        if (triangles1.Length > triangles2.Length)
        {
            myMeshStructure.VertexMerging(triangles2.Length / 3);
            vertices1 = mesh1.mesh.vertices;
            triangles1 = mesh1.mesh.triangles;
            Mesh2PositionCorrecting();
            CalculateMidPointOfTriangle2();
            StoreDataForEachTriangle();
            morphMaterial.SetFloat("_Triangle1TravelDistance", triangle1TravelDistance);
            mesh1.GetComponent<MeshRenderer>().material = morphMaterial;
        }
        else return;
    }

    public void CalculateMidPointOfTriangle2()
    {
        for (int i = 0; i < triangles2.Length; i += 3)
        {
            triangles2MidPoint[i / 3] = (vertices2[triangles2[i]] + vertices2[triangles2[i + 1]] + vertices2[triangles2[i + 2]]) / 3;
        }
    }

    public Matrix4x4 ConstructRotationAndScaleMatrixForMesh2()
    {
        Matrix4x4 matrix4X4 = mesh2.transform.localToWorldMatrix;
        matrix4X4.SetColumn(3, new Vector4(0, 0, 0, 1));
        Matrix4x4 mesh1RotationMatrix = Matrix4x4.Rotate(mesh1.transform.localRotation);
        mesh1RotationMatrix = mesh1RotationMatrix.inverse;
        return mesh1RotationMatrix * matrix4X4;
    }

    public void Mesh2PositionCorrecting()
    {
        vertices2AfterScalingAndRotating = new Vector3[vertices2.Length];
        Matrix4x4 matrix4X4 = ConstructRotationAndScaleMatrixForMesh2();
        for (int i = 0; i < vertices2.Length; i++)
        {
            vertices2AfterScalingAndRotating[i] = matrix4X4.MultiplyPoint3x4(vertices2[i]);
        }
    }

    public List<PerTriangleData> perTriangleDatas = new List<PerTriangleData>();
    public void StoreDataForEachTriangle()
    {
        Vector3 tempNormal, tempMidPointPosition, tempDirection;
        float tempMinDistance, currentDistance;
        int markedMinPosition = 0;
        for (int i=0;i<triangles1.Length;i+=3)
        {
            tempNormal = Vector3.Cross(vertices1[triangles1[i + 1]] - vertices1[triangles1[i]], vertices1[triangles1[i + 2]] - vertices1[triangles1[i]]);
            tempMidPointPosition = 
            (
                (vertices1[triangles1[i]] + vertices1[triangles1[i + 1]] + vertices1[triangles1[i + 2]]) / 3
                + tempNormal.normalized * triangle1TravelDistance
            );
            tempMinDistance= float.MaxValue;

            for (int j=0;j<triangles2MidPoint.Length;j++)
            {
                if (!triangles2MidPointHandled[j])
                {
                    currentDistance = Vector3.Distance(tempMidPointPosition, triangles2MidPoint[j]);
                    if (currentDistance < tempMinDistance)
                    {
                        tempMinDistance = currentDistance;
                        markedMinPosition = j;
                    }
                }
            }
            triangles2MidPointHandled[markedMinPosition] = true;

            markedMinPosition *= 3;
            tempDirection = tempNormal * triangle1TravelDistance;
            perTriangleDatas.Add
            (
                new PerTriangleData
                (
                    vertices2AfterScalingAndRotating[triangles2[markedMinPosition]],
                    vertices2AfterScalingAndRotating[triangles2[markedMinPosition + 1]],
                    vertices2AfterScalingAndRotating[triangles2[markedMinPosition + 2]],
                    uv2[triangles2[markedMinPosition]],
                    uv2[triangles2[markedMinPosition + 1]],
                    uv2[triangles2[markedMinPosition + 2]],
                    tempDirection,
                    vertices1[triangles1[i]] + tempDirection,
                    vertices1[triangles1[i + 1]] + tempDirection,
                    vertices1[triangles1[i + 2]] + tempDirection
                )
            );
        }

        computeBuffer = new ComputeBuffer(perTriangleDatas.Count, 3 * sizeof(float) * 7 + 2 * sizeof(float) * 3);
        computeBuffer.SetData(perTriangleDatas.ToArray());

        morphMaterial.SetBuffer("_PerTriangleData", computeBuffer);
    }

    public bool morph = false;
    // Update is called once per frame
    void Update()
    {
        if (morph) {morph = false; StartMorphing();}
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
    public Vector3 triangle1TravelDirection;
    public Vector3 v0MaxPos;
    public Vector3 v1MaxPos;
    public Vector3 v2MaxPos;

    public PerTriangleData(Vector3 v0, Vector3 v1, Vector3 v2, Vector2 uv0, Vector2 uv1, Vector2 uv2, Vector3 triangle1TravelDirection, Vector3 v0MaxPos, Vector3 v1MaxPos, Vector3 v2MaxPos)
    {
        this.v0 = v0;
        this.v1 = v1;
        this.v2 = v2;
        this.uv0 = uv0;
        this.uv1 = uv1;
        this.uv2 = uv2;
        this.triangle1TravelDirection = triangle1TravelDirection;
        this.v0MaxPos = v0MaxPos;
        this.v1MaxPos = v1MaxPos;
        this.v2MaxPos = v2MaxPos;
    }
}
