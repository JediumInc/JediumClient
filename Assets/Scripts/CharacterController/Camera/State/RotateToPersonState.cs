using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToPersonState : AbstractStateCamera {

    Vector3 StartPos;
    MyCamera MyCamera;
    float delta;
    public RotateToPersonState(Transform camera, Vector3 target) : base(camera, target)
    {
        MyCamera = camera.GetComponent<MyCamera>();
        StartPos = MyCamera.StartPos;
    }

    public override void OnSee(Vector3 target)
    {
        var targetRotation = Quaternion.LookRotation(target - camera.transform.position, Vector3.up);

        delta += Time.deltaTime;
        camera.transform.rotation = Quaternion.Slerp(camera.transform.rotation, targetRotation, delta);

        camera.transform.position = Vector3.MoveTowards(camera.transform.position, StartPos, Time.deltaTime * 10);

      
       
        if(camera.transform.rotation.eulerAngles==targetRotation.eulerAngles&&Vector3.Distance(camera.transform.position,StartPos)<=0.1f)
        {
            MyCamera.stateCamera = new PersonState(camera, target);
        }

    }
}
