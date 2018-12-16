using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR.MagicLeap;

public class Meshing : MonoBehaviour {

    public Material material;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        UpdateMeshMaterial();
    }


    public void UpdateMeshMaterial()
    {
        // Loop over all the child mesh nodes created by MLSpatialMapper script
        for (int i = 0; i < transform.childCount; i++)
        {
            // Get the child gameObject
            GameObject gameObject = transform.GetChild(i).gameObject;
            // Get the meshRenderer component
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
            meshRenderer.material = material;
        }
    }
}
