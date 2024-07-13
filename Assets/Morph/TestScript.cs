
using System.Collections;
using UnityEngine;

class TestScript : MonoBehaviour
{
    public bool morph = false;
    public bool resetMorph = false;
    public RealMorph realMorph;
    public MyMeshStructure myMeshStructure;
    public GameObject crossBow1;
    public GameObject crossBow2;
    private Rigidbody crossBow1Rigidbody;
    private Rigidbody crossBow2Rigidbody;
    private Vector3 crossBow1InitialPosition;
    private Vector3 crossBow2InitialPosition;
    public GameObject morphToGameObject;

    private void Awake() 
    {
        realMorph = GetComponent<RealMorph>();
        myMeshStructure = GetComponent<MyMeshStructure>();
        crossBow1Rigidbody = crossBow1.GetComponent<Rigidbody>();
        crossBow2Rigidbody = crossBow2.GetComponent<Rigidbody>();
        crossBow1InitialPosition = crossBow1.transform.position;
        crossBow2InitialPosition = crossBow2.transform.position;
    }

    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            realMorph.StartMorphing(morphToGameObject);
            StartCoroutine(TrickSwapMesh());
            crossBow1Rigidbody.useGravity = true;
            crossBow2Rigidbody.useGravity = true;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            realMorph.ResetMorphing();
            crossBow1Rigidbody.useGravity = false;
            crossBow2Rigidbody.useGravity = false;
            crossBow1.transform.position = crossBow1InitialPosition;
            crossBow2.transform.position = crossBow2InitialPosition;
        }
    }

    IEnumerator TrickSwapMesh()
    {
        yield return new WaitForSeconds(2);

        myMeshStructure.MeshFilter.mesh.Clear();
        myMeshStructure.MeshFilter.mesh.vertices = realMorph.Vertices2AfterScalingAndRotating;
        myMeshStructure.MeshFilter.mesh.uv = realMorph.Uv2;
        myMeshStructure.MeshFilter.mesh.triangles = realMorph.Triangles2;
        myMeshStructure.MeshFilter.mesh.RecalculateNormals();
        realMorph.meshRenderer.material = realMorph.meshRenderer2.material;
    }
}