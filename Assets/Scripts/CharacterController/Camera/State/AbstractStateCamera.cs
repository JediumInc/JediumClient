using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractStateCamera  {

    public float rotSpeed = 1.5f;
    protected float _rotY;
    protected float _rotX;

    protected Transform camera;

    public AbstractStateCamera(Transform camera,Vector3 target)
    {
        this.camera = camera;
        _rotY = camera.transform.eulerAngles.y;
        _rotX = camera.transform.eulerAngles.x;
    }
    public virtual void SetOffset(Vector3 offset)
    {
        
    }
    public virtual void OnSee (Transform target) {
		
	}


    public virtual void OnSee(Vector3 target)
    {

    }

}
