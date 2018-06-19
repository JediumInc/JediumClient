using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonState : AbstractStateCamera {
    protected Vector3 _offset;

    public PersonState(Transform camera, Vector3 target) : base(camera, target)
    {
        this._offset = new Vector3(0f, -2, 3) * -1;
    }
    public override void OnSee(Transform target)
    {
    
      
    }

    public override void OnSee(Vector3 target)
    {
       


        _rotY += (Input.GetAxis("Horizontal") * (rotSpeed));
   
        Vector3 tempPositionBot = new Vector3(target.x,
                                       target.y ,//Здесь определяется точка фокуса камеры
                                       target.z);
        

      Quaternion  newRotation = Quaternion.Euler(0, _rotY, 0f);
        Vector3 off = _offset;
        RaycastHit hit;
      
        if (Physics.Raycast(target,newRotation* _offset, out hit, _offset.magnitude))
        {
            Vector3 t = target - hit.point;
            off = new Vector3(0, t.y*-1, t.z);
        }
        Vector3 newPosition = newRotation * off+ tempPositionBot;


        camera.transform.rotation = newRotation;
        camera.transform.position = Vector3.MoveTowards(camera.transform.position, newPosition, Time.deltaTime * 20);
        camera.LookAt(target);
    }

}
