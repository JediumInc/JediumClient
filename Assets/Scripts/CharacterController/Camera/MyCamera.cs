using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCamera : MonoBehaviour {
    public Transform Person;
    private Vector3 target;

    [HideInInspector]
    public Vector3 StartPos;

    private float rotSpeed = 1.5f;

    bool alt = false;
    
    public AbstractStateCamera stateCamera;

    private bool _characterInitialized = false;

    void Start()
    {
      // target = Person.position;
      // stateCamera = new PersonState(this.transform, target);
      // stateCamera.rotSpeed = rotSpeed;
    }

    public void SetCharacter(Transform person)
    {
        Person = person;
        target = Person.position;
        stateCamera = new PersonState(this.transform, target);
        stateCamera.rotSpeed = rotSpeed;
        _characterInitialized = true;
    }
    void Update()
    {
        if (_characterInitialized)
        {

            if (Input.GetKey(KeyCode.LeftAlt))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (stateCamera is PersonState)
                    {
                        Ray ray = this.gameObject.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit))
                        {
                            target = hit.point;
                            StartPos = this.transform.position;
                            stateCamera = new RotateToObjectState(this.transform, hit.point);
                            stateCamera.SetOffset(target - transform.position);
                            alt = true;

                        }
                    }
                }

            }

            if (Input.GetKeyUp(KeyCode.LeftAlt))
            {
                if (alt)
                {
                    alt = false;
                    target = Person.position;
                    stateCamera = new RotateToPersonState(this.transform, target);
                }
            }

            if (!alt)
            {
                target = Person.position;
            }
        }
    }

    void LateUpdate()
    {
        if (_characterInitialized)
        {
            stateCamera.OnSee(target);
        }
    }


}
