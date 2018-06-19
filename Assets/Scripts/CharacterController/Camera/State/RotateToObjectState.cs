using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToObjectState : AbstractStateCamera
{
    float delta = 0;
    public RotateToObjectState(Transform camera, Vector3 target) : base(camera, target)
    {

    }

    public override void OnSee(Vector3 target)
    {


        var targetRotation = Quaternion.LookRotation(target - camera.transform.position, Vector3.up);


        delta += Time.deltaTime;
        
        camera.transform.rotation = Quaternion.Slerp(camera.transform.rotation, targetRotation, delta);



        

        if (camera.transform.rotation.eulerAngles == targetRotation.eulerAngles)
        {
            camera.GetComponent<MyCamera>().stateCamera = new ObjectState(camera, target);
        }
    }
}
