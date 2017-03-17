using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollField : MonoBehaviour
{

    public float Parallax = 2f;
	// Update is called once per frame
	void Update ()
	{
	    var mr = GetComponent<MeshRenderer>();
	    var mat = mr.material;

	    var offset = mat.mainTextureOffset;
	    offset.y += Time.deltaTime / 10 / Parallax;

	    mat.mainTextureOffset = offset;
	}
}
