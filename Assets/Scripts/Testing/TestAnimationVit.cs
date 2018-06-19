using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAnimationVit : MonoBehaviour
{

    public Animator _Animator;
    private bool testbool = false;
	
	// Update is called once per frame
	void Update () {
		//_Animator.SetFloat("Umnijitel",Time.deltaTime*10);
	   // _Animator.SetFloat("Speed", Time.deltaTime*10);

//        Input.GetMouseButtonUp(0)
	    if (Input.GetMouseButtonUp(0))
	    {
            Debug.Log("___bool FIRED"+Time.realtimeSinceStartup);
	        testbool = !testbool;
	        _Animator.SetBool("BoolTest",testbool);
	    }

    }
}
