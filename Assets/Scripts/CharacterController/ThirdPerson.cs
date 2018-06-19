using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CharacterController))]
public class ThirdPerson : MonoBehaviour {

    public MyCamera Camera;
    public float rotSpeed = 15.0f;
    public float moveSpeed = 6.0f;
    private CharacterController _charController;


    void Awake()
    {
        Camera = UnityEngine.Camera.main.gameObject.GetComponent<MyCamera>();
        Camera.SetCharacter(this.transform);
    }

    void Start()
    {
      
        _charController = GetComponent<CharacterController>();
    }
    void FixedUpdate()
    {
        if (Camera.stateCamera is PersonState)
        {
            Vector3 movement = Vector3.zero;
          //  float horInput = Input.GetAxis("Horizontal");
            float vertInput = Input.GetAxis("Vertical");
            if ( vertInput != 0)
            {
         //       movement.x = horInput * moveSpeed;
                movement.z = vertInput * moveSpeed;

                movement = Vector3.ClampMagnitude(movement, moveSpeed);
                Quaternion tmp = Camera.transform.rotation;
                Camera.transform.eulerAngles = new Vector3(0, Camera.transform.eulerAngles.y, 0);
                movement = Camera.transform.TransformDirection(movement);
                Camera.transform.rotation = tmp;
                Quaternion direction = Quaternion.LookRotation(movement);
                transform.rotation = Quaternion.Lerp(transform.rotation,
                direction, rotSpeed * Time.deltaTime);
            }
            movement.y = -9.8f;
            movement *= Time.deltaTime;
            _charController.Move(movement);
        }
    }

}
