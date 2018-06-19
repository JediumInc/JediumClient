using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCrate : MonoBehaviour
{
    private Animator _crate;

    private bool open = false;
	// Use this for initialization
	void Start ()
	{
	    _crate = GetComponent<Animator>();
	}

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            open = !open;
            _crate.SetBool("OpenCrate",open);
        }
    }
}
