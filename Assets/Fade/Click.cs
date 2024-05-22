using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Click : MonoBehaviour
{
    public Material goUpMat;
    public Material defaultMat;
    public SkinnedMeshRenderer meshRenderer;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //check if we click the right mouse 
        if (Input.GetMouseButton(1))
        {
            meshRenderer.material = this.defaultMat;
        }

        if (Input.GetMouseButton(0))
        {
            this.goUpMat.SetFloat("_StartTime", Time.timeSinceLevelLoad);
            meshRenderer.material = this.goUpMat;
        }
    }

}
