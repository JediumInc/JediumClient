using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectState : AbstractStateCamera {

    Vector3 _offset;
    //  Vector3 startPos;
    float dlina;
    public ObjectState(Transform camera,Vector3 target) : base(camera,target)
    {
     
        _offset = (target-camera.position);
        dlina = _offset.magnitude;
    }

    public override void OnSee(Vector3 target)
    {
        _rotY = Input.GetAxis("Mouse X") * rotSpeed * 3;
        float delta = Input.GetAxis("Mouse Y")/3;
        camera.RotateAround(target, new Vector3(0, 1, 0), _rotY);
        _offset=(target - camera.position);

        Vector3 dist = Vector3.zero;
        if((target - ((target-_offset)+_offset*delta)).magnitude>1&& (target - ((target - _offset) + _offset * delta)).magnitude <= dlina)
        {
            dist = _offset * delta;
        }


        RaycastHit hit;

        if (Physics.Raycast(target, _offset*-1, out hit, _offset.magnitude))
        {
            Vector3 t = hit.point - (target - _offset); 
            dist = t;
        }


        camera.transform.position = (target - _offset) + dist;
  
        camera.transform.LookAt(target);
    }

}
