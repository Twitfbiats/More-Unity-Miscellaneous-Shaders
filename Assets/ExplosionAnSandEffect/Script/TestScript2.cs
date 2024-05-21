using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript2 : MonoBehaviour
{

    public Material explosionMat;
    public Material defaultMat;
    public Material goUpMat;
    public bool dedfaultMatBool = false;
    public bool goUpMatBool = false;

    private bool isClicked;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //check if we click the right mouse 
        if (Input.GetMouseButton(1))
        {
            MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();

            for(int i = 0; i< renderers.Length; i++)
            {
                renderers[i].material = this.defaultMat;
            }
        }

        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                MeshRenderer[] renderers = hit.collider.GetComponentsInChildren<MeshRenderer>();
                if (goUpMatBool)
                {
                    this.goUpMat.SetFloat("_StartTime", Time.timeSinceLevelLoad);
                    for(int i = 0; i< renderers.Length; i++)
                    {
                        renderers[i].material = this.goUpMat;
                    }
                }
                else 
                {
                    this.explosionMat.SetFloat("_StartTime", Time.timeSinceLevelLoad);
                    for(int i = 0; i< renderers.Length; i++)
                    {
                        renderers[i].material = this.explosionMat;
                    }
                }
            }
        }
    }

}
